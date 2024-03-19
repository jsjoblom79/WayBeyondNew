using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        }
        #region Properties

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
            set { SetProperty(ref _selectedBatch, value); }
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
            
            _allBatches = await _db.GetAllProcessedFilesBatchAsync();

            SelectedBatch = _allBatches.Where(b => b.CreateDate.Value.Date == DateTime.Now.Date).FirstOrDefault();
            SelectedBatch.ClientLoads = await _db.GetAllClientLoadsByBatchIdAsync(SelectedBatch.Id);

            var locations = await _db.GetFileLocationByNameAsync(LocationName.Prepared);
            foreach (var location in locations)
            {
                _allPreparedFiles.AddRange(await _transfer.GetFileObjectsAsync(location));
            }

            PreparedFiles = new ObservableCollection<FileObject>(_allPreparedFiles);

            ClientLoads = new ObservableCollection<ClientLoad>(SelectedBatch.ClientLoads);

        }
        #endregion
    }
}
