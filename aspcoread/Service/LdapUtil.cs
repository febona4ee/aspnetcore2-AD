using Microsoft.Extensions.Configuration;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace aspcoread.Service
{
    public class LdapUtil
    {
        
        public static string Host { get; private set; }
        public static string BindDN { get; private set; }
        public static string BindPassword { get; private set; }
        public static int Port { get; private set; }
        public static string BaseDC { get; private set; }
        public static string CookieName { get; private set; }

        public static void Register(IConfigurationRoot configuration)
        {
            Host = configuration.GetValue<string>("LDAPServer");
            Port = configuration?.GetValue<int>("LDAPPort") ?? 389;
            BindDN = configuration.GetValue<string>("BindDN");
            BindPassword = configuration.GetValue<string>("BindPassword");
            BaseDC = configuration.GetValue<string>("LDAPBaseDC");
            CookieName = configuration.GetValue<string>("CookieName");
        }


        public static bool Validate(string username, string password)
        {
            try
            {
                using (var conn = new LdapConnection())
                {
                    conn.Connect(Host, Port);
                    conn.Bind($"{BindDN},{BaseDC}",BindPassword);
                    var entities =
                        conn.Search(BaseDC, LdapConnection.ScopeSub,
                            $"(sAMAccountName={username})",
                            new string[] { "sAMAccountName" }, false);
                    string userDn = null;
                    while (entities.HasMore())
                    {
                        var entity = entities.Next();
                        var account = entity.GetAttribute("sAMAccountName");
                        if (account != null && account.StringValue == username)
                        {
                            userDn = entity.Dn;
                            break;
                        }
                    }

                    if (string.IsNullOrWhiteSpace(userDn)) return false;
                    conn.Bind(userDn, password);
                    conn.Disconnect();
                    return true;
                }
            }
            catch (LdapException e)
            {
                Console.WriteLine(e);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
