using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.Reporting
{
    public class ProcessedFilesViewModel: BindableBase
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        public ProcessedFilesViewModel(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;
            _transfer = transfer;
            UploadDropFile = new RelayCommand<FileObject>(OnUploadDropFile);
            DeleteBadDropFile = new RelayCommand<FileObject>(OnDeleteBadDropFile);
            DeleteClientLoadCommand = new RelayCommand<ClientLoad>(OnDeleteClientLoadCommand);
            ClearClientLoads = new RelayCommand(OnClearClientLoads);
            CreateClientLoad = new RelayCommand(OnCreateClientLoad);
        }
        #region Properties

        private List<ClientLoad> _currentClientLoads;
        private ObservableCollection<ClientLoad> _clientLoads;

        public ObservableCollection<ClientLoad> ClientLoads
        {
            get { return _clientLoads; }
            set { SetProperty(ref _clientLoads, value); }
        }

        private List<FileObject> _allPreparedFiles = new List<FileObject>();
        private ObservableCollection<FileObject> _preparedFiles;

        public ObservableCollection<FileObject> PreparedFiles
        {
            get { return _preparedFiles; }
            set { SetProperty(ref _preparedFiles, value); }
        }

        private List<ProcessedFileBatch> _allBatches;
        private ObservableCollection<ProcessedFileBatch?> _batches;

        public ObservableCollection<ProcessedFileBatch?> Batches
        {
            get { return _batches; }
            set { SetProperty(ref _batches, value); }
        }


        private ProcessedFileBatch? _selectedBatch;

        public ProcessedFileBatch? SelectedBatch
        {
            get { return _selectedBatch; }
            set { SetProperty(ref _selectedBatch, value);
                GetClientLoads(value);
            }
        }

        #endregion

        #region Methods

        public RelayCommand<FileObject> UploadDropFile { get; private set; }
        public RelayCommand<FileObject> DeleteBadDropFile { get; private set; }
        public RelayCommand<ClientLoad> DeleteClientLoadCommand { get; private set; }

        public RelayCommand ClearClientLoads { get; private set; }
        public RelayCommand CreateClientLoad { get; private set; }

        public event Action<string> StatusUpdate = delegate { };

        public async void OnViewLoaded()
        {
            var allBatches = _db.GetAllProcessedFilesBatchAsync();
            //var clientLoads = _db.GetClientLoadsByDateAsync(DateTime.Now.Date);
            var fileLocations = _db.GetFileLocationsByNameAsync(LocationName.Prepared);

            var onLoadTasks = new List<Task> { allBatches,  fileLocations };

            while (onLoadTasks.Count > 0)
            {
                Task finishedTasks = await Task.WhenAny(onLoadTasks);

                if(finishedTasks == allBatches)
                {
                    _allBatches = allBatches.Result;
                    Batches = new ObservableCollection<ProcessedFileBatch?>(_allBatches);
                    SelectedBatch = _allBatches.Where(b => b.UpdateDate == null).FirstOrDefault();
                    if(SelectedBatch != null)
                    {
                        _currentClientLoads = SelectedBatch.ClientLoads.ToList();
                        ClientLoads = new ObservableCollection<ClientLoad>(_currentClientLoads);
                    }
                    
                }
                //else if(finishedTasks == clientLoads)
                //{
                //    _currentClientLoads = clientLoads.Result;
                //    ClientLoads = new ObservableCollection<ClientLoad>(_currentClientLoads);
                //}
                else if (finishedTasks == fileLocations)
                {
                    _allPreparedFiles.Clear();
                    foreach (var location in fileLocations.Result)
                    {
                        _allPreparedFiles.AddRange(await _transfer.GetFileObjectsAsync(location));
                    }
                    PreparedFiles = new ObservableCollection<FileObject>(_allPreparedFiles);
                }

                await finishedTasks;
                onLoadTasks.Remove(finishedTasks);
            } 
        }

        private async void GetClientLoads(ProcessedFileBatch? value)
        {
            if(value != null)
            {
                ClientLoads = new ObservableCollection<ClientLoad?>(await _db.GetAllClientLoadsByBatchIdAsync(value.Id));
            }
            else
            {
                if(_currentClientLoads != null)
                {
                    ClientLoads = new ObservableCollection<ClientLoad?>(_currentClientLoads);
                }
                
            }
        }

        private async void OnUploadDropFile(FileObject file)
        {
            if (await _transfer.UploadFile(file))
            {
                await _transfer.DeleteFileAsync(file);
                StatusUpdate($"File: {file.FileName} has been uploaded.");
                _allPreparedFiles.Clear();
                OnViewLoaded();
            }
        }

        private async void OnDeleteBadDropFile(FileObject file)
        {
            if(await _transfer.DeleteFileAsync(file))
            {
                StatusUpdate($"File: {file.FileName} has been deleted.");
                _allPreparedFiles.Clear();
                OnViewLoaded();
            }
        }

        private async void OnDeleteClientLoadCommand(ClientLoad load)
        {
            if(await _db.DeleteObjectAsync(load) > 0)
            {
                StatusUpdate($"Client Load: {load.ClientName} was removed from Batch.");
               
                OnViewLoaded();

            }
        }

        private async void OnCreateClientLoad()
        {
            var location = await _db.GetFileLocationsByNameAsync(LocationName.ClientLoadFile);
            ExcelService excel = new ExcelService();
            var filename = $"{location[0].Path}Clients_Loaded_" + DateTime.Now.ToString("MMddyyyy");
            var excelResult = excel.WriteClientLoadFile(ClientLoads.ToList(),filename);

            if(await excelResult)
            {
                SelectedBatch.UpdateDate = DateTime.Now;
                if(await _db.UpdateObjectAsync(SelectedBatch) > 0)
                {
                    StatusUpdate($"Batch: {SelectedBatch.BatchName} has been updated.\n Drop File Created.");
                }
            }
            OnViewLoaded();
        }

        private async void OnClearClientLoads()
        {
            SelectedBatch = null;
            ClientLoads = null;
            OnViewLoaded();
        }
        #endregion
    }
}
