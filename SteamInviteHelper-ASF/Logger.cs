using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArchiSteamFarm;

namespace SteamInviteHelper_ASF
{
    class Logger
    {
        public static void LogInfo(params string[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[SIH] ");
            if (arguments.Length > 1)
            {
                sb.AppendFormat(arguments[0], arguments.Skip(1).ToArray());
            }
            else
            {
                sb.Append(arguments[0]);
            }
            ASF.ArchiLogger.LogGenericInfo(sb.ToString());
        }

        public static void LogWarning(params string[] arguments)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[SIH] ");
            if (arguments.Length > 1)
            {
                sb.AppendFormat(arguments[0], arguments.Skip(1).ToArray());
            }
            else
            {
                sb.Append(arguments[0]);
            }
            ASF.ArchiLogger.LogGenericWarning(sb.ToString());
        }

        public static void LogDebug(params string[] arguments)
        {
            if (!ASF.GlobalConfig.Debug)
                return;

            StringBuilder sb = new StringBuilder();
            sb.Append("[SIH] ");
            if (arguments.Length > 1)
            {
                sb.AppendFormat(arguments[0], arguments.Skip(1).ToArray());
            }
            else
            {
                sb.Append(arguments[0]);
            }
            ASF.ArchiLogger.LogGenericDebug(sb.ToString());
        }
    }
}
