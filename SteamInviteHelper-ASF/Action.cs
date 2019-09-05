using System;
using System.Collections.Generic;
using System.Text;

namespace SteamInviteHelper_ASF
{
    class Action : IEquatable<Action>
    {
        public string action { get; set; }
        public string reason { get; set; }

        public Action(string action, string reason)
        {
            this.action = action;
            this.reason = reason;
        }

        public Action(string action)
        {
            this.action = action;
            this.reason = "No rules matched";
        }

        public bool Equals(Action other)
        {
            return other.action.Equals(this.action, StringComparison.OrdinalIgnoreCase);
        }
    }
}
