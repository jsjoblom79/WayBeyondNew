﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Drops.Formats
{
    public class FileFormatViewModel: BindableBase
    {
        private IBeyondRepository _db;

        public FileFormatViewModel(IBeyondRepository db)
        {
            _db = db;
            ClearSearchTerm = new RelayCommand(OnClearSearchTerm);
            AddFileFormatCommand = new RelayCommand(OnAddFileFormatCommand);
            EditFileFormatCommand = new RelayCommand<FileFormat>(OnEditFileFormatCommand);
            DeleteFileFormatCommand = new RelayCommand<FileFormat>(OnDeleteFileFormatCommand);
        }

        #region Properties

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set 
            { 
                SetProperty(ref _searchTerm, value);
                FilterFileFormats();
            }
        }

        private List<FileFormat> _allFileFormats;
        private ObservableCollection<FileFormat> _fileFormats;

        public ObservableCollection<FileFormat> FileFormats
        {
            get { return _fileFormats; }
            set { SetProperty(ref _fileFormats, value); }
        }

        #endregion

        #region Methods
        public RelayCommand ClearSearchTerm { get; private set; }
        public RelayCommand AddFileFormatCommand { get; private set; }
        public RelayCommand<FileFormat> EditFileFormatCommand { get; private set; }
        public RelayCommand<FileFormat> DeleteFileFormatCommand { get; private set; }

        public event Action<FileFormat, bool> AddEditFileFormatRequest;
        public event Action<string> Completed;

        public async void OnViewLoaded()
        {
            _allFileFormats = await _db.GetAllFileFormatsAsync();
            FileFormats = new ObservableCollection<FileFormat>(_allFileFormats);
            
        }

        private void FilterFileFormats()
        {
            if (string.IsNullOrWhiteSpace(_searchTerm))
            {
                FileFormats = new ObservableCollection<FileFormat>(_allFileFormats);
            }
            else
            {
                FileFormats = new ObservableCollection<FileFormat>(_allFileFormats.Where(f=> f.FileFormatName.ToLower().Contains(SearchTerm.ToLower())));
            }
        }

        private void OnClearSearchTerm() => SearchTerm = null;

        private void OnAddFileFormatCommand() => AddEditFileFormatRequest(new FileFormat(), false);

        private void OnEditFileFormatCommand(FileFormat fileFormat) => AddEditFileFormatRequest(fileFormat, true);

        private async void OnDeleteFileFormatCommand(FileFormat format)
        {
            if(await _db.DeleteObjectAsync(format) > 0)
            {
                OnViewLoaded();
                Completed($"File Format: {format.FileFormatName} has been deleted.");
            }
        }
        #endregion
    }
}
