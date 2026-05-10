using Infra.Drivers;
using Microsoft.Playwright;
using System.Text.Json;

public class PlaywrightApiDriver : IAPIdriver
{
    private readonly IPlaywright _playwright;
    private readonly IAPIRequestContext _api;

    private PlaywrightApiDriver(IPlaywright playwright, IAPIRequestContext api)
    {
        _playwright = playwright;
        _api = api;
    }

    public static async Task<PlaywrightApiDriver> CreateAsync()
    {
        var playwright = await Playwright.CreateAsync();
        var api = await playwright.APIRequest.NewContextAsync();

        return new PlaywrightApiDriver(playwright, api);
    }


    public async Task<string> GetAsync(string url, Dictionary<string, object>? query = null)
    {
        var response = await _api.GetAsync(url, new()
        {
            Params = query
        });

        return await response.TextAsync();
    }

    public async Task<string> PostAsync(string url, object? body = null)
    {
        var response = await _api.PostAsync(url, new()
        {
            DataObject = body
        });

        return await response.TextAsync();
    }

    public async ValueTask DisposeAsync()
    {
        await _api.DisposeAsync();
        _playwright.Dispose();
    }
}

