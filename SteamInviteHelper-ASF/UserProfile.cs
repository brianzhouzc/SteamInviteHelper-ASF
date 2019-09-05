using ArchiSteamFarm;
using SteamKit2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    class UserProfile
    {
        public static ConcurrentDictionary<Bot, string> WebAPIKeys = new ConcurrentDictionary<Bot, string>();

        public ulong steamId64 { get; set; }

        public string profileUrl { get; set; }

        //Profile visibility. 1 - Private, 3 - Public
        public int communityVisibilityState { get; set; }

        //If set, indicates the user has a community profile configured (will be set to '1')
        public int profileState { get; set; }

        public string personaName { get; set; }

        public int steamLevel { get; set; }

        //If set, indicates the profile allows public comments.
        public int commentPermission { get; set; }

        //The user's current status. 0 - Offline, 1 - Online, 2 - Busy, 3 - Away, 4 - Snooze, 5 - looking to trade, 6 - looking to play. 
        //If the player's profile is private, this will always be "0", except if the user has set their status to looking to trade or looking to play, because a bug makes those status appear even if the profile is private.
        public int personaState { get; set; }

        public bool communityBanned { get; set; }

        public bool vacBanned { get; set; }

        public int numberOfVACBans { get; set; }

        public bool gameBanned { get; set; }

        public int numberOfGamebans { get; set; }

        public int daysSinceLastBan { get; set; }

        public string economyBan { get; set; }

        public string userProfileURL { get; set; }

        public UserProfile(ulong steamId64, Bot bot)
        {
            this.steamId64 = steamId64;

            string apikey = WebAPIKeys.GetOrAdd(bot, "");
            using (dynamic steamUser = WebAPI.GetInterface("ISteamUser", apikey))
            {
                KeyValue kvSummaries = steamUser.GetPlayerSummaries(steamids: steamId64.ToString());
                var userSummaries = kvSummaries["players"]["player"].Children[0];

                profileUrl = userSummaries["profileurl"].AsString();
                communityVisibilityState = userSummaries["communityvisibilitystate"].AsInteger();
                profileState = userSummaries["profilestate"].AsInteger();
                personaName = userSummaries["personaname"].AsString();
                commentPermission = userSummaries["commentpermission"].AsInteger();
                personaState = userSummaries["personaState"].AsInteger();
                userProfileURL = userSummaries["profileurl"].AsString();

                KeyValue kvBans = steamUser.GetPlayerBans(steamids: steamId64.ToString());
                var userBans = kvBans["players"].Children[0];

                communityBanned = userBans["CommunityBanned"].AsBoolean();
                vacBanned = userBans["VACBanned"].AsBoolean();
                numberOfVACBans = userBans["NumberOfVACBans"].AsInteger();
                numberOfGamebans = userBans["NumberOfGameBans"].AsInteger();
                gameBanned = numberOfGamebans > 0;
                daysSinceLastBan = userBans["DaysSinceLastBan"].AsInteger();
                economyBan = userBans["EconomyBan"].AsString();
            }

            using (dynamic steamUser = WebAPI.GetInterface("IPlayerService", apikey))
            {
                KeyValue kvServices = steamUser.GetSteamLevel(steamid: steamId64.ToString());
                steamLevel = kvServices["player_level"].AsInteger();
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
