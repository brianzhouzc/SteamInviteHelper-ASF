using ArchiSteamFarm;
using SteamKit2;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    class UserProfile
    {
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

        private UserProfile(){}
        async public static Task<UserProfile> BuildUserProfile(ulong steamId64, Bot bot)
        {
            UserProfile userProfile = new UserProfile();

            (bool success, string steamApiKey) = await bot.ArchiWebHandler.CachedApiKey.GetValue().ConfigureAwait(false);

            if (!success)
                return null;

            userProfile.steamId64 = steamId64;

            using (dynamic steamUser = WebAPI.GetInterface("ISteamUser", steamApiKey))
            {
                KeyValue kvUserSummaries = steamUser.GetPlayerSummaries(steamids: steamId64.ToString())["players"]["player"].Children[0];
                userProfile.profileUrl = kvUserSummaries["profileurl"].AsString();
                userProfile.communityVisibilityState = kvUserSummaries["communityvisibilitystate"].AsInteger();
                userProfile.profileState = kvUserSummaries["profilestate"].AsInteger();
                userProfile.personaName = kvUserSummaries["personaname"].AsString();
                userProfile.commentPermission = kvUserSummaries["commentpermission"].AsInteger();
                userProfile.personaState = kvUserSummaries["personaState"].AsInteger();
                userProfile.userProfileURL = kvUserSummaries["profileurl"].AsString();

                KeyValue kvUserBans = steamUser.GetPlayerBans(steamids: steamId64.ToString())["players"].Children[0];
                userProfile.communityBanned = kvUserBans["CommunityBanned"].AsBoolean();
                userProfile.vacBanned = kvUserBans["VACBanned"].AsBoolean();
                userProfile.numberOfVACBans = kvUserBans["NumberOfVACBans"].AsInteger();
                userProfile.numberOfGamebans = kvUserBans["NumberOfGameBans"].AsInteger();
                userProfile.gameBanned = userProfile.numberOfGamebans > 0;
                userProfile.daysSinceLastBan = kvUserBans["DaysSinceLastBan"].AsInteger();
                userProfile.economyBan = kvUserBans["EconomyBan"].AsString();
            }

            using (dynamic steamUser = WebAPI.GetInterface("IPlayerService", steamApiKey))
            {
                KeyValue kvPlayerServices = steamUser.GetSteamLevel(steamid: steamId64.ToString());
                userProfile.steamLevel = kvPlayerServices["player_level"].AsInteger();
            }

            return userProfile;
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
