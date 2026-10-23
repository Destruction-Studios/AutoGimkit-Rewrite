using Microsoft.Playwright;
using System.Diagnostics;

namespace AutoGimkit
{
    internal static class Program
    {
        public static bool isRunning = false;
        private static CancellationTokenSource? cts;

        public const string SAVE_FILE = "data.json";
        public static DataSaving? dataSaving { get; private set; }

        private static Form1? form;
        private static WordDb? wordDb;
        private static IPlaywright? playwright;
        private static IBrowserContext? context;
        private static IPage? page;

        private static string lastClickedAnswer = string.Empty;
        private static string lastHandledWord = string.Empty;

        [STAThread]
        static void Main()
        {
            dataSaving = new DataSaving(SAVE_FILE);
            dataSaving.LoadData();

            wordDb = new WordDb();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();
            form = new Form1();

            form.FormClosed += async (s, e) => await Program.CloseBrowser();

            //Sync UI with data
            form.homeScreen.SetGimkitIdText(dataSaving.GetGimkitId());
            form.homeScreen.SetSpeedMultiplierValue(dataSaving.GetSpeedMultiplier());
            form.homeScreen.UpdateMinAndMax(dataSaving.GetIncorrectMin(), dataSaving.GetIncorrectMax());

            Application.Run(form);

            if (dataSaving != null && dataSaving.IsDirty())
                dataSaving.WriteData();
        }

        public static async Task OpenChrome()
        {
            if (dataSaving != null && !dataSaving.IsDirty())
            {
                Console.WriteLine("Saving data");
                dataSaving.WriteData();
            }

            await CreateBrowserAsync();
        }

        public static async Task StartProgram()
        {
            int totalRandomMistakes = 0;
            int currentRandomMistakes = 0;
            DataSaving? ds = dataSaving;
            if (ds != null)
            {
                totalRandomMistakes = Random.Shared.Next(ds.GetIncorrectMin(), ds.GetIncorrectMax() + 1);
            }
            form?.mainScreen.UpdateMistakesText(currentRandomMistakes, totalRandomMistakes);

            if (cts != null)
                return;
            cts = new CancellationTokenSource();
            try
            {
                await RunLoopAsync(cts.Token);
            }
            catch (OperationCanceledException)
            {
                //its ok
            }
            finally
            {
                cts.Dispose();
                cts = null;
            }

            await Task.CompletedTask;
        }

        private static async Task RunLoopAsync(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (page == null)
                    {
                        await Task.Delay(50, token);
                        continue;
                    }
                    string lastClickedAtStart = lastClickedAnswer;

                    //Main loops
                    string latinWord = await page.Locator(".sc-gSifMm").InnerTextAsync();

                    if (latinWord == lastHandledWord)
                    {
                        await Task.Delay(50, token);
                        continue;
                    }
                    lastHandledWord = latinWord;

                    form?.mainScreen.SetCurrentLatin(latinWord);

                    WordEntry? key = wordDb?.Get(latinWord);

                    Debug.WriteLine(key);

                    if (key == null)
                    {
                        //Detecting the correct answer

                        form?.mainScreen.SetCurrentDef("Click the correct answer");
                        while (lastClickedAnswer == lastClickedAtStart)
                        {
                            await Task.Delay(30, token);
                        }
                        Debug.WriteLine($"Got correct Answer: {lastClickedAnswer}");
                        wordDb?.Add(latinWord, lastClickedAnswer);
                        await Task.Delay(100, token);
                        continue;
                    }
                    //found word in database
                    form?.mainScreen.SetCurrentDef(key.English);

                    string target = key.English.Trim().ToLowerInvariant();
                    ILocator answers = page.Locator("div.sc-ixcdjX.kLONFv");

                    bool shouldBeWrong = true;

                    int index;
                    index = await answers.EvaluateAllAsync<int>(@"
(divs, target) => divs.findIndex(div =>
    [...div.querySelectorAll('span.notranslate')].some(span =>
        span.innerText.trim().toLowerCase() === target))",
                    target);


                    if (index < 0)
                    {
                        await Task.Delay(50, token);
                        continue;
                    }

                    //if (shouldBeWrong)
                    //{
                    //    index = index == 3 ? 0 : index + 1;
                    //}

                    Debug.WriteLine(index);

                    ILocator answer = answers.Nth(index);
                    string t = await answer.InnerTextAsync();
                    Debug.WriteLine($"Found: {t}");

                    answer?.ClickAsync();

                    //Wait to click continue
                    await Task.Delay(18, token);

                    await page.Locator("div.sc-eEHciv.hkdshG").ClickAsync();

                }
                catch (PlaywrightException e)
                {
                    Debug.WriteLine(e.Message);
                }

                await Task.Delay(10, token);
            }
        }

        private static void OnAnswerClicked(string text)
        {
            lastClickedAnswer = text;
            Debug.WriteLine($"CLcked: {text}");
        }

        public static void StopProgram() => cts?.Cancel();

        public static async Task CloseBrowser()
        {
            if (context != null) await context.CloseAsync();
            playwright?.Dispose();
        }

        public static async Task CreateBrowserAsync()
        {
            if (context != null)
                return;

            string profileDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AutoGimkit",
                "BrowserProfile"
                );

            playwright = await Playwright.CreateAsync();
            context = await playwright.Chromium.LaunchPersistentContextAsync(profileDir, new()
            {
                Headless = false
            });

            await context.ExposeFunctionAsync<string>("onAnswerClicked", OnAnswerClicked);

            await context.AddInitScriptAsync(@"
document.addEventListener('click', e => {
    const el = e.target.closest('.sc-ixcdjX.kLONFv');
    if (!el) return;
    const span = el.querySelector('span.notranslate');
    const word = document.querySelector('.sc-itBLYH.botDlG span.lang-en');
    window.onAnswerClicked((span || el).innerText.trim());
}, true);
");


            page = await context.NewPageAsync();

            await page.GotoAsync($"https://gimkit.com/join/{dataSaving?.GetGimkitId()}");
        }
    }
}