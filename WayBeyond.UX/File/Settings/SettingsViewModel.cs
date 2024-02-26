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
    public class SettingsViewModel: BindableBase
    {
        private IBeyondRepository _db;
        public SettingsViewModel(IBeyondRepository db)
        {
            _db = db;

            ClearSearch = new RelayCommand(OnClearSearch);
            AddSettingCommand = new RelayCommand(OnAddSetting);
            EditSettingCommand = new RelayCommand<Setting>(OnEditSetting);
            DeleteSettingCommand = new RelayCommand<Setting>(OnDeleteSetting);
        }

        #region Properties
        private List<Setting> _allSettings;
        private ObservableCollection<Setting> _applicationSettings;

        public ObservableCollection<Setting> ApplicationSettings
        {
            get { return _applicationSettings; }
            set { SetProperty(ref _applicationSettings, value); }

        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set
            {
                SetProperty(ref _searchTerm, value);
                FilterSettings(_searchTerm);
            }
        }

        #endregion
        #region Methods

        public async void OnViewLoaded()
        {
            _allSettings = await _db.GetAllSettingsAsync();
            ApplicationSettings = new ObservableCollection<Setting>(_allSettings);
        }


        //Relay Commands
        public RelayCommand ClearSearch { get; private set; }
        public RelayCommand AddSettingCommand { get; private set; }
        public RelayCommand<Setting> EditSettingCommand { get; private set; }
        public RelayCommand<Setting> DeleteSettingCommand { get; private set; }

        //Delegate Commands
        public event Action<Setting> AddSettingRequest = delegate { };
        public event Action<Setting> EditSettingRequest = delegate { };
        public event Action<Setting> DeleteSettingRequest = delegate { };

        private async void OnClearSearch()
        {
            SearchTerm = null;
        }

        private async void OnAddSetting()
        {
            AddSettingRequest(new Setting());
        }

        private async void OnEditSetting(Setting setting)
        {
            EditSettingRequest(setting);
        }

        private async void OnDeleteSetting(Setting setting)
        {
            DeleteSettingRequest(setting);

        }

        private void FilterSettings(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                ApplicationSettings = new ObservableCollection<Setting>(_allSettings);
                return;
            }
            else
            {
                ApplicationSettings = new ObservableCollection<Setting>(_allSettings.Where(c => c.Key.ToLower().Contains(searchTerm.ToLower())));
            }
        }
        #endregion
    }
}
