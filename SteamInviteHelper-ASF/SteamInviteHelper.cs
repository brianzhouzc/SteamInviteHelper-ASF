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

    internal sealed class SteamInviteHelper : IASF, IBot, IBotCommand, IBotConnection, IBotFriendRequest, IBotMessage, IBotModules, IBotSteamClient

    {
        private static ConcurrentDictionary<Bot, FriendInviteHandler> FriendInviteHandlers = new ConcurrentDictionary<Bot, FriendInviteHandler>();

        public string Name => nameof(SteamInviteHelper);

        public Version Version => typeof(SteamInviteHelper).Assembly.GetName().Version;

        public void OnASFInit(IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            if (additionalConfigProperties == null)
            {
                return;
            }
        }

        public Task<string> OnBotCommand(Bot bot, ulong steamID, string message, string[] args){ return null; }

        public void OnBotDestroy(Bot bot){ }

        public void OnBotDisconnected(Bot bot, EResult reason){ }

        public Task<bool> OnBotFriendRequest(Bot bot, ulong steamID)
        {
            FriendInviteHandlers.GetOrAdd(bot, new FriendInviteHandler()).processFriendRequest(steamID, bot);
            return Task.FromResult(false);
        }

        public void OnBotInit(Bot bot){ }

        public async void OnBotInitModules(Bot bot, IReadOnlyDictionary<string, JToken> additionalConfigProperties = null)
        {
            await bot.Actions.Pause(true).ConfigureAwait(false);
        }

        public void OnBotLoggedOn(Bot bot)
        {
            Config.FriendInviteConfigs.TryAdd(bot, new Config(bot));
        }

        public Task<string> OnBotMessage(Bot bot, ulong steamID, string message)
        {
            return null;
        }

        public void OnBotSteamCallbacksInit(Bot bot, CallbackManager callbackManager){ }

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
