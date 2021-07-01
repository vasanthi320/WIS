using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class UserInfo
    {
        public string Uid { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
    }

   public class UserUtils
    {
        private static SearchResult SearchDirectory(string uid)
        {
            var connString = ConfigurationManager.ConnectionStrings["ActiveDirCS"];
            var domain = ConfigurationManager.AppSettings["AdDomain"];
            var accountNameProperty = ConfigurationManager.AppSettings["AdAccountNameProperty"];

            uid = uid.Replace(domain + @"\", "");
            var entry = new DirectoryEntry(connString.ConnectionString);
            var dirSearcher = new DirectorySearcher(entry);
            dirSearcher.Filter = "(&(objectClass=user)(objectcategory=person)(" + accountNameProperty + "=" + uid + "*))";
            SearchResult srEmail = dirSearcher.FindOne();
            return srEmail;
        }

        private static string GetProperty(SearchResult sr, string propertyName)
        {
            return sr.Properties[ConfigurationManager.AppSettings[propertyName]][0].ToString();
        }

        public static string FindEmail(string uid)
        {
            SearchResult srEmail = SearchDirectory(uid);
            if (srEmail == null)
                return null;

            string propName = ConfigurationManager.AppSettings["AdMailProperty"];
            ResultPropertyValueCollection valColl = srEmail.Properties[propName];
            try
            {
                return valColl[0].ToString().ToLower();
            }
            catch
            {
                return null;
            }
        }


        public static UserInfo FindUserInfo(string Uid)
        {
            try
            {
                SearchResult sr = SearchDirectory(Uid);

                if (sr != null && sr.Properties != null && sr.Properties.PropertyNames != null)
                {
                    return new UserInfo()
                    {
                        Uid = GetProperty(sr, "AdAccountNameProperty"),
                        DisplayName = GetProperty(sr, "AdDisplayNameProperty"),
                        Email = GetProperty(sr, "AdMailProperty").ToLower()
                    };
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public static string Default(string checkStr, string defaultStr)
        {
            return String.IsNullOrEmpty(checkStr) ? defaultStr : checkStr;
        }
    }
}
