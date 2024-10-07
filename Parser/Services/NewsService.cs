using System.Text.Json.Nodes;
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
        throw new NotImplementedException();
    }

    public async Task<string> GetMostUsedWords()
    {
        throw new NotImplementedException();
    }

    public async Task<List<News>> GetNewsByWord(string word)
    {
        throw new NotImplementedException();
    }

    public async Task ParseAndSaveNewsAsync()
    {
        using HttpClient client = new HttpClient();
        for (int i = 1; i < 2; i++)
        {
            var response = await client.GetAsync(BaseUrl + $"/api/today-news/?pn={i}&pSize=20");
            var content = await response.Content.ReadAsStringAsync();

            var jsonObject = JsonNode.Parse(content)!.AsObject();
            var news = jsonObject["data_list"]!.AsArray();

            foreach (var item in news)
            {
                var newsTitle = item?["page_title"]?.GetValue<string>();
                var date = item?["published_date"]?.GetValue<string>();
                var dateOfPublish = DateTime.Parse(date!);
                var contentText = await ParseContentFromUrl(BaseUrl + item!["alias"]?.GetValue<string>());
                await _connection.ExecuteAsync(@"insert into news (title,content,date_of_publish)
            values (@title,@content,@date_of_publish)",
                    new { title = newsTitle, content = contentText, date_of_publish = dateOfPublish });
            }
        }
        
        _logger.LogInformation("Parsed new news successfully");
    }

    private async Task<string> ParseContentFromUrl(string url)
    {
        using HttpClient client = new HttpClient();
        var result = await client.GetAsync(url);
        var html=await result.Content.ReadAsStringAsync();
        
        var pattern = @"<div class=""content"".*?>([\s\S]*?)<\/div>";
        
        var match = Regex.Match(html, pattern);

        if (!match.Success) throw new Exception("Could not parse content");
        var content = match.Groups[1].Value;
            
        var textOnly = Regex.Replace(content, "<.*?>", String.Empty).Trim();
        return textOnly;

    }
}