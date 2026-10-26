using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoGimkit
{
    internal class WordDownloader
    {
        private  const string PASTEBIN_URL = "https://pastebin.com/raw/Bva3nZbw";
        private static readonly HttpClient httpClient = new HttpClient();


        public static async Task<(bool, string?)> GetRawJson()
        {
            try
            {
                string content = await httpClient.GetStringAsync(PASTEBIN_URL);
                Debug.WriteLine(content);
               return (true, content);
            }
            catch (HttpRequestException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return (false, null);
        }
    }
}
