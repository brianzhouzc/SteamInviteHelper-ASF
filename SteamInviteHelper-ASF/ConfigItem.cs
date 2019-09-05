using System;
using System.Collections.Generic;
using System.Text;

namespace SteamInviteHelper_ASF
{
    class ConfigItem
    {
        public string condition { get; set; }
        public string value { get; set; }
        public string action { get; set; }

        public ConfigItem(string condition, string value, string action)
        {
            this.condition = condition;
            this.value = value;
            this.action = action;
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
