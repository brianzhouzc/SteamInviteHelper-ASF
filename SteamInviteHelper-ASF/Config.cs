using ArchiSteamFarm;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using System.Threading;

namespace SteamInviteHelper_ASF
{
    class Config
    {
        public static ConcurrentDictionary<Bot, Config> FriendInviteConfigs = new ConcurrentDictionary<Bot, Config>();

        public List<string> ActionPriority { get; set; }

        public string PrivateProfile { get; set; }
        public string SteamRepScammer { get; set; }
        public List<ConfigItem> SteamLevel { get; set; }
        public List<ConfigItem> VacBanned { get; set; }
        public List<ConfigItem> GameBanned { get; set; }
        public List<ConfigItem> DaysSinceLastBan { get; set; }
        public string CommunityBanned { get; set; }
        public string EconomyBanned { get; set; }
        public List<ConfigItem> ProfileName { get; set; }

        public bool Debug { get; set; }


        public Config(Bot bot)
        {
            string configpath= "./config/" + bot.BotName + ".json";
            string json = File.ReadAllText(configpath);

            try
            {
                JObject o = JObject.Parse(json);
                
                if (!o.ContainsKey("SteamInviteHelper"))
                {
                    JObject defaultconfig = JObject.Parse(@"{""SteamInviteHelper"":{""ActionPriority"":[""block"",""ignore"",""add"",""none""],""PrivateProfile"":{""action"":""block""},""SteamRepScammer"":{""action"":""block""},""SteamLevel"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""less_than"",""value"":1,""action"":""block""},{""condition"":""less_than"",""value"":5,""action"":""ignore""}],""VACBanned"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""more_than"",""value"":1,""action"":""ignore""}],""GameBanned"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""more_than"",""value"":1,""action"":""ignore""}],""DaysSinceLastBan"":[{""condition"":""default"",""value"":-1,""action"":""none""},{""condition"":""less_than"",""value"":90,""action"":""ignore""}],""CommunityBanned"":{""action"":""none""},""EconomyBanned"":{""action"":""none""},""ProfileName"":[{""condition"":""default"",""value"":"""",""action"":""none""},{""condition"":""contain"",""value"":""shittygamblingsite.com"",""action"":""ignore""}]}}");
                    o.Merge(defaultconfig);

                    File.WriteAllText(configpath, o.ToString());
                    Logger.LogWarning("Config not found! Loading default config...");
                    Logger.LogWarning("Saved default config, please review and edit your bot's config!");
                }


                this.ActionPriority = JsonConvert.DeserializeObject<List<string>>(o.SelectToken("SteamInviteHelper.ActionPriority").ToString());

                this.PrivateProfile = (string)o.SelectToken("SteamInviteHelper.PrivateProfile.action");
                this.SteamRepScammer = (string)o.SelectToken("SteamInviteHelper.SteamRepScammer.action");

                this.SteamLevel = new List<ConfigItem>();
                IEnumerable<JToken> steamleveltokens = o.SelectTokens("SteamInviteHelper.SteamLevel[*]");
                foreach (JToken token in steamleveltokens)
                {
                    SteamLevel.Add(JsonConvert.DeserializeObject<ConfigItem>(token.ToString()));
                }
                //Console.WriteLine(SteamLevel[0].ToString());

                this.VacBanned = new List<ConfigItem>();
                IEnumerable<JToken> vacbannedtockens = o.SelectTokens("SteamInviteHelper.VACBanned[*]");
                foreach (JToken token in vacbannedtockens)
                {
                    VacBanned.Add(JsonConvert.DeserializeObject<ConfigItem>(token.ToString()));
                }

                this.GameBanned = new List<ConfigItem>();
                IEnumerable<JToken> gamebannedtokens = o.SelectTokens("SteamInviteHelper.GameBanned[*]");
                foreach (JToken token in gamebannedtokens)
                {
                    GameBanned.Add(JsonConvert.DeserializeObject<ConfigItem>(token.ToString()));
                }

                this.DaysSinceLastBan = new List<ConfigItem>();
                IEnumerable<JToken> dayssincelastbantokens = o.SelectTokens("SteamInviteHelper.DaysSinceLastBan[*]");
                foreach (JToken token in dayssincelastbantokens)
                {
                    DaysSinceLastBan.Add(JsonConvert.DeserializeObject<ConfigItem>(token.ToString()));
                }

                this.CommunityBanned = (string)o.SelectToken("SteamInviteHelper.CommunityBanned.action");

                this.EconomyBanned = (string)o.SelectToken("SteamInviteHelper.EconomyBanned.action");

                this.ProfileName = new List<ConfigItem>();
                IEnumerable<JToken> profilenametokens = o.SelectTokens("SteamInviteHelper.ProfileName[*]");
                foreach (JToken token in profilenametokens)
                {
                    ProfileName.Add(JsonConvert.DeserializeObject<ConfigItem>(token.ToString()));
                }

                try
                {
                    this.Debug = (Boolean)o.SelectToken("SteamInviteHelper.debug");
                }
                catch
                {
                    this.Debug = false;
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning("Error when loading config file");
                Logger.LogWarning("Exception: " + e.Message);
                Logger.LogWarning("Exiting in 5 seconds...");
                Thread.Sleep(5000);
                Environment.Exit(1);
            }

        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");
            foreach (var property in this.GetType().GetProperties())
            {
                sb.AppendFormat("\"{0}\": \"{1}\", ", property.Name, property.GetValue(this, null));
            }
            sb.Length -= 2;
            sb.Append("}");
            return sb.ToString();
        }
    }
}
