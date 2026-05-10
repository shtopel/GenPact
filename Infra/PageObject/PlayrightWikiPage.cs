using Infra.Drivers;
using Infra.Elements;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.PageObject
{
    public class PlayrightWikiPage : BasePage
    {
  
        public const string debugging_feautures_name = "Debugging features";
        public const string external_links_name = "External links";


        public enum ThemeButton
        {
            [Description("#skin-client-pref-skin-theme-value-night")]
            ColorDark,

            [Description("#skin-client-pref-skin-theme-value-day")]
            ColorLight,

            [Description("#skin-client-pref-skin-theme-value-os")]
            ColorAutomatic
        }

        public PlayrightWikiPage(IUIDriverAdapter driver, string url = "") : base(driver, url)
        {
            base.url = url;
        }

        public async Task<List<string>> GetTextFromDebuggingFeauteuresAsync()
        {
            await Driver.GotoAsync(url);
            await ClickOnDebuggingFeauteures();

            var items = await GetItemsFromDebuggingFeauturesList();
            List<string> result = new List<string>();

            foreach (var item in items)
            {
                result.Add(await item.GetTextAsync());
            }

            var paragraphText = await GetParagraphTextFRomDebuggingFeauturesList();
            result.Add(paragraphText);
            result.Add(debugging_feautures_name);

            return result;
        }

        public async Task ChangeAppereanceBackcolor(ThemeButton color)
        {       
            string selector = color.GetValue();
            var radio = Driver.Find(selector);
            await radio.ClickAsync();
            
 
        }

        public async Task<List<string>> GetNoneLinkedItemsFromMicrosoftToolsSection()
        {
            await Driver.GotoAsync(url);
            var items = await GetMicrosoftDevelopmentToolsItems();
            return await GetAllItemsWithoutALinkFromTab(items);
        }

        public async Task GoTOContentLinkBySubject(string name)
        {
            await Driver.GetByRole("link", name).ClickAsync();
            var heading = Driver.GetByRole("heading", name);
            await heading.ShouldBeVisibleAsync();
        }

        private async Task ClickOnDebuggingFeauteures()
        {
            await GoTOContentLinkBySubject(debugging_feautures_name);
        }

        private async Task<List<string>> GetAllItemsWithoutALinkFromTab(List<IUIElement> techContainers)
        {
            var validationTasks = techContainers.Select(async container =>
            {
                var links = await container.GetAllByRoleAsync("link");
                var text = await container.GetTextAsync();

                return new { Text = text.Trim(), HasLink = links.Count > 0 };
            });

            var results = await Task.WhenAll(validationTasks);

            return results
                .Where(r => !r.HasLink && !string.IsNullOrWhiteSpace(r.Text))
                .Select(r => r.Text)
                .ToList();
        }

        private async Task<List<IUIElement>> GetMicrosoftDevelopmentToolsItems()
        {
            await GoTOContentLinkBySubject(external_links_name);

            var microsoftTable = Driver.Find(".navbox:has-text('Microsoft development tools')");
            var showButton = microsoftTable.Find("button.mw-collapsible-toggle");

            var buttonText = await showButton.GetTextAsync();
            if (buttonText.Contains("show", StringComparison.OrdinalIgnoreCase))
            {
                await showButton.ClickAsync();
                await Task.Delay(300);
            }

            return await microsoftTable.FindAllAsync("td.navbox-list li");
        }

        private async Task<IReadOnlyList<IUIElement>> GetItemsFromDebuggingFeauturesList()
        {
            var list = Driver.Find("xpath=(//h3|//h2)[contains(., 'Debugging features')]/following::ul[1]");
            return await list.GetAllByRoleAsync("listitem");
        }

        private async Task<string> GetParagraphTextFRomDebuggingFeauturesList()
        {
            var paragraph = Driver.Find("xpath=(//h3|//h2)[contains(., 'Debugging features')]/following::p[1]");
            return await paragraph.GetTextAsync();
        }
    }
}