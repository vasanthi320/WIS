using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Caching;
using System.Web.Hosting;
using System.Web.Security;
using System.Configuration;
using log4net;
using NLog;
using System.Configuration.Provider;
using System.DirectoryServices.AccountManagement;
using System.DirectoryServices;
using System.Collections.Specialized;

namespace WIS.Auth
{
    public sealed class ADRoleProvider : RoleProvider
    {
        //ADRoleProvider Private Fields
        private String _ApplicationName;
        private String _ActiveDirectoryConnectionString;
        private String _DomainDN;
        private Boolean _IsAdditiveGroupMode;
        private ArrayList _GroupsToUse = new ArrayList();
        private ArrayList _GroupsToIgnore = new ArrayList();
        private ArrayList _UsersToIgnore = new ArrayList();
        private Boolean _EnableCache;
        private Int32 _CacheTimeoutInMinutes;
        private Dictionary<string, List<string>> _Aliases = new Dictionary<string, List<string>>();
        // IMPORTANT - DEFAULT LIST OF ACTIVE DIRECTORY USERS TO "IGNORE"
        //             DO NOT REMOVE ANY OF THESE UNLESS YOU FULLY UNDERSTAND THE SECURITY IMPLICATIONS
        //             VERYIFY THAT ALL CRITICAL USERS ARE IGNORED DURING TESTING
        private String[] _DefaultUsersToIgnore = new String[]
            {
                "Administrator", "TsInternetUser", "Guest", "krbtgt", "Replicate", "SERVICE", "SMSService"
            };
        // IMPORTANT - DEFAULT LIST OF ACTIVE DIRECTORY DOMAIN GROUPS TO "IGNORE"
        //             PREVENTS ENUMERATION OF CRITICAL DOMAIN GROUP MEMBERSHIP
        //             DO NOT REMOVE ANY OF THESE UNLESS YOU FULLY UNDERSTAND THE SECURITY IMPLICATIONS
        //             VERIFY THAT ALL CRITICAL GROUPS ARE IGNORED DURING TESTING BY CALLING GetAllRoles MANUALLY
        private String[] _DefaultGroupsToIgnore = new String[]
           {
                "Domain Guests", "Domain Computers", "Group Policy Creator Owners", "Guests", "Users",
                "Domain Users", "Pre-Windows 2000 Compatible Access", "Exchange Domain Servers", "Schema Admins",
                "Enterprise Admins", "Domain Admins", "Cert Publishers", "Backup Operators", "Account Operators",
                "Server Operators", "Print Operators", "Replicator", "Domain Controllers", "WINS Users",
                "DnsAdmins", "DnsUpdateProxy", "DHCP Users", "DHCP Administrators", "Exchange Services",
                "Exchange Enterprise Servers", "Remote Desktop Users", "Network Configuration Operators",
                "Incoming Forest Trust Builders", "Performance Monitor Users", "Performance Log Users",
                "Windows Authorization Access Group", "Terminal Server License Servers", "Distributed COM Users",
                "Administrators", "Everybody", "RAS and IAS Servers", "MTS Trusted Impersonators",
                "MTS Impersonators", "Everyone", "LOCAL", "Authenticated Users"
           };
        #region ADRoleProvider Properties
        public override String ApplicationName
        {
            get { return _ApplicationName; }
            set { _ApplicationName = value; }
        }
        #endregion

        private ILogger Logger { get; set; }

        #region ADRoleProvider Member Functions
        /// <summary>
        /// Initialize ADRoleProvider with config values
        /// </summary>
        /// <param name="name"></param>
        /// <param name="config"></param>
        public override void Initialize(String name, NameValueCollection config)
        {
            Logger = NLog.LogManager.GetLogger("Global");
            // Initialize values from web.config.
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }
            if (String.IsNullOrEmpty(name))
            {
                name = "ADRoleProvider";
            }
            if (String.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Active Directory Role Provider");
            }

