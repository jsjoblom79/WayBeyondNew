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
            ClearSearchCommand = new RelayCommand(OnClearSearch);
            AddCommand = new RelayCommand(OnAddCommand);
            EditCommand = new RelayCommand<Setting>(OnEditCommand);
            DeleteCommand = new RelayCommand<Setting>(OnDeleteCommand);



        }

        #region Properties

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

        private List<Setting> _allSettings;
        private ObservableCollection<Setting> _settings;

        public ObservableCollection<Setting> Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        #endregion

        #region Methods
        public RelayCommand ClearSearchCommand { get; private set; }
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand<Setting> EditCommand { get; private set; }
        public RelayCommand<Setting> DeleteCommand { get; private set; }

        public event Action<Setting, bool> AddEditSettingRequest;
        public event Action<string> Completed;

        public async void OnViewLoaded()
        {
            _allSettings = await _db.GetAllSettingsAsync();
            Settings = new ObservableCollection<Setting>(_allSettings);
        }

        private async void FilterSettings(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                Settings = new ObservableCollection<Setting>(_allSettings);
                return;
            }
            else
            {
                Settings = new ObservableCollection<Setting>(_allSettings.Where(s => s.Key.ToLower().Contains(searchTerm.ToLower())));
            }
        }
        private async void OnClearSearch()
        {
            SearchTerm = null;
        }

        private async void OnAddCommand()
        {
            AddEditSettingRequest(new Setting(), false);
        }

        private async void OnEditCommand(Setting setting)
        {
            AddEditSettingRequest(setting, true);
        }

        private async void OnDeleteCommand(Setting setting)
        {
            if(await _db.DeleteSettingsAsync(setting) > 0)
            {
                OnViewLoaded();
                Completed($"Setting: {setting.Key} has been deleted.");
            }
        }
        #endregion
    }
}
