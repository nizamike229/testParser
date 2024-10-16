﻿using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Dapper;
using Npgsql;
using WebApplication1.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.Services;

public class NewsService : INewsService
{
    private readonly NpgsqlConnection _connection;
    private readonly ILogger<INewsService> _logger;
    private static readonly string BaseUrl = "https://www.zakon.kz/";

    public NewsService(NpgsqlConnection connection, ILogger<INewsService> logger)
    {
        _connection = connection;
        _logger = logger;
    }

    public async Task<List<News>> GetNewsByDates(DateTime from, DateTime to)
    {
        var news = await _connection.QueryAsync<News>(
            "SELECT title as Title, content as Content,date_of_publish as PublishDate FROM news WHERE date_of_publish BETWEEN @startDate AND @endDate ORDER BY date_of_publish;",
            new
            {
                startDate = from,
                endDate = to
            });

        return news.ToList();
    }

    public async Task<Dictionary<string, int>> GetMostUsedWords()
    {
        var allTexts = (await _connection.QueryAsync<string>("select content from news")).ToList();
        var wordFrequency = new SortedDictionary<string, int>();

        foreach (var text in allTexts)
        {
            var words = text.ToLower().Split([' ', ',', '.', '!', '?'], StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                if (word.Length < 4) continue;
                if (wordFrequency.TryAdd(word, 1)) continue;
                wordFrequency[word]++;
                break;
            }
        }

        return wordFrequency.OrderByDescending(x => x.Value).Take(10).ToDictionary(x => x.Key, x => x.Value);
    }

    public async Task<List<News>> GetNewsByWord(string word)
    {
        var result = await _connection.QueryAsync<News>(
            "select content as Content, title as Title, date_of_publish as PublishDate " +
            "from news where lower(title) like '%' || @searchWord || '%';",
            new
            {
                searchWord = word.ToLower()
            });

        return result.ToList();
    }

    public async Task ParseAndSaveNewsAsync()
    {
        await _connection.ExecuteAsync("delete from news");
        using HttpClient client = new HttpClient();
        var usedPages = new List<int>();
        for (int i = 0; i < 10; i++)
        {
            Random random = new Random();
            var randomNum = random.Next(15, 200);
            while (usedPages.Contains(randomNum))
                randomNum = random.Next(1, 55);

            usedPages.Add(randomNum);
            var response = await client.GetAsync(BaseUrl + $"/api/today-news/?pn={randomNum}&pSize=3");
            var content = await response.Content.ReadAsStringAsync();

            var jsonObject = JsonNode.Parse(content)!.AsObject();
            var news = jsonObject["data_list"]!.AsArray();

            foreach (var item in news)
            {
                var newsTitle = item?["page_title"]?.GetValue<string>();
                var date = item?["published_date"]?.GetValue<string>();
                var dateOfPublish = DateTime.Parse(date!);
                var contentText = await ParseContentFromUrl(BaseUrl + item!["alias"]?.GetValue<string>());
                await _connection.ExecuteAsync("call insert_news(@title,@content,@date_of_publish)",
                    new { title = newsTitle, content = contentText, date_of_publish = dateOfPublish });
            }
        }

        _logger.LogInformation("Parsed new news successfully");
    }

    private async Task<string> ParseContentFromUrl(string url)
    {
        using HttpClient client = new HttpClient();
        var result = await client.GetAsync(url);
        var html = await result.Content.ReadAsStringAsync();

        var pattern = @"<div class=""content"".*?>([\s\S]*?)<\/div>";

        var match = Regex.Match(html, pattern);

        if (!match.Success) throw new Exception("Could not parse content");
        var content = match.Groups[1].Value;

        var textOnly = Regex.Replace(content, "<.*?>", string.Empty).Trim();
        return textOnly;
    }

    public async Task CreateTablesAsync()
    {
        await _connection.ExecuteAsync(
            "create table if not exists news(    id              serial       constraint news_pk           primary key,   title           varchar(300) not null,   date_of_publish timestamp    not null,     content         varchar      not null );  create table if not exists users (     id       serial         constraint users_pk             primary key,     username varchar(255),     password varchar(300) );  create or replace procedure insert_news(IN p_title text, IN p_content text, IN p_date_of_publish timestamp without time zone)     language plpgsql as $$ BEGIN     INSERT INTO news (title, content, date_of_publish)     VALUES (p_title, p_content, p_date_of_publish); END; $$;  create or replace procedure create_user(IN p_login text, IN p_password text)     language plpgsql as $$ BEGIN     INSERT INTO users (username, password)     VALUES (p_login, p_password); END; $$;  create or replace function check_user_valid(p_username character varying, p_password character varying) returns boolean     language plpgsql as $$ DECLARE     stored_hash VARCHAR(255); BEGIN     SELECT password INTO stored_hash     FROM users     WHERE username = p_username;      IF stored_hash IS NULL THEN         RETURN false;     END IF;      RETURN stored_hash = p_password; END; $$;  ");
    }
}