            // Initialize the abstract base class.
            base.Initialize(name, config);

            // Retrieve Active Directory Connection String from config
            String temp = config["activeDirectoryConnectionString"];
            if (String.IsNullOrEmpty(temp))
            {
                throw new ProviderException("The attribute 'activeDirectoryConnectionString' is missing or empty.");
            }
            ConnectionStringSettings connObj = ConfigurationManager.ConnectionStrings[temp];
            if (connObj != null)
            {
                _ActiveDirectoryConnectionString = connObj.ConnectionString;
            }
            if (String.IsNullOrEmpty(_ActiveDirectoryConnectionString))
            {
                throw new ProviderException("The connection name 'activeDirectoryConnectionString' was not found in the applications configuration or the connection string is empty.");
            }
            if (_ActiveDirectoryConnectionString.Substring(0, 10) == "LDAP://DC=")
            {
                _DomainDN = _ActiveDirectoryConnectionString.Substring(7, _ActiveDirectoryConnectionString.Length - 7);
            }
            else
            {
                throw new ProviderException("The connection string specified in 'activeDirectoryConnectionString' does not appear to be a valid LDAP connection string.");
            }

            // Retrieve Application Name
            _ApplicationName = config["applicationName"];
            if (String.IsNullOrEmpty(_ApplicationName))
            {
                _ApplicationName = GetDefaultAppName();
            }
            if (_ApplicationName.Length > 256)
            {
                throw new ProviderException("The application name is too long.");
            }

            // Retrieve Group Mode
            // "Additive" indicates that only the groups specified in groupsToUse will be used
            // "Subtractive" indicates that all Active Directory groups will be used except those specified in groupsToIgnore
            // "Additive" is somewhat more secure, but requires more maintenance when groups change
            temp = config["groupMode"];
            if (String.IsNullOrEmpty(temp))
            {
                throw new ProviderException("The attribute 'groupMode' is missing or empty.");
            }
            if (temp == "Additive")
            {
                _IsAdditiveGroupMode = true;
            }
            else if (temp == "Subtractive")
            {
                _IsAdditiveGroupMode = false;
            }
            else
            {
                throw new ProviderException("The attribute 'groupMode' must be set to 'Additive' or 'Subtractive'.");
            }

            // If Additive group mode, populate GroupsToUse with specified AD groups
            if (_IsAdditiveGroupMode)
            {
                if (!String.IsNullOrEmpty(config["groupsToUse"]))
                {
                    foreach (String group in config["groupsToUse"].Trim().Split(','))
                    {
                        _GroupsToUse.Add(group.Trim());
                    }
                }
            }

            // Populate GroupsToIgnore ArrayList with AD groups that should be ignored for roles purposes
            foreach (String group in _DefaultGroupsToIgnore)
            {
                _GroupsToIgnore.Add(group.Trim());
            }
            if (!String.IsNullOrEmpty(config["groupsToIgnore"]))
            {
                foreach (String group in config["groupsToIgnore"].Trim().Split(','))
                {
                    _GroupsToIgnore.Add(group.Trim());
                }
            }

            // Populate UsersToIgnore ArrayList with AD users that should be ignored for roles purposes
            foreach (String group in _DefaultUsersToIgnore)
            {
                _UsersToIgnore.Add(group.Trim());
            }
            if (!String.IsNullOrEmpty(config["usersToIgnore"]))
            {
                foreach (String group in config["usersToIgnore"].Trim().Split(','))
                {
                    _UsersToIgnore.Add(group.Trim());
                }
            }

            // Check if Caching is enabled
            if (!String.IsNullOrEmpty(config["enableCache"]))
            {
                if (config["enableCache"] == "True")
                {
                    _EnableCache = true;
                }
                else if (config["enableCache"] == "False")
                {
                    _EnableCache = false;
                }
                else
                {
                    throw new ProviderException("The attribute 'enableCache' is specified as an invalid value. Must be 'True' or 'False'.");
                }
            }

