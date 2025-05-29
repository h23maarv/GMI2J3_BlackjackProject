using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using Xunit;

namespace Blackjack.Tests
{
    public class SeleniumTests
    {
        [Fact]
        public void Can_Play_Two_Games()
        {
            using var driver = new EdgeDriver();
            driver.Navigate().GoToUrl("https://blackjack-dydzc5gjcdddbsbb.swedencentral-01.azurewebsites.net");
            for (int game = 0; game < 2; game++)
            {
                // Wait for bet input.
                var betInput = driver.FindElement(By.Name("bet"));
                betInput.Clear();
                betInput.SendKeys("1000");

                // Click the Deal button.
                var dealButton = driver.FindElement(By.CssSelector("form[action*='Deal'] button"));
                dealButton.Click();
                System.Threading.Thread.Sleep(1200);
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(5));
                var playerValueElement = wait.Until(drv => drv.FindElement(By.Id("playerValue")));
                var playerValueInt = int.Parse(playerValueElement.Text);

                if (playerValueInt < 17 && playerValueInt < 21)
                {
                    var hitButtons = driver.FindElements(By.CssSelector("form[action*='Hit'] button"));
                    if (hitButtons.Any())
                    {
                        hitButtons.First().Click();
                        System.Threading.Thread.Sleep(1000);
                        playerValueElement = wait.Until(drv => drv.FindElement(By.Id("playerValue")));
                        playerValueInt = int.Parse(playerValueElement.Text);
                    }

                }

                if (playerValueInt >= 17 || playerValueInt == 21)
                {
                    // No Hit button found, must stand.
                    var standButtons = driver.FindElements(By.CssSelector("form[action*='Stand'] button"));
                    if (standButtons.Any())
                    {
                        standButtons.First().Click();
                        System.Threading.Thread.Sleep(1000);
                    }
                }



                while (true)
                {
                    // Kontrollera om resultat visas (d� �r spelet �ver)
                    var resultElements = driver.FindElements(By.XPath(
                        "//h4[span[contains(text(), 'Du vann!') or contains(text(), 'Dealern vann.') or contains(text(), 'Oavgjort!')]]"));
                    if (resultElements.Any())
                        break;

                    var playerValueElementLoop = driver.FindElement(By.Id("playerValue"));
                    playerValueInt = int.Parse(playerValueElementLoop.Text);

                    if (playerValueInt < 17 && playerValueInt < 21)
                    {
                        // Tryck p� Hit om det �r till�tet
                        var hitButtons = driver.FindElements(By.CssSelector("form[action*='Hit'] button"));
                        if (hitButtons.Any())
                        {
                            hitButtons.First().Click();
                            System.Threading.Thread.Sleep(1000);
                            continue;
                        }
                    }

                    if (playerValueInt >= 17 || playerValueInt == 21)
                    {
                        // Har vi 17 eller mer, klicka alltid Stand om det g�r
                        var standButtons = driver.FindElements(By.CssSelector("form[action*='Stand'] button"));
                        if (standButtons.Any())
                        {
                            standButtons.First().Click();
                            System.Threading.Thread.Sleep(1000);
                        }
                        else
                        {
                            if (playerValueInt < 21)
                            {
                                // Om stand ej finns, f�rs�k hitta Hit f�r s�kerhets skull
                                var hitButtons = driver.FindElements(By.CssSelector("form[action*='Hit'] button"));
                                if (hitButtons.Any())
                                {
                                    hitButtons.First().Click();
                                    System.Threading.Thread.Sleep(1000);
                                }
                            }
                        }
                    }
                    System.Threading.Thread.Sleep(1000); // V�nta in n�sta UI-uppdatering
                }

                var resultText = driver.FindElement(By.XPath(
                    "//h4[span[contains(text(), 'Du vann!') or contains(text(), 'Dealern vann.') " +
                    "or contains(text(), 'Oavgjort!')]]")).Text;
                Assert.Contains("Resultat: ", resultText);
                System.Threading.Thread.Sleep(1000);

                if (game < 1)
                {
                    var playAgainLink = driver.FindElement(By.LinkText("Spela igen"));
                    playAgainLink.Click();
                    System.Threading.Thread.Sleep(1200);
                }
            }
        }
    }
}
