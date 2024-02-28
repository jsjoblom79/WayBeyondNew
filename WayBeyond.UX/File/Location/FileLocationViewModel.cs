using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
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
        }

        #region Properties
        private List<FileLocation> _allFileLocations;
        private ObservableCollection<FileLocation> _fileLocations;

        public ObservableCollection<FileLocation> FileLocations
        {
            get { return _fileLocations; }
            set { SetProperty(ref _fileLocations, value); }
        }

        #endregion

        #region Methods
        public async void OnViewLoaded()
        {
            _allFileLocations = await _db.GetAllFileLocationsAsync();
        }
        #endregion
    }
}
