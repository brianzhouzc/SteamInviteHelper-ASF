using ArchiSteamFarm;
using System;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    class WebRequestsHelper
    {
        private static string SteamCommunityURL => ArchiWebHandler.SteamCommunityURL;
        private static readonly HttpClient client = new HttpClient();

        public static async void getApiKeyAsync(Bot bot)
        {
            const string request = "/dev/apikey";
            var html = await bot.ArchiWebHandler.UrlGetToHtmlDocumentWithSession(SteamCommunityURL, request);
            string key = html.GetElementbyId("bodyContents_ex").ChildNodes[3].InnerText.Replace("Key: ", "");
            UserProfile.WebAPIKeys.GetOrAdd(bot, key);
        }

        public static async Task<bool> StreamRepIsScammer(ulong steam64ID)
        {
            string url = "http://steamrep.com/id2rep.php?steamID32=" + Steam64ToSteam32(steam64ID);
            string result = await client.GetStringAsync(url);

            return result.Contains("SCAMMER");
        }

        private static string Steam64ToSteam32(ulong communityId)
        {
            if (communityId < 76561197960265729L || !Regex.IsMatch(communityId.ToString((IFormatProvider)CultureInfo.InvariantCulture), "^7656119([0-9]{10})$"))
                return string.Empty;
            communityId -= 76561197960265728L;
            ulong num = communityId % 2L;
            communityId -= num;
            string input = string.Format("STEAM_0:{0}:{1}", num, (communityId / 2L));
            if (!Regex.IsMatch(input, "^STEAM_0:[0-1]:([0-9]{1,10})$"))
            {
                return string.Empty;
            }
            return input;
        }
    }
}
