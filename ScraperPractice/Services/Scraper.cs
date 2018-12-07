using ScraperPractice.Models;
using Microsoft.Extensions.Options;
using System;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace ScraperPractice.Services
{
    public class Scraper
    {
        private string userName;
        private string password;

        public Scraper(string userName, string password)
        {
            this.userName = userName;
            this.password = password;
        }

        public List<Stock> Scrape()
        {
            // Add --headless to options 
            var options = new ChromeOptions();
            options.AddArguments("--headless");
            options.AddArguments("--disable-gpu");
            options.AddArguments("disable-popup-blocking");

            // Initial new ChromeDriver and navigate to Yahoo URL
            var chromeDriver = new ChromeDriver("C:\\Users\\Sreenath\\source\\repos\\ScraperPractice", options);

            chromeDriver.Navigate().GoToUrl("https://login.yahoo.com");
            chromeDriver.Manage().Window.Maximize();

            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            // Input username and click on Next 
            chromeDriver.FindElementById("login-username").SendKeys(this.userName);
            chromeDriver.FindElementById("login-signin").Click();

            // Input password and click on submit
            chromeDriver.FindElementById("login-passwd").SendKeys(this.password);
            chromeDriver.FindElementById("login-signin").Click();


            // After password verification navigate to Yahoo portfolio page

            //chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            chromeDriver.Url = "https://finance.yahoo.com/portfolio/p_0/view/v1";

            //var closePopUp = chromeDriver.FindElement(By.XPath("//*[@id=\"fin-tradeit\"]/div[2]/div/div/div[2]/button[2]"));
            //closePopUp.Click();
            var closePopup = chromeDriver.FindElementByXPath("//dialog[@id = '__dialog']/section/button");
            closePopup.Click();

            //var stocks = chromeDriver.FindElements(By.XPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[*]/td[*]"));
            //foreach (var stock in stocks)
            //    Console.WriteLine(stock.Text);

            IWebElement list = chromeDriver.FindElementByTagName("tbody");
            System.Collections.ObjectModel.ReadOnlyCollection<IWebElement> stocks = list.FindElements(By.TagName("tr"));
            int count = stocks.Count();

            List<Stock> stockList = new List<Stock>();
            for(int i = 1; i <= count; i++)
            {
                var symbol = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[1]/span/a").GetAttribute("innerText");
                var lastPrice = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[2]/span").GetAttribute("innerText");
                var change = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[3]/span").GetAttribute("innerText");
                var percentChange = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[4]/span").GetAttribute("innerText");
                var currency = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[5]").GetAttribute("innerText");
                var marketCap = chromeDriver.FindElementByXPath("//*[@id=\"main\"]/section/section[2]/div[2]/table/tbody/tr[" + i + "]/td[13]/span").GetAttribute("innerText");

                Stock stock = new Stock();
                stock.Symbol = symbol;
                stock.LastPrice = Decimal.Parse(lastPrice);
                stock.Change = Decimal.Parse(change);
                //stock.PercentChange = Decimal.Parse(percentChange);
                stock.PercentChange = Decimal.Parse(percentChange.Trim('%'));
                stock.Currency = currency;
                stock.MarketCap = marketCap;
            
                //Console.WriteLine("symbol: " + symbol);
                stockList.Add(stock);
            }
            return stockList;
        }
    }
}