using Microsoft.Playwright;
using Infra.Elements; // Ensure your PlaywrightElement is here
using static Microsoft.Playwright.Assertions;

namespace Infra.Drivers
{
    public class PlaywrightUIDriver : IUIDriverAdapter, IAsyncDisposable
    {
        private readonly IPage _page;
        private readonly IBrowser? _browser;
        private readonly IPlaywright? _playwright;

        private PlaywrightUIDriver(IPage page, IBrowser? browser = null, IPlaywright? playwright = null)
        {
            _page = page;
            _browser = browser;
            _playwright = playwright;
        }

        public static async Task<PlaywrightUIDriver> CreateWithNewBrowserAsync()
        {
            var playwright = await Microsoft.Playwright.Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false, // Set to false to see the window
                SlowMo = 1000     // Optional: slows down actions by 1s so you can follow along
            });
            var page = await browser.NewPageAsync();
            return new PlaywrightUIDriver(page, browser, playwright);
        }

        public static async Task<PlaywrightUIDriver> CreateFromContextAsync(IBrowserContext context)
        {
            var page = await context.NewPageAsync();
            return new PlaywrightUIDriver(page);
        }

        public async ValueTask DisposeAsync()
        {
            await _page.CloseAsync();
            if (_browser != null) await _browser.CloseAsync();
            _playwright?.Dispose();
        }

        public string Url => _page.Url;

        public async Task GotoAsync(string url) => await _page.GotoAsync(url);

        public async Task GoToAsync(string url) => await GotoAsync(url);

        public async Task ClickAsync(string selector) => await _page.ClickAsync(selector);

        public async Task FillAsync(string selector, string value) => await _page.FillAsync(selector, value);

        public async Task<string> GetTextAsync(string selector) => await _page.InnerTextAsync(selector);

        public async Task<bool> IsVisibleAsync(string selector) => await _page.IsVisibleAsync(selector);

        public async Task HoverAsync(string selector) => await _page.HoverAsync(selector);

        public async Task PressKeyAsync(string selector, string key) => await _page.PressAsync(selector, key);

        public async Task WaitForSelectorAsync(string selector, int timeoutMilliseconds = 5000) =>
            await _page.WaitForSelectorAsync(selector, new() { Timeout = timeoutMilliseconds });

        public async Task WaitForUrlAsync(string urlPattern) => await _page.WaitForURLAsync(urlPattern);

        // Standardized GetByRole: Synchronous and returns the interface
        public IUIElement GetByRole(string role, string name)
        {
            if (!Enum.TryParse<AriaRole>(role, true, out var ariaRole))
                throw new ArgumentException($"Invalid role string: {role}");

            var locator = _page.GetByRole(ariaRole, new() { Name = name });
            return new PlaywrightElement(locator);
        }

        // Specific helper using the internal Expect logic
        public async Task VerifyHeadingVisibleAsync(string name)
        {
            var locator = _page.GetByRole(AriaRole.Heading, new() { Name = name });
            await Expect(locator).ToBeVisibleAsync();
        }

        IUIElement IUIDriverAdapter.GetByRole(string role, string name)
        {
            if (!Enum.TryParse<AriaRole>(role, true, out var ariaRole))
            {
                throw new ArgumentException($"The role '{role}' is not a valid Playwright AriaRole.");
            }

            // 2. Use the Page to find the locator
            var locator = _page.GetByRole(ariaRole, new() { Name = name });

            // 3. Return your wrapper class that implements IUIElement
            return new PlaywrightElement(locator);
        }
        public IUIElement Find(string selector)
        {

            var childLocator = _page.Locator(selector);
            return new PlaywrightElement(childLocator);
        }

        public async Task<List<IUIElement>> FindAllAsync(string selector)
        {
            // 1. Get the locator for all matches
            var locators = _page.Locator(selector);

            // 2. Resolve them into a list of individual elements
            var elements = new List<IUIElement>();
            int count = await locators.CountAsync();

            for (int i = 0; i < count; i++)
            {
                elements.Add(new PlaywrightElement(locators.Nth(i)));
            }

            return elements;
        }


    }
}
