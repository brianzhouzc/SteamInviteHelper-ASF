using ArchiSteamFarm;
using SteamKit2;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    class WebRequestsHelper
    {
        private static readonly HttpClient client = new HttpClient();

        public static async Task<bool> StreamRepIsScammer(ulong steam64ID)
        {
            string url = "http://steamrep.com/id2rep.php?steamID32=" + new SteamID(steam64ID).Render();
            string result = await client.GetStringAsync(url);

            return result.Contains("SCAMMER");
        }
    }
}
