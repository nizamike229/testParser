using WebApplication1.Interfaces;

namespace WebApplication1.BackgroundServices;

public class MyHostedService : IHostedService
{
    private readonly INewsService _newsService;

    public MyHostedService(INewsService newsService)
    {
        _newsService = newsService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        for (var i = 0; i < 2; i++)
        {
            try
            {
                await _newsService.CreateTablesAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine("Database connection to migrate is missing");
                Thread.Sleep(2000);
            }
        }

        await _newsService.ParseAndSaveNewsAsync();
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}