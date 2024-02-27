using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Settings
{
    public class SettingsViewModel : BindableBase
    {
        private IBeyondRepository _db;
        public SettingsViewModel(IBeyondRepository db)
        {

            _db = db;

        }

        #region Properties

        private List<Setting> _allSettings;
        private ObservableCollection<Setting> _settings;

        public ObservableCollection<Setting> Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        #endregion

        #region Methods
        public async void OnViewLoaded()
        {
            _allSettings = await _db.GetAllSettingsAsync();
            Settings = new ObservableCollection<Setting>(_allSettings);
        }
        #endregion
    }
}
