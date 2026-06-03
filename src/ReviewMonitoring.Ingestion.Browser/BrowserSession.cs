// Ingestion.Browser/BrowserSession.cs
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;
using ReviewMonitoring.Ingestion.Interfaces;

namespace ReviewMonitoring.Ingestion.Browser;

internal class BrowserSession : IBrowserSession
{
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private readonly ILogger<BrowserSession> _log;
    private bool _initialized;

    public BrowserSession(ILogger<BrowserSession> log)
    {
        _log = log;
    }

    public async Task InitializeAsync()
    {
        if (_initialized)
            return;

        _playwright = await Playwright.CreateAsync();

        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = false,
            Args =
            [
                "--disable-blink-features=AutomationControlled",
                "--disable-dev-shm-usage",
                "--no-sandbox"
            ]
        });

        _context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            ViewportSize = new ViewportSize { Width = 1920, Height = 1080 },
            UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/120.0.0.0 Safari/537.36",
            JavaScriptEnabled = true,
            IgnoreHTTPSErrors = true
        });

        await _context.AddInitScriptAsync("""
            Object.defineProperty(navigator, 'webdriver', { get: () => undefined });
            Object.defineProperty(navigator, 'plugins', { get: () => [1, 2, 3, 4, 5] });
        """);

        _initialized = true;
        _log.LogInformation("Browser initialized");
    }

    private async Task EnsureInitializedAsync()
    {
        if (_initialized)
            return;
        await InitializeAsync();
    }

    public async Task<string> GetPageHtmlAsync(string url, CancellationToken ct)
    {
        await EnsureInitializedAsync();
        return await GetPageHtmlWithScrollAsync(url, 0, ct);
    }

    public async Task<string> GetPageHtmlWithScrollAsync(
        string url,
        int scrollCount,
        CancellationToken ct)
    {
        if (_context is null)
            throw new InvalidOperationException("Browser not initialized. Call InitializeAsync first.");

        var page = await _context.NewPageAsync();

        try
        {
            _log.LogInformation("Navigating to {Url}", url);

            await page.GotoAsync(url, new PageGotoOptions
            {
                WaitUntil = WaitUntilState.DOMContentLoaded,
                Timeout = 20000
            });

            try
            {
                await page.WaitForSelectorAsync("h1", new PageWaitForSelectorOptions
                {
                    Timeout = 5000
                });
            }
            catch (TimeoutException)
            {
                // продолжаем энивей
                _log.LogWarning("h1 not found on {Url} — proceeding anyway", url);
            }

            for (int i = 0; i < scrollCount; i++)
            {
                await page.EvaluateAsync("window.scrollBy(0, window.innerHeight)");
                await Task.Delay(Random.Shared.Next(800, 1500), ct);
            }

            await Task.Delay(Random.Shared.Next(1500, 3000), ct);

            return await page.ContentAsync();
        }
        finally
        {
            await page.CloseAsync();
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_browser is not null)
            await _browser.CloseAsync();

        _playwright?.Dispose();
        _initialized = false;
    }
}