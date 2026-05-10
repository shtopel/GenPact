using Infra.Drivers;
using Infra.Elements;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.PageObject
{
    public class PlayrightWikiPage : BasePage
    {
        public const string debugging_feautures_name = "Debugging features";
        public const string external_links_name = "External links";



        public PlayrightWikiPage(IUIDriverAdapter driver, string url = "" ) : base(driver)
        {
            this.url = url;
        }
        
        public async Task< List<string>> GetTextFromDebuggingFeauteuresAsync()
        {
            await Driver.GotoAsync(url);
            await ClickOnDebuggingFeauteures();
            var items = await  GetItemsFromDebuggingFeauturesList();

            List<string> result = new List<string>(); 
            foreach (var item in items)
            {           
                result.Add(await item.GetTextAsync());
            }

            result.Add(GetParagraphTextFRomDebuggingFeauturesList().Result);
            result.Add("Debugging features");
            return result;
        }


        public async Task<List<string>> GetNoneLinkedItemsFromMicrosoftToolsSection()
        {
            await Driver.GotoAsync(url);            
            var  items = await GetMicrosoftDevelopmentToolsItems();
            return await GetAllItemsWithoutALinkFromTab(items);

        }




        public async Task GoTOContentLinkBySubject(string name)
        {
            Driver.GetByRole("link", name).ClickAsync();
            var heading = Driver.GetByRole("heading", name);
            await heading.ShouldBeVisibleAsync();
            return;
        }
        private async Task ClickOnDebuggingFeauteures()
        {
            //Driver.GetByRole("link", debugging_feautures_name).ClickAsync();
            //var heading = Driver.GetByRole("heading", "Debugging features");
            //await heading.ShouldBeVisibleAsync();
            await GetMicrosoftDevelopmentToolsItems();
           // await GoTOContentLinkBySubject(debugging_feautures_name);
            return;
        }

        private async Task<List<string>> GetAllItemsWithoutALinkFromTab(List<IUIElement> techContainers)
        {

            List<string> failures = new List<string>();
            List<string> names_without_link = new();
            var validationTasks = techContainers.Select(async container =>
            {
                var links = await container.GetAllByRoleAsync("link");
                var text = await container.GetTextAsync();

                // Return a tuple or a small object with the results
                return new { Text = text.Trim(), HasLink = links.Count > 0 };
            });

            // 2. Wait for all parallel tasks to finish
            var results = await Task.WhenAll(validationTasks);

            // 3. Filter the results for failures
            var namesWithoutLink = results
                .Where(r => !r.HasLink && !string.IsNullOrWhiteSpace(r.Text))
                .Select(r => r.Text)
                .ToList();

            // 4. Final Validation
            if (namesWithoutLink.Any())
            {
                //Assert.Fail($"Task 2 Failed: The following are not links: {string.Join(", ", namesWithoutLink)}");
            }
            return namesWithoutLink;

        }
        private async Task<List<IUIElement>> GetMicrosoftDevelopmentToolsItems()
        {
           
            await GoTOContentLinkBySubject(external_links_name);
            var microsoftTable = Driver.Find(".navbox:has-text('Microsoft development tools')");

            // 2. Find the "show" button within that navbox
            // Based on your screenshot, it's a button with the class 'mw-collapsible-toggle'
            var showButton = microsoftTable.Find("button.mw-collapsible-toggle");

            // 3. Click "show" if it's currently collapsed
            // We can check the text or the aria-expanded attribute
            var buttonText = await showButton.GetTextAsync();
            if (buttonText.Contains("show", StringComparison.OrdinalIgnoreCase))
            {
                await showButton.ClickAsync();
            }

            // Give it a tiny bit of time to animate/render if necessary
            // await Task.Delay(500); 
            var techContainers = await microsoftTable.FindAllAsync("td.navbox-list li");

            return techContainers;

            

        }
        private async Task<IReadOnlyList<IUIElement>> GetItemsFromDebuggingFeauturesList()
        {

            var list = Driver.Find("xpath=(//h3|//h2)[contains(., 'Debugging features')]/following::ul[1]"); 
            return await list.GetAllByRoleAsync("listitem");

        }

        private async Task<string> GetParagraphTextFRomDebuggingFeauturesList()
        {
     
            var list = Driver.Find("xpath=(//h3|//h2)[contains(., 'Debugging features')]/following::p[1]");
            return await list.GetTextAsync();
        }

    }
}
