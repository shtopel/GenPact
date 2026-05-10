using Infra.Drivers;
using Microsoft.Playwright;
using static Microsoft.Playwright.Assertions; // Needed for Expect()

namespace Infra.Elements
{
    public class PlaywrightElement : IUIElement
    {
        private readonly ILocator _locator;

        public PlaywrightElement(ILocator locator)
        {
            _locator = locator;
        }

        public async Task ClickAsync() => await _locator.ClickAsync();
        public async Task FillAsync(string value) => await _locator.FillAsync(value);
        public async Task HoverAsync() => await _locator.HoverAsync();
        public async Task ClearAsync() => await _locator.ClearAsync();
        public async Task<string> GetTextAsync() => await _locator.InnerTextAsync();
        public async Task<string?> GetAttributeAsync(string name) => await _locator.GetAttributeAsync(name);

        public async Task ShouldBeVisibleAsync() => await Expect(_locator).ToBeVisibleAsync();
        public async Task ShouldBeHiddenAsync() => await Expect(_locator).ToBeHiddenAsync();
        public async Task ShouldHaveTextAsync(string text) => await Expect(_locator).ToHaveTextAsync(text);

        public IUIElement GetByRole(string role, string name)
        {
            if (!Enum.TryParse<AriaRole>(role, true, out var ariaRole))
                throw new ArgumentException($"Invalid AriaRole: {role}");

            var nestedLocator = _locator.GetByRole(ariaRole, new() { Name = name });
            return new PlaywrightElement(nestedLocator);
        }

        public async Task<IReadOnlyList<IUIElement>> GetAllByRoleAsync(string role)
        {
            if (!Enum.TryParse<AriaRole>(role, true, out var ariaRole))
                throw new ArgumentException($"Invalid role: {role}");

            var locators = await _locator.GetByRole(ariaRole).AllAsync();

            return locators
                .Select(l => (IUIElement)new PlaywrightElement(l))
                .ToList()
                .AsReadOnly();
        }

        public IUIElement Find(string selector)
        {
           
            var childLocator = _locator.Locator(selector);
            return new PlaywrightElement(childLocator);
        }

        public async Task<List<IUIElement>> FindAllAsync(string selector)
        {
           
            var locators = _locator.Locator(selector);
              var elements = new List<IUIElement>();
            int count = await locators.CountAsync();

            for (int i = 0; i < count; i++)
            {
                elements.Add(new PlaywrightElement(locators.Nth(i)));
            }

            return elements;
        }

        public Task DblClickAsync()
        {
            throw new NotImplementedException();
        }

        public Task PressKeyAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEnabledAsync()
        {
            throw new NotImplementedException();
        }

        public Task ShouldBeEnabledAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<T> EvaluateAsync<T>(string expression)
        {
            return await _locator.EvaluateAsync<T>(expression);
        }
 
    }
}
