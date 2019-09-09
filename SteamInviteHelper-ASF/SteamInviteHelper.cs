using ArchiSteamFarm;
using ArchiSteamFarm.Plugins;
using JetBrains.Annotations;
using Newtonsoft.Json.Linq;
using SteamKit2;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

namespace SteamInviteHelper_ASF
{
    [Export(typeof(IPlugin))]
    internal sealed class SteamInviteHelper : IASF, IBot, IBotConnection, IBotFriendRequest, IBotModules, IBotSteamClient
    {
        private static ConcurrentDictionary<Bot, FriendInviteHandler> FriendInviteHandlers = new ConcurrentDictionary<Bot, FriendInviteHandler>();
        public static ConcurrentDictionary<Bot, UserProfile> BotProfiles = new ConcurrentDictionary<Bot, UserProfile>();

        public string Name => nameof(SteamInviteHelper);

        public Version Version => typeof(SteamInviteHelper).Assembly.GetName().Version;

        public void OnASFInit(IReadOnlyDictionary<string, JToken> additionalConfigProperties = null) { }

        public void OnBotDestroy(Bot bot)
        {
            FriendInviteHandlers.TryRemove(bot, out FriendInviteHandler friendInviteHandler);
            BotProfiles.TryRemove(bot, out UserProfile userProfile);
            Config.FriendInviteConfigs.TryRemove(bot, out Config config);
        }

        public void OnBotDisconnected(Bot bot, EResult reason) { }

        public async Task<bool> OnBotFriendRequest(Bot bot, ulong steamID)
        {
            await Task.Delay(5000);

            Config.FriendInviteConfigs.TryGetValue(bot, out Config config);
            if (!config.Enabled)
                return false;

            if (FriendInviteHandlers.TryGetValue(bot, out FriendInviteHandler friendInviteHandler))
            {
                await friendInviteHandler.processFriendRequest(steamID, bot);
            }
            return false;
        }

        public void OnBotInit(Bot bot) { }

        public void OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            if (additionalConfigProperties != null)
            {
                additionalConfigProperties.TryGetValue("SteamInviteHelper", out JToken jToken);

                if (Config.FriendInviteConfigs.TryGetValue(bot, out Config oldConfig))
                {
                    Config.FriendInviteConfigs.TryUpdate(bot, new Config(bot, jToken), oldConfig);
                }
                else
                {
                    Config.FriendInviteConfigs.TryAdd(bot, new Config(bot, jToken));
                }
            }
            else
            {
                Config.AppendDefaultConfig(bot);
            }
        }

        public void OnBotLoggedOn(Bot bot) { }

        public void OnBotSteamCallbacksInit(Bot bot, CallbackManager callbackManager) { }

        public IReadOnlyCollection<ClientMsgHandler> OnBotSteamHandlersInit(Bot bot)
        {
            FriendInviteHandler CurrentFriendInviteHandler = new FriendInviteHandler();
            FriendInviteHandlers.TryAdd(bot, CurrentFriendInviteHandler);
            return new HashSet<ClientMsgHandler> { CurrentFriendInviteHandler };
        }

        public void OnLoaded()
        {
            ASF.ArchiLogger.LogGenericInfo(@"          ___                       ___");
            ASF.ArchiLogger.LogGenericInfo(@"         /\  \          ___        /\__\");
            ASF.ArchiLogger.LogGenericInfo(@"        /::\  \        /\  \      /:/  /");
            ASF.ArchiLogger.LogGenericInfo(@"       /:/\ \  \       \:\  \    /:/__/");
            ASF.ArchiLogger.LogGenericInfo(@"      _\:\~\ \  \      /::\__\  /::\  \ ___");
            ASF.ArchiLogger.LogGenericInfo(@"     /\ \:\ \ \__\  __/:/\/__/ /:/\:\  /\__\");
            ASF.ArchiLogger.LogGenericInfo(@"     \:\ \:\ \/__/ /\/:/  /    \/__\:\/:/  /");
            ASF.ArchiLogger.LogGenericInfo(@"      \:\ \:\__\   \::/__/          \::/  /");
            ASF.ArchiLogger.LogGenericInfo(@"       \:\/:/  /    \:\__\          /:/  /");
            ASF.ArchiLogger.LogGenericInfo(@"        \::/  /      \/__/         /:/  /");
            ASF.ArchiLogger.LogGenericInfo(@"         \/__/                     \/__/");
            ASF.ArchiLogger.LogGenericInfo(@"              Steam Invite Helper");
        }
    }
}
