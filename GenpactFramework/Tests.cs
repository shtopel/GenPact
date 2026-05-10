using Infra.Drivers;
using Infra.PageObject;
using Microsoft.Playwright;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xunit;
using static Infra.PageObject.PlayrightWikiPage;

namespace GenpactFramework
{
    public class Tests : IAsyncLifetime
    {
        private IUIDriverAdapter _driver;
        private IAPIdriver _api;

        private const string PlaywritghtWikiUrl = @"https://en.wikipedia.org/wiki/Playwright_(software)#Debugging_features";

        public async Task InitializeAsync()
        {
            _driver = await PlaywrightUIDriver.CreateWithNewBrowserAsync();
            _api = await PlaywrightApiDriver.CreateAsync();
        }

        [Fact]
        public async Task ValidateApiAndUiResultsAreSame()
        {
         
            var result_from_api = await GetContentViaApi();

            PlayrightWikiPage wp = new PlayrightWikiPage(_driver, PlaywritghtWikiUrl);
            await wp.GoToPage();
            var result_from_ui = await wp.GetTextFromDebuggingFeauteuresAsync();

            var normalizedUi = await NormilizeResultFromUI(result_from_ui);
            var apiWords = await FindDistinctWords(result_from_api);
            var uiWords = await FindDistinctWords(normalizedUi);

            Assert.Equal(apiWords.Count, uiWords.Count);
        }

        [Fact]
        public async Task ValidateAllMicrosoftItemsAreLinks()
        {
            PlayrightWikiPage wp = new PlayrightWikiPage(_driver, PlaywritghtWikiUrl);
            await wp.GoToPage();

            var items = await wp.GetNoneLinkedItemsFromMicrosoftToolsSection();     
            if (items.Contains("Playwright"))
            {
                items.Remove("Playwright");
            }

            Assert.Empty(items);
        }

        [Fact]
        public async Task ValidateBackGroundColorChanged()
        {
            PlayrightWikiPage wp = new PlayrightWikiPage(_driver, PlaywritghtWikiUrl);
            await wp.GoToPage();

            var color_before = await wp.GetBackgroundColor();     
            await wp.ChangeAppereanceBackcolor(ThemeButton.ColorDark);

            var color_after = await wp.GetBackgroundColor();

            Assert.NotEqual(color_before, color_after);
        }
  
        private async Task<string> GetContentViaApi()
        {
            var sectionIndex = await GetSectionIndex(_api, "Playwright_(software)", "Debugging features");
            Assert.NotNull(sectionIndex);

            var content = await GetSectionContent(_api, "Playwright_(software)", sectionIndex!);
            return CleanContent(content!);
        }

        private static async Task<string?> GetSectionContent(IAPIdriver api, string page, string sectionIndex)
        {
            var json = await api.GetAsync("https://en.wikipedia.org/w/api.php", new()
            {
                ["action"] = "parse",
                ["page"] = page,
                ["section"] = sectionIndex,
                ["prop"] = "wikitext",
                ["format"] = "json",
                ["origin"] = "*"
            });

            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.GetProperty("parse").GetProperty("wikitext").GetProperty("*").GetString();
        }

        private static async Task<string?> GetSectionIndex(IAPIdriver api, string page, string sectionName)
        {
            var json = await api.GetAsync("https://en.wikipedia.org/w/api.php", new()
            {
                ["action"] = "parse",
                ["page"] = page,
                ["prop"] = "sections",
                ["format"] = "json",
                ["origin"] = "*"
            });

            using var doc = JsonDocument.Parse(json);
            foreach (var section in doc.RootElement.GetProperty("parse").GetProperty("sections").EnumerateArray())
            {
                if (section.GetProperty("line").GetString() == sectionName)
                    return section.GetProperty("index").GetString();
            }
            return null;
        }

        // --- Logic Helpers ---

        private static async Task<List<string>> FindDistinctWords(string str)
        {
            return str.Split(' ', System.StringSplitOptions.RemoveEmptyEntries).Distinct().ToList();
        }

        private static async Task<string> NormilizeResultFromUI(List<string> lst)
        {
            string result = string.Join(" ", lst);
            return CleanContent(result);
        }

        private static string CleanContent(string input)
        {
            input = Regex.Replace(input, @"\{\{.*?\}\}", "");
            input = Regex.Replace(input, @"<ref.*?>.*?<\/ref>", "");
            input = Regex.Replace(input, @"\*+", "");
            input = Regex.Replace(input, @"\s+", " ");
            input = Regex.Replace(input, @"=", "");
            return input.Trim();
        }

        public async Task DisposeAsync()
        {
            if (_driver is IAsyncDisposable disposable) await disposable.DisposeAsync();
        }
    }
}