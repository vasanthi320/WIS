using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class Utils
    {
        public static string FixMailAddress(string address, string domain)
        {
            var slash = address.IndexOf(@"/");
            var at = address.IndexOf(@"@");
            if (slash >= 0)
                address = address.Substring(slash + 1);

            if (at < 0)
                address = address + "@" + domain;

            return address.Trim();
        }
    }
}
