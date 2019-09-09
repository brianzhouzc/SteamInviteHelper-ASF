using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiSteamFarm;

namespace SteamInviteHelper_ASF
{
    class Logger
    {
        private static string constructString(params string[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[SIH] ");
            if (arguments.Length == 0)
                return sb.ToString();

            if (arguments.Length > 1)
            {
                sb.AppendFormat(arguments[0], arguments.Skip(1).ToArray());
            }
            else
            {
                sb.Append(arguments[0]);
            }
            return sb.ToString();
        }

        public static void LogInfo(params string[] arguments)
        {
            ASF.ArchiLogger.LogGenericInfo(constructString(arguments));
        }

        public static void LogWarning(params string[] arguments)
        {
            ASF.ArchiLogger.LogGenericWarning(constructString(arguments));
        }

        public static void LogError(params string[] arguments)
        {
            ASF.ArchiLogger.LogGenericError(constructString(arguments));
        }

        public static void LogDebug(params string[] arguments)
        {
            if (!ASF.GlobalConfig.Debug)
                return;
            ASF.ArchiLogger.LogGenericDebug(constructString(arguments));
        }
    }
}
