using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.Services
{
    public class ConfigurationEncryptionService
    {
        private const string _protectionProvider = "DataProtectionConfigurationProvider";
        public static void EncryptConfiguration()
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var appSettings = config.GetSection("appSettings");
            var connectionStrings = config.GetSection("connectionStrings");

            if(appSettings != null && !appSettings.SectionInformation.IsProtected)
            {
                appSettings.SectionInformation.ProtectSection(_protectionProvider);
                config.Save();
            }

            if(connectionStrings != null && !connectionStrings.SectionInformation.IsProtected)
            {
                connectionStrings.SectionInformation.ProtectSection(_protectionProvider);
                config.Save();
            }
        }
    }
}
