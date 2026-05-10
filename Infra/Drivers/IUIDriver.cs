using Infra.Elements;

namespace Infra.Drivers
{
    public interface IUIDriverAdapter
    {
        // Navigation
        Task GotoAsync(string url);
        string Url { get; }

        // Global Selector-based Interactions (Direct)
        Task ClickAsync(string selector);
        Task FillAsync(string selector, string value);
        Task<string> GetTextAsync(string selector);
        Task<bool> IsVisibleAsync(string selector);

        // Advanced Actions
        Task HoverAsync(string selector);
        Task PressKeyAsync(string selector, string key);

        // Wait Logic
        Task WaitForSelectorAsync(string selector, int timeoutMilliseconds = 5000);
        Task WaitForUrlAsync(string urlPattern);

        // Role-based Access (Returns a decoupled Element)
        IUIElement GetByRole(string role, string name);

        IUIElement Find(string selector);

        Task<List<IUIElement>> FindAllAsync(string selector);
    }
}
