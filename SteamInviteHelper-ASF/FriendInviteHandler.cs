using ArchiSteamFarm;
using SteamKit2;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    class FriendInviteHandler : ClientMsgHandler
    {
        public async Task<bool> processFriendRequest(SteamID SteamID, Bot bot)
        {
            SteamFriends steamFriends = Client.GetHandler<SteamFriends>();
            UserProfile userProfile = await UserProfile.BuildUserProfile(SteamID.ConvertToUInt64(), bot);
            Logger.LogDebug("[PROFILE DETAILS]: " + userProfile.ToString());

            List<Action> actions = new List<Action>();

            actions.Add(processPrivateProfile(userProfile, bot));
            Logger.LogDebug("[ACTION PRIVATE PROFILE]: " + processPrivateProfile(userProfile, bot).action);

            actions.Add(await processSteamRepScammerAsync(userProfile, bot));
            Logger.LogDebug("[ACTION STAEMREP SCAMMER]: " + (await processSteamRepScammerAsync(userProfile, bot)).action);

            actions.Add(processSteamLevel(userProfile, bot));
            Logger.LogDebug("[ACTION STEAM LEVEL]: " + processSteamLevel(userProfile, bot).action);

            actions.Add(processVACBanned(userProfile, bot));
            Logger.LogDebug("[ACTION VAC BANNED]: " + processVACBanned(userProfile, bot).action);

            actions.Add(processGameBanned(userProfile, bot));
            Logger.LogDebug("[ACTION GAME BANNED]: " + processGameBanned(userProfile, bot).action);

            actions.Add(processDaysSinceLastBan(userProfile, bot));
            Logger.LogDebug("[ACTION DAYS SINCE LAST BAN]: " + processDaysSinceLastBan(userProfile, bot).action);

            actions.Add(processCommunityBanned(userProfile, bot));
            Logger.LogDebug("[ACTION COMMUNITY BANNED]: " + processCommunityBanned(userProfile, bot).action);

            actions.Add(processEconomyBanned(userProfile, bot));
            Logger.LogDebug("[ACTION ECONOMY BANNED]: " + processEconomyBanned(userProfile, bot).action);

            actions.Add(processProfileName(userProfile, bot));
            Logger.LogDebug("[ACTION PROFILE NAME]: " + processProfileName(userProfile, bot).action);

            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            List<string> actionpriority = config.ActionPriority;

            foreach (string action_string in actionpriority)
            {
                Action action = new Action(action_string);
                if (actions.Contains(action))
                {
                    action = actions[actions.IndexOf(action)];

                    switch (action.action)
                    {
                        case "block":
                            await steamFriends.IgnoreFriend(SteamID);
                            break;
                        case "ignore":
                            steamFriends.RemoveFriend(SteamID);
                            break;
                        case "add":
                            steamFriends.AddFriend(SteamID);
                            break;
                    }

                    Logger.LogInfo("New pending invite from {0}", userProfile.personaName);
                    Logger.LogInfo("  ├─ SteamID: {0}", Convert.ToString(SteamID.ConvertToUInt64()));
                    Logger.LogInfo("  ├─ Profile url: {0}", userProfile.profileUrl);
                    Logger.LogInfo("  └─ Action: {0} | Reason: {1}", action.action.ToUpper(), action.reason);
                }
            }
            return true;
        }

        private static Action processPrivateProfile(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            if (userProfile.communityVisibilityState == 1)
            {
                return new Action(config.PrivateProfile, "Private Profile");
            }
            else
            {
                return new Action("none");
            }
        }

        private static async Task<Action> processSteamRepScammerAsync(UserProfile userProfile, Bot bot)
        {
            WebBrowser wb = bot.ArchiWebHandler.WebBrowser;
            string url = "http://steamrep.com/id2rep.php?steamID32=" + new SteamID(userProfile.steamId64).Render();
            string result = (await wb.UrlGetToHtmlDocument(url)).Content.Text;

            if (result.Contains("SCAMMER"))
            {
                Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
                return new Action(config.SteamRepScammer, "SteamRep scammer");
            }
            return new Action("none");
        }

        private static Action processSteamLevel(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            string defaultAction = "none";

            foreach (ConfigItem item in config.SteamLevel)
            {
                int value = Convert.ToInt32(item.value);
                switch (item.condition)
                {
                    case "less_than":
                        if (userProfile.steamLevel < value)
                            return new Action(item.action, "Steam level < " + value);
                        break;
                    case "more_than":
                        if (userProfile.steamLevel > value)
                            return new Action(item.action, "Steam level > " + value);
                        break;
                    case "equal":
                        if (userProfile.steamLevel == value)
                            return new Action(item.action, "Steam level = " + value);
                        break;
                    case "default":
                        defaultAction = item.action;
                        break;
                }
            }
            return new Action(defaultAction);
        }

        private static Action processVACBanned(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            string defaultAction = "none";

            foreach (ConfigItem item in config.VacBanned)
            {
                int value = Convert.ToInt32(item.value);
                switch (item.condition)
                {
                    case "less_than":
                        if (userProfile.numberOfVACBans < value)
                            return new Action(item.action, "Number of VAC bans < " + value);
                        break;
                    case "more_than":
                        if (userProfile.numberOfVACBans > value)
                            return new Action(item.action, "Number of VAC bans > " + value);
                        break;
                    case "equal":
                        if (userProfile.numberOfVACBans == value)
                            return new Action(item.action, "Number of VAC bans = " + value);
                        break;
                    case "default":
                        defaultAction = item.action;
                        break;
                }
            }
            return new Action(defaultAction);
        }

        private static Action processGameBanned(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            string defaultAction = "none";

            foreach (ConfigItem item in config.GameBanned)
            {
                int value = Convert.ToInt32(item.value);
                switch (item.condition)
                {
                    case "less_than":
                        if (userProfile.numberOfGamebans < value)
                            return new Action(item.action, "Number of game bans < " + value);
                        break;
                    case "more_than":
                        if (userProfile.numberOfGamebans > value)
                            return new Action(item.action, "Number of game bans > " + value);
                        break;
                    case "equal":
                        if (userProfile.numberOfGamebans == value)
                            return new Action(item.action, "Number of game bans = " + value);
                        break;
                    case "default":
                        defaultAction = item.action;
                        break;
                }
            }
            return new Action(defaultAction);
        }

        private static Action processDaysSinceLastBan(UserProfile userProfile, Bot bot)
        {
            if (userProfile.vacBanned || userProfile.gameBanned)
            {
                Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
                string defaultAction = "none";

                foreach (ConfigItem item in config.DaysSinceLastBan)
                {
                    int value = Convert.ToInt32(item.value);
                    switch (item.condition)
                    {
                        case "less_than":
                            if (userProfile.daysSinceLastBan < value)
                                return new Action(item.action, "Days since last ban < " + value);
                            break;
                        case "more_than":
                            if (userProfile.daysSinceLastBan > value)
                                return new Action(item.action, "Days since last ban > " + value);
                            break;
                        case "equal":
                            if (userProfile.daysSinceLastBan == value)
                                return new Action(item.action, "Days since last ban = " + value);
                            break;
                        case "default":
                            defaultAction = item.action;
                            break;
                    }
                }
                return new Action(defaultAction);
            }
            else
            {
                return new Action("none");
            }
        }

        private static Action processCommunityBanned(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            if (userProfile.communityBanned)
            {
                return new Action(config.CommunityBanned, "Community banned");
            }
            else
            {
                return new Action("none");
            }
        }

        private static Action processEconomyBanned(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            if (!userProfile.economyBan.Equals("none"))
            {
                return new Action(config.EconomyBanned, "Economy banned");
            }
            else
            {
                return new Action("none");
            }
        }

        private static Action processProfileName(UserProfile userProfile, Bot bot)
        {
            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            string defaultAction = "none";

            foreach (ConfigItem item in config.ProfileName)
            {
                switch (item.condition)
                {
                    case "equal":
                        if (userProfile.personaName.Equals(item.value, StringComparison.OrdinalIgnoreCase))
                            return new Action(item.action, "Profile name equals " + item.value);
                        break;
                    case "contain":
                        if (userProfile.personaName.Contains(item.value, StringComparison.OrdinalIgnoreCase))
                            return new Action(item.action, "Profile name contains " + item.value);
                        break;
                    case "default":
                        defaultAction = item.action;
                        break;
                }
            }
            return new Action(defaultAction);
        }

        private static async Task<Action> processCommentedOnProfile(UserProfile userProfile, Bot bot)
        {
            if (!SteamInviteHelper.BotProfiles.TryGetValue(bot, out UserProfile botProfile))
            {
                botProfile = SteamInviteHelper.BotProfiles.GetOrAdd(bot, await UserProfile.BuildUserProfile(bot.SteamID, bot));
            }

            WebBrowser webBrowser = bot.ArchiWebHandler.WebBrowser;
            string requesturl = botProfile.profileUrl +"/allcomments";
            string html = (await webBrowser.UrlGetToHtmlDocument(requesturl)).Content.Text;
            bool commented = html.Contains(Convert.ToString(userProfile.steamId64));

            if (commented)
            {

            }
            return html.Contains(Convert.ToString(userProfile.steamId64));

        }

        public override void HandleMsg(IPacketMsg packetMsg)
        {
        }
    }
}
