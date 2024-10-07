using WebApplication1.Models;

namespace WebApplication1.Interfaces;

public interface INewsService
{
    Task<List<News>> GetNewsByDates(DateTime from, DateTime to);
    Task<string> GetMostUsedWords();
    Task<List<News>> GetNewsByWord(string word);
    Task ParseAndSaveNewsAsync();
}