            if (!String.IsNullOrEmpty(config["cacheTimeInMinutes"]))
            {
                int timeout = 0;
                if (int.TryParse(config["cacheTimeInMinutes"], out timeout))
                {
                    _CacheTimeoutInMinutes = timeout;
                }
                else
                {
                    throw new ProviderException("The attribute 'cacheTimeInMinutes' is specified as an invalid value. Must be a integer number.");
                }
            }

            if (!String.IsNullOrEmpty(config["roleAliases"]))
            {
                var list = config["roleAliases"].Split('|');
                foreach (var a in list)
                {
                    var split = a.Split(':');
                    if (split.Length != 2)
                        throw new ProviderException("The attribute 'roleAliases' is specified as an invalid value.");
                    _Aliases.Add(split[0], split[1].Split(',').ToList());
                }
            }
        }

        private string FindAlias(string group)
        {
            foreach (var a in _Aliases)
            {
                if (a.Value.Contains(group))
                    return a.Key;
            }
            return group;
        }

        private List<string> FindGroups(string alias)
        {
            if (_Aliases.ContainsKey(alias))
                return _Aliases[alias];
            return new List<string> { alias };
        }

        /// <summary>
        /// Retrieve listing of all roles to which a specified user belongs.
        /// </summary>
        /// <param name="username"></param>
        /// <returns>String array of roles</returns>
        public override String[] GetRolesForUser(String username)
        {
            if (_EnableCache)
            {
                String CachedValue;
                CachedValue = GetCacheItem('U', username);
                if (CachedValue != "*NotCached")
                {
                    return CachedValue.Split(',');
                }
            }
            ArrayList results = new ArrayList();
            var domain = ConfigurationManager.AppSettings["AdDomain"];
            using (PrincipalContext context = new PrincipalContext(ContextType.Domain, domain, _DomainDN))
            {
                try
                {
                    UserPrincipal p = UserPrincipal.FindByIdentity(context, IdentityType.SamAccountName, username);
                    var groups = p.GetAuthorizationGroups();
                    foreach (GroupPrincipal group in groups)
                    {
                        if (!_GroupsToIgnore.Contains(group.SamAccountName))
                        {
                            if (_IsAdditiveGroupMode && !_GroupsToUse.Contains(group.SamAccountName))
                            {
                                continue;
                            }
                            results.Add(FindAlias(group.SamAccountName));
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    throw new ProviderException("Unable to query Active Directory.", ex);
                }
            }
            // If Caching is enabled, send value to cache
            if (_EnableCache)
            {
                SetCacheItem('U', username, ArrayListToCSString(results));
            }
            return results.ToArray(typeof(String)) as String[];
        }

        /// <summary>
        /// Retrieve listing of all users in a specified role.
        /// </summary>
        /// <param name="rolename">String array of users</param>
        /// <returns></returns>
        public override String[] GetUsersInRole(String rolename)
        {
            ArrayList results = new ArrayList();
            foreach (var group in FindGroups(rolename))
            {
                if (!RoleExists(group))
                {
                    throw new ProviderException(String.Format("The role '{0}' was not found.", group));
                }
                // If Caching is enabled, try to pull a cached value.
                if (_EnableCache)
                {
                    String CachedValue;
                    CachedValue = GetCacheItem('R', group);
                    if (CachedValue != "*NotCached")
                    {
                        return CachedValue.Split(',');
                    }
                }

                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, null, _DomainDN))
                {
                    try
                    {
                        GroupPrincipal p = GroupPrincipal.FindByIdentity(context, IdentityType.SamAccountName, group);
                        var users = p.GetMembers(true);
                        foreach (UserPrincipal user in users)
                        {
                            if (!_UsersToIgnore.Contains(user.SamAccountName))
                            {
                                results.Add(user.SamAccountName);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.Message, ex);
                        throw new ProviderException("Unable to query Active Directory.", ex);
                    }
                }
                // If Caching is enabled, send value to cache
                if (_EnableCache)
                {
                    SetCacheItem('R', group, ArrayListToCSString(results));
                }
            }
            return results.ToArray(typeof(String)) as String[];
        }

        /// <summary>
        /// Determine if a specified user is in a specified role.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="rolename"></param>
        /// <returns>Boolean indicating membership</returns>
        public override bool IsUserInRole(string username, string rolename)
        {
            foreach (String strUser in GetUsersInRole(rolename))
            {
                if (username == strUser) return true;
            }
            return false;
        }

        /// <summary>
        /// Retrieve listing of all roles.
        /// </summary>
        /// <returns>String array of roles</returns>
        public override string[] GetAllRoles()
        {
            // If Caching is enabled, try to pull a cached value.
            if (_EnableCache)
            {
                String CachedValue;
                CachedValue = GetCacheItem('L', "AllRoles");
                if (CachedValue != "*NotCached")
                {
                    return CachedValue.Split(',');
                }
            }
            ArrayList results = new ArrayList();
            String[] roles = ADSearch(_ActiveDirectoryConnectionString, "(&(objectCategory=group)(|(groupType=-2147483646)(groupType=-2147483644)(groupType=-2147483640)))", "samAccountName");
            foreach (String strRole in roles)
            {
                if (!_GroupsToIgnore.Contains(strRole))
                {
                    if (_IsAdditiveGroupMode)
                    {
                        if (_GroupsToUse.Contains(strRole))
                        {
                            results.Add(strRole);
                        }
                    }
                    else
                    {
                        results.Add(strRole);
                    }
                }
            }
            if (_EnableCache)
            {
                SetCacheItem('L', "AllRoles", ArrayListToCSString(results));
            }
            return results.ToArray(typeof(String)) as String[];
        }

        /// <summary>
        /// Determine if given role exists
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <returns>Boolean indicating existence of role</returns>
        public override bool RoleExists(string rolename)
        {
            foreach (String strRole in GetAllRoles())
            {
                if (rolename == strRole) return true;
            }
            return false;
        }

        /// <summary>
        /// Return sorted list of usernames like usernameToMatch in rolename
        /// </summary>
        /// <param name="rolename">Role to check</param>
        /// <param name="usernameToMatch">Partial username to check</param>
        /// <returns></returns>
        public override string[] FindUsersInRole(string rolename, string usernameToMatch)
        {
            if (!RoleExists(rolename))
            {
                throw new ProviderException(String.Format("The role '{0}' was not found.", rolename));
            }
            ArrayList results = new ArrayList();
            String[] roles = GetAllRoles();
            foreach (String role in roles)
            {
                if (role.ToLower().Contains(usernameToMatch.ToLower()))
                {
                    results.Add(role);
                }
            }
            results.Sort();
            return results.ToArray(typeof(String)) as String[];
        }
        #endregion

        #region NonSupported Base Class Functions
        /// <summary>
        /// AddUsersToRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void AddUsersToRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to add users to roles.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// CreateRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void CreateRole(string rolename)
        {
            throw new NotSupportedException("Unable to create new role.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// DeleteRole not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override bool DeleteRole(string rolename, bool throwOnPopulatedRole)
        {
            throw new NotSupportedException("Unable to delete role.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }

        /// <summary>
        /// RemoveUsersFromRoles not supported.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory. 
        /// </summary>
        public override void RemoveUsersFromRoles(string[] usernames, string[] rolenames)
        {
            throw new NotSupportedException("Unable to remove users from roles.  For security and management purposes, ADRoleProvider only supports read operations against Active Direcory.");
        }
        #endregion

        #region ADRoleProvider Helper Functions
        /// <summary>
        /// Performs an extremely constrained query against Active Directory.  Requests only a single value from
        /// AD based upon the filtering parameter to minimize performance hit from large queries.
        /// </summary>
        /// <param name="ConnectionString">Active Directory Connection String</param>
        /// <param name="filter">LDAP format search filter</param>
        /// <param name="field">AD field to return</param>
        /// <param name="scopeQuery">Display name of the distinguished name attribute to search in</param>
        /// <returns>String array containing values specified by 'field' parameter</returns>
        private String[] ADSearch(String ConnectionString, String filter, String field)
        {
            String strResults = "";
            DirectorySearcher searcher = new DirectorySearcher();
            searcher.SearchRoot = new DirectoryEntry(ConnectionString);
            searcher.Filter = filter;
            searcher.PropertiesToLoad.Clear();
            searcher.PropertiesToLoad.Add(field);
            searcher.PageSize = 500;
            SearchResultCollection results;
            try
            {
                results = searcher.FindAll();
            }
            catch (Exception ex)
            {
                throw new ProviderException("Unable to query Active Directory.", ex);
            }
            foreach (SearchResult result in results)
            {
                int resultCount = result.Properties[field].Count;
                for (int c = 0; c < resultCount; c++)
                {
                    String temp = result.Properties[field][c].ToString();
                    strResults += temp + "|";
                }
            }
            // IMPORTANT - Dispose SearchResulCollection to prevent memory leak
            results.Dispose();
            if (strResults.Length > 0)
            {
                // Remove trailing |.
                strResults = strResults.Substring(0, strResults.Length - 1);
                return strResults.Split('|');
            }
            return new string[0];
        }

        /// <summary>
        /// Write item to cache
        /// </summary>
        /// <param name="ItemType">
        ///     Type of object being cached
        ///     L = Complete list of roles
        ///     U = User (i.e. list of roles for user)
        ///     R = Role (i.e. list of users in role)
        /// </param>
        /// <param name="ItemKey">UserName or RoleName</param>
        /// <param name="ItemValue">Relevant list</param>
        private void SetCacheItem(Char ItemType, String ItemKey, String ItemValue)
        {
            MemoryCache.Default.Add(ItemType + "-" + ItemKey, ItemValue, DateTime.Now.AddMinutes(_CacheTimeoutInMinutes));
        }

        /// <summary>
        /// Read item from cache
        /// </summary>
        /// <param name="ItemType">
        ///     Type of object being cached
        ///     L = Complete list of roles
        ///     U = User (i.e. list of roles for user)
        ///     R = Role (i.e. list of users in role)
        /// </param>
        /// <param name="ItemKey">UserName or RoleName</param>
        /// <returns>Comma-delimited list</returns>
        private String GetCacheItem(Char ItemType, String ItemKey)
        {
            var item = MemoryCache.Default.Get(ItemType + "-" + ItemKey) as string;
            if (item == null)
                return "*NotCached";
            return item;
        }

        /// <summary>
        /// Convert ArrayList to comma-delimited string
        /// </summary>
        /// <param name="inArray">ArrayList to convert</param>
        /// <returns>Comma-delimited string</returns>
        private String ArrayListToCSString(ArrayList inArray)
        {
            String result = "";
            foreach (String item in inArray)
            {
                result += item + ",";
            }
            if (!String.IsNullOrEmpty(result))
            {
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }

        /// <summary>
        /// Retrieve the current app name if none has been specified in config.
        /// As implimented by MS, lifted from SqlRoleProvider.
        /// </summary>
        /// <returns>String containing the current app name.</returns>
        private static string GetDefaultAppName()
        {
            try
            {
                string appName = HostingEnvironment.ApplicationVirtualPath;
                if (String.IsNullOrEmpty(appName))
                {
                    appName = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;
                    int indexOfDot = appName.IndexOf('.');
                    if (indexOfDot != -1)
                    {
                        appName = appName.Remove(indexOfDot);
                    }
                }
                if (String.IsNullOrEmpty(appName))
                {
                    return "/";
                }
                else
                {
                    return appName;
                }
            }
            catch
            {
                return "/";
            }
        }
        #endregion

    }
}