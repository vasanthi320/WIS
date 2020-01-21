using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Principal;
using System.Web;

namespace WIS.Models
{
    public static class Helper
    {
        public static string GetFullName(this IIdentity id)
        {
            if (id == null) return null;

            using (var context = new PrincipalContext(ContextType.Domain))
            {
                var userPrincipal = UserPrincipal.FindByIdentity(context, id.Name);
                return userPrincipal != null ? $"{userPrincipal.GivenName} {userPrincipal.Surname}" : null;
            }
        }
    }
}