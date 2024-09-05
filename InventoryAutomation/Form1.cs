using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Threading;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.Text.Json;
using System.Xml.Schema;

namespace InventoryAutomation
{
    public partial class Form1 : Form
    {
        private IWebDriver driver;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var inventoryUrl = textBox1.Text;
            var email = textBox2.Text;
            var password = textBox3.Text;

            ChromeOptions options = new ChromeOptions();
            //options.AddArgument("headless");
            driver = new ChromeDriver(options);  // Make sure ChromeDriver.exe is in PATH or provide the path
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            // Step 1: Navigate to Kream website
            driver.Navigate().GoToUrl("https://kream.co.kr");

            // Wait for the page to load completely before executing the script
            //System.Threading.Thread.Sleep(10000);  // Adjust the wait time as needed

            // Use WebDriverWait to wait until the page is fully loaded
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until(driver => js.ExecuteScript("return document.readyState").Equals("complete"));

            // Prepare the JavaScript code
            string loginScript = @"
                const myHeaders = new Headers();
                    myHeaders.append('Host', 'api.kream.co.kr');
                    myHeaders.append('Content-Length', '55');
                    myHeaders.append('sec-ch-ua', '""Chromium"";v=""128"", ""Not;A=Brand"";v=""24"", ""Google Chrome"";v=""128""');
                    myHeaders.append('x-kream-device-id', 'web;5bcfa2a0-e949-41f2-bc2e-4bf78d96ce55');
                    myHeaders.append('sec-ch-ua-mobile', '?0');
                    myHeaders.append('x-kream-web-request-secret', 'kream-djscjsghdkd');
                    myHeaders.append('user-agent', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36');
                    myHeaders.append('x-kream-web-build-version', '6.0.1');
                    myHeaders.append('content-type', 'application/json');
                    myHeaders.append('accept', 'application/json, text/plain, */*');
                    myHeaders.append('x-kream-client-datetime', '20240905081847+1000');
                    myHeaders.append('x-kream-api-version', '35');
                    myHeaders.append('sec-ch-ua-platform', '""Windows""');
                    myHeaders.append('origin', 'https://kream.co.kr');
                    myHeaders.append('sec-fetch-site', 'same-site');
                    myHeaders.append('sec-fetch-mode', 'cors');
                    myHeaders.append('sec-fetch-dest', 'empty');
                    myHeaders.append('referer', 'https://kream.co.kr/');
                    myHeaders.append('accept-encoding', 'gzip, deflate, br, zstd');
                    myHeaders.append('accept-language', 'en-US,en;q=0.9');
                    myHeaders.append('priority', 'u=1, i');
                    myHeaders.append('Cookie', 'csrf_refresh_token=8f59f12d-c605-4761-8833-8363e453fa05; refresh_token_cookie=eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...');

                const raw = JSON.stringify({
                    email: '" + email + @"',
                    password: '" + password + @"'
                });

                const requestOptions = {
                    method: 'POST',
                    headers: myHeaders,
                    body: raw,
                    redirect: 'follow',
                    mode: 'cors'
                };

                return (async () => {
                    try {
                        const response = await fetch('https://api.kream.co.kr/api/auth/login?request_key=a5b31fea-b853-4864-8ded-0b7cc05cfa0e', requestOptions);
                        const result = await response.text();
                        
                        return result;  // Return result to C#
                    } catch (error) {
                        return 'Error: ' + error;
                    }
                })();
            ";
            //MessageBox.Show("loginScript: " + loginScript);

            try
            {
                var result = js.ExecuteScript(loginScript);

                // Parse the JSON result to extract access token
                string jsonResponse = result.ToString();
                using (JsonDocument doc = JsonDocument.Parse(jsonResponse))
                {
                    JsonElement root = doc.RootElement;
                    if (root.TryGetProperty("access_token", out JsonElement tokenElement)) {
                        string accessToken = tokenElement.GetString();
                        MessageBox.Show("Access Token: " + accessToken);

                        // JavaScript code to be executed, items should be dynamic based on input.
                        string mainScriptTemplate = @"
                           const myHeaders = new Headers();
                            myHeaders.append('Accept', 'application/json, text/plain, */*');
                            myHeaders.append('Accept-Language', 'en-US,en;q=0.9');
                            myHeaders.append('Authorization', 'Bearer {accessToken}');
                            myHeaders.append('Content-Type', 'application/json');
                            myHeaders.append('Origin', 'https://kream.co.kr');
                            myHeaders.append('Priority', 'u=1, i');
                            myHeaders.append('Referer', 'https://kream.co.kr/');
                            myHeaders.append('Sec-CH-UA', '\""Chromium\"";v=\""128\"", \""Not;A=Brand\"";v=\""24\"", \""Google Chrome\"";v=\""128\""');
                            myHeaders.append('Sec-CH-UA-Mobile', '?0');
                            myHeaders.append('Sec-CH-UA-Platform', '\""Windows\""');
                            myHeaders.append('Sec-Fetch-Dest', 'empty');
                            myHeaders.append('Sec-Fetch-Mode', 'cors');
                            myHeaders.append('Sec-Fetch-Site', 'same-site');
                            myHeaders.append('User-Agent', 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/128.0.0.0 Safari/537.36');
                            myHeaders.append('X-Kream-Api-Version', '35');
                            myHeaders.append('X-Kream-Client-Datetime', '20240906053132+1000');
                            myHeaders.append('X-Kream-Device-Id', 'web;93e8ccf2-b7a8-4ede-b5ed-555881027c86');
                            myHeaders.append('X-Kream-Web-Build-Version', '6.0.1');
                            myHeaders.append('X-Kream-Web-Request-Secret', 'kream-djscjsghdkd');

                           const raw = JSON.stringify({
                            items: [
                                {
                                    option: '20년 상반기-개런티카드+박스',
                                    product_option: {
                                        id: 2414642,
                                        product_id: 43348,
                                        key: '20년 상반기-개런티카드+박스',
                                        name: '20년 상반기-개런티카드+박스',
                                        name_display: '20년 상반기-개런티카드+박스',
                                        display_order: 19
                                    },
                                    max_qty: 1000,
                                    quantity: 1,
                                    product_id: 43348
                                }
                            ]
                        });

                            const requestOptions = {
                                method: 'POST',
                                headers: myHeaders,
                                body: raw,
                                redirect: 'follow',
                                mode: 'cors'
                            };

                            return (async () => {
                            try {
                                const response = await fetch('https://api.kream.co.kr/api/seller/inventory/stock_request/review?request_key=dbad9a8d-6990-4cb0-9470-7c07984e628b', requestOptions);
                                const result = await response.text();
            
                                console.log(result);
                                return result;  // Return result to C#
                            } catch (error) {
                                console.error('Error:', error);
                                return 'Error: ' + error;
                            }
                        })();

                        ";
                        string mainScript = mainScriptTemplate.Replace("{accessToken}", accessToken);

                        MessageBox.Show(mainScript);

                        try { 
                            var main_result = js.ExecuteScript(mainScript);

                            string a = main_result.ToString();
                            using (JsonDocument b = JsonDocument.Parse(a))
                            {
                                MessageBox.Show("here : ");
                                JsonElement c = b.RootElement;


                                if (c.TryGetProperty("total_amount", out JsonElement d))
                                {

                                    var res = d;

                                    MessageBox.Show("price : " + res.ToString());
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error: " + ex.Message);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

            driver.Navigate().GoToUrl(inventoryUrl);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Close the browser when the form is closed
            driver.Quit();
            base.OnFormClosing(e);
        }
    }
}
