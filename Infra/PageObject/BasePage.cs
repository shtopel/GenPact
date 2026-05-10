using Infra.Drivers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infra.PageObject
{
    public abstract class BasePage
    {

        protected IUIDriverAdapter Driver { get; }

        protected string url;



        public BasePage(IUIDriverAdapter driver,string url)
        {
            Driver = driver;
            this.url = url;
        }



        public async Task<string> GetBackgroundColor()
        {

            var body = Driver.Find("body");

            return await body.EvaluateAsync<string>("element => window.getComputedStyle(element).backgroundColor");
        }

        public async Task GoToPage()
        {
            await Driver.GotoAsync(url);
        }

    }
}
