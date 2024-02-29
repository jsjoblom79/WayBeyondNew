using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Location
{
    public class FileLocationViewModel: BindableBase
    {
        private IBeyondRepository _db;

        public FileLocationViewModel(IBeyondRepository db)
        {
            _db = db;
            EditCommand = new RelayCommand<FileLocation>(OnEditCommand);
            AddCommand = new RelayCommand(OnAddCommand);
            DeleteCommand = new RelayCommand<FileLocation>(OnDeleteCommand);
            ClearSearchCommand = new RelayCommand(OnClearSearchCommand);
        }

        #region Properties
        private List<FileLocation> _allFileLocations;
        private ObservableCollection<FileLocation> _fileLocations;

        public ObservableCollection<FileLocation> FileLocations
        {
            get { return _fileLocations; }
            set { SetProperty(ref _fileLocations, value); }
        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set 
            { 
                SetProperty(ref _searchTerm, value);
                FilterFileLocations(_searchTerm);
            }
        }


        #endregion

        #region Methods
        public RelayCommand AddCommand { get; private set; }
        public RelayCommand<FileLocation> EditCommand { get; private set; }
        public RelayCommand<FileLocation> DeleteCommand { get; private set; }
        public RelayCommand ClearSearchCommand { get; private set; }

        public event Action<FileLocation, bool> AddEditCommandRequest;
        public event Action<string> Complete;

        public async void OnViewLoaded()
        {
            _allFileLocations = await _db.GetAllFileLocationsAsync();
            FileLocations = new ObservableCollection<FileLocation>(_allFileLocations);
        }

        private async void OnAddCommand()
        {
            AddEditCommandRequest(new FileLocation(), false);
        }

        private async void OnEditCommand(FileLocation location)
        {
            AddEditCommandRequest(location, true);
        }

        private async void OnDeleteCommand(FileLocation location)
        {
            if( await _db.DeleteObjectAsync(location) > 0)
            {
                OnViewLoaded();
                Complete($"File Location: {location.FileLocationName} has been delete.");
            }

        }

        private async void OnClearSearchCommand()
        {
            SearchTerm = null;
        }

        private void FilterFileLocations(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                FileLocations = new ObservableCollection<FileLocation>(_allFileLocations);
            }
            else
            {
                FileLocations = new ObservableCollection<FileLocation>(_allFileLocations.Where(l => l.FileLocationName.ToLower().Contains(searchTerm.ToLower())));
            }
        }
        #endregion
    }
}
