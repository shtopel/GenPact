using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.Elements
{
    public interface IUIElement
    {
        Task ClickAsync();
        Task DblClickAsync();
        Task FillAsync(string value);
        Task ClearAsync();
        Task HoverAsync();
        Task PressKeyAsync(string key);

        // Data
        Task<string> GetTextAsync();
        Task<string?> GetAttributeAsync(string name);
        Task<bool> IsEnabledAsync();

        // Assertions (Retrying)
        Task ShouldBeVisibleAsync();
        Task ShouldBeHiddenAsync();
        Task ShouldHaveTextAsync(string text);
        Task ShouldBeEnabledAsync();
        IUIElement GetByRole(string role, string name);
        
        Task<IReadOnlyList<IUIElement>> GetAllByRoleAsync(string role);
        IUIElement Find(string selector);

        Task<List<IUIElement>> FindAllAsync(string selector);
        Task<T> EvaluateAsync<T>(string v);
    }
}
