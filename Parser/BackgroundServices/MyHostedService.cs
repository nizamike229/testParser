using WebApplication1.Interfaces;

namespace WebApplication1.BackgroundServices;

public class MyHostedService : IHostedService
{
    private readonly INewsService _newsService;

    public MyHostedService(INewsService newsService)
    {
        _newsService = newsService;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _newsService.ParseAndSaveNewsAsync();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}