using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.Services
{
    public class ConfigurationHandler
    {
        private static EncryptionService _encryptionService = new EncryptionService();
        private Configuration _config;
        private ConnectionStringSettingsCollection _settingsCollection;

        public ConfigurationHandler()
        {
            
            _config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            _settingsCollection = _config.ConnectionStrings.ConnectionStrings;
        }
        public void UpdateConfigurationData(string key, string value)
        {

            foreach (ConnectionStringSettings setting in _settingsCollection)
            {
                if (setting.Name.Equals(key))
                {
                    setting.ConnectionString = _encryptionService.ProtectData(value);
                }
            }

            _config.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection(AppConstants.ConnectionStrings);
        }

        public void AddConfigurationData(string key, string value)
        {
            _settingsCollection.Add(new ConnectionStringSettings(key, _encryptionService.ProtectData(value)));

            _config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection(AppConstants.ConnectionStrings);
        }
        public static string GetConfigurationData(string key)
        {
            return (_encryptionService.UnProtectData(ConfigurationManager.ConnectionStrings[key].ConnectionString));
        }
    }
}
