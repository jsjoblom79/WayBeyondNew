using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Drops.Drop
{
    public class DropFormatViewModel: BindableBase
    {
        private IBeyondRepository _db;

        public DropFormatViewModel(IBeyondRepository db)
        {
            _db = db;

            ClearSearchTerm = new RelayCommand(OnClearSearchTerm);
            AddDropFormatCommand = new RelayCommand(OnAddDropFormatCommand);
            EditDropFormatCommand = new RelayCommand<DropFormat>(OnEditDropFormatCommand);
            DeleteDropFormatCommand = new RelayCommand<DropFormat>(OnDeleteFromatCommand);
        }

        #region Properties
        private List<DropFormat> _allDropformats;
        private ObservableCollection<DropFormat> _dropformats;

        public ObservableCollection<DropFormat> DropFormats
        {
            get { return _dropformats; }
            set { SetProperty(ref _dropformats, value); }
        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set { SetProperty(ref _searchTerm, value); }
        }

        #endregion

        #region Methods
        public RelayCommand ClearSearchTerm { get; private set; }
        public RelayCommand AddDropFormatCommand { get; private set; }
        public RelayCommand<DropFormat> EditDropFormatCommand { get; private set; } 
        public RelayCommand<DropFormat> DeleteDropFormatCommand { get; private set; }

        public event Action<DropFormat, bool> AddEditDropFormatRequest;
        public event Action<string> Completed;

        public async void OnViewLoaded()
        {
            _allDropformats = await _db.GetAllDropFormatsAsync();
            DropFormats = new ObservableCollection<DropFormat>(_allDropformats);
        }

        private void OnClearSearchTerm()
        {
            SearchTerm = null;
        }
        private void OnAddDropFormatCommand()
        {
            AddEditDropFormatRequest(new DropFormat(), false);
        }
        private void OnEditDropFormatCommand(DropFormat format)
        {
            AddEditDropFormatRequest(format, true);
        }
        private async void OnDeleteFromatCommand(DropFormat dropFormat)
        {
            if(await _db.DeleteDropFromatAsync(dropFormat) > 0)
            {
                OnViewLoaded();
                Completed($"Drop Format: {dropFormat.DropName} has been deleted.");
            }
        }
        #endregion
    }
}
