using Microsoft.Playwright;
using System.Diagnostics;

namespace AutoGimkit
{
    internal static class Program
    {
        public static bool isRunning = false;
        private static CancellationTokenSource? cts;
        private static bool browserChecked;

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
            form.homeScreen.UpdateMaxMistakes(dataSaving.GetIncorrectMax());

            Application.Run(form);

            SaveIfNotDirty();
        }

        public static async Task EnsureBrowserInstalled()
        {
            if (browserChecked) return;

            // fully qualified, because your own class is also called Program
            Task<int> install = Task.Run(() =>
                Microsoft.Playwright.Program.Main(new[] { "install", "chromium" }));

            // still running after 400 ms means it's really downloading
            if (await Task.WhenAny(install, Task.Delay(400)) != install && form != null)
            {
                using var loading = new LoadingForm(
                    "Downloading the browser (first run only).\nThis can take a minute...");

                form.Enabled = false;   // stop clicks on the main window meanwhile
                loading.Show(form);
                try
                {
                    await install;
                }
                finally
                {
                    form.Enabled = true;
                    loading.Close();
                }
            }

            int exitCode = await install;
            if (exitCode != 0)
                throw new Exception($"Browser install failed (exit code {exitCode}). Check your internet connection.");

            browserChecked = true;
        }
        private static void SaveIfNotDirty()
        {
            if (dataSaving != null && dataSaving.IsDirty())
            {
                dataSaving.WriteData();
            }
        }

        public static async Task<bool> AttemptDownload()
        {
            if (wordDb != null)
            {
                var (s, r) = await WordDownloader.GetRawJson();
                if (s == true && r != null)
                    wordDb.LoadFromPastebin(r);

                return false;
            }     
            else
                await Task.CompletedTask;

            return false;
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
            SaveIfNotDirty();

            int totalRandomMistakes = 0;

            DataSaving? ds = dataSaving;
            if (ds != null)
            {
                totalRandomMistakes = ds.GetIncorrectMax();
            }
            form?.mainScreen.UpdateMistakesText(0, totalRandomMistakes);

            if (cts != null)
                return;
            cts = new CancellationTokenSource();
            try
            {
                await RunLoopAsync(cts.Token, totalRandomMistakes);
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

        private static async Task Delay(int ms, float multi, CancellationToken token, float influence = 1.0f)
        {

            await Task.Delay((int)(ms * multi * influence), token);
        }
        private static async Task RunLoopAsync(CancellationToken token, int totalMistakes)
        {
            int currentMistakes = 0;
            float speedMulti = 1.0f;

            DataSaving? ds = Program.dataSaving;
            if (ds != null)
            {
                speedMulti = ds.GetSpeedMultiplier();
            }

            Debug.WriteLine($"Starting program with speed multi {speedMulti}");

            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (page == null)
                    {
                        await Delay(50, speedMulti, token);
                        continue;
                    }

                    string lastClickedAtStart = lastClickedAnswer;

                    //Main loops
                    string latinWord = await page.Locator(".sc-gSifMm").InnerTextAsync();

                    if (latinWord == lastHandledWord)
                    {
                        await Delay(50, speedMulti, token);
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
                            //Dont need speedMulti here
                            await Delay(30, 1, token);
                        }
                        Debug.WriteLine($"Got correct Answer: {lastClickedAnswer}");
                        wordDb?.Add(latinWord, lastClickedAnswer);
                        await Delay(100, speedMulti, token);
                        continue;
                    }
                    //found word in database
                    form?.mainScreen.SetCurrentDef(key.English);

                    string target = key.English.Trim().ToLowerInvariant();
                    ILocator answers = page.Locator("div.sc-ixcdjX.kLONFv");


                    float chance = .1f + (currentMistakes / totalMistakes) * .5f;

                    bool shouldBeWrong = (currentMistakes < totalMistakes && Random.Shared.NextSingle() < chance);

                    Debug.WriteLine($"Should be wrong {shouldBeWrong}");

                    int index;
                    index = await answers.EvaluateAllAsync<int>(@"
(divs, target) => divs.findIndex(div =>
    [...div.querySelectorAll('span.notranslate')].some(span =>
        span.innerText.trim().toLowerCase() === target))",
                    target);


                    if (index < 0)
                    {
                        //Does not need speedMulti
                        await Delay(50, 1, token);
                        continue;
                    }

                    if (shouldBeWrong)
                    {
                        index += 1;
                        if (index > 3)
                        {
                            index = 0;
                        }
                    }


                    ILocator answer = answers.Nth(index);
                    string t = await answer.InnerTextAsync();

                    answer?.ClickAsync();


                    if (shouldBeWrong)
                    {
                        currentMistakes++;
                        form?.mainScreen.UpdateMistakesText(currentMistakes, totalMistakes);
                    }

                    //Wait to click continue


                    //Does not work immediatly
                    //bool isIncorrect = await page.Locator(".sc-fcSHUR.dFnPXq").CountAsync() > 0;

                    //do not add influence 
                    await Delay(300, speedMulti, token);

                    ILocator bg = page.Locator(".sc-iVnIWt.gOiICl.maxAll");

                    bool foundBg = true;

                    try
                    {
                        await bg.WaitForAsync(new() { Timeout = 100 });
                    }
                    catch (TimeoutException)
                    {
                        //nothing
                        foundBg = false;
                    }

                    int count = await bg.CountAsync();
                    Debug.WriteLine($"count: {count}, found: {foundBg}");



                    // OLD LOGIC OF CALCULATING BACKGROUND COLOR
                    // BACKGROUND DOES NOT EXIST UNLESS ANSWER IS WRONG
                    //string color = await bg.EvaluateAsync<string>("e => getComputedStyle(e).backgroundColor");
                    //Debug.WriteLine($"Color: {color}"); //{color[0]}");

                    //string[] parts = color
                    //.Replace("rgba(", "")
                    //.Replace(")", "")
                    //.Split(',');

                    //int r = int.Parse(parts[0].Trim());
                    //int g = int.Parse(parts[1].Trim());
                    //int b = int.Parse(parts[2].Trim());

                    bool isIncorrect = foundBg;

                    //if (r > g && r > b)
                    //{
                    //    isIncorrect = true;
                    //}

                    Debug.WriteLine($"Is incorrect: {isIncorrect}");
                    if (isIncorrect)
                    {
                        await Delay(3000 + (int)(1000 * speedMulti), 1, token);
                        Debug.WriteLine("Pressing continue");
                        ILocator continueButton = page.Locator(".sc-erPUmh.fUXOFu.maxAll .sc-goiVcJ.hKIzUO");
                        await continueButton.ClickAsync();
                        Debug.WriteLine("Clicked");
                    }
                    else
                    {
                        await Delay(18, speedMulti, token);
                        Debug.WriteLine("Pressing continue");
                        await page.Locator("div.sc-eEHciv.hkdshG").ClickAsync();
                    }
                }
                catch (PlaywrightException e)
                {
                    Debug.WriteLine(e.Message);
                }

                await Delay(10, speedMulti, token);
            }
        }

        private static void OnAnswerClicked(string text)
        {
            lastClickedAnswer = text;
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

            await EnsureBrowserInstalled();
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