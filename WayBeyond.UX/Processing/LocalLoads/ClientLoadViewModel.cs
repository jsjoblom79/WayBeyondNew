using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.Processing.LocalLoads
{
    public class ClientLoadViewModel: BindableBase
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private IClientProcess _clientProcess;
        public ClientLoadViewModel(IBeyondRepository db, ITransfer transfer, IClientProcess cp)
        {
            _db = db;   
            _transfer = transfer;
            _fileObjects = new List<FileObject>();
            _clientProcess = cp;
            ClearSelections = new RelayCommand(OnClearSelections);
            ProcessSelections = new RelayCommand(OnProcessSelections);
            ((ClientProcess)_clientProcess).ProcessUpdates += Completed;
        }

        #region Properties

        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set { SetProperty(ref _clients, value); }
        }

        private List<FileObject> _fileObjects;
        private ObservableCollection<FileObject> _placementFiles;

        public ObservableCollection<FileObject> PlacementFiles
        {
            get { return _placementFiles; }
            set { SetProperty(ref _placementFiles, value); }
        }


        private Client _selectedClient;

        public Client SelectedClient
        {
            get { return _selectedClient; }
            set { SetProperty(ref _selectedClient, value);
                if (SelectedFile != null)
                { 
                    Process = true;
                }
                else
                {
                    Process = false;
                }
                
            }
        }

        private FileObject _selectedFile;

        public FileObject SelectedFile
        {
            get { return _selectedFile; }
            set { SetProperty(ref _selectedFile, value);
                if(SelectedClient != null)
                {
                    Process = true;
                }
                else
                {
                    Process = false;
                }
            }
        }

        private bool _process;

        public bool Process
        {
            get { return _process; }
            set { SetProperty(ref _process, value); }
        }

        #endregion

        #region Methods
        public RelayCommand ClearSelections { get; private set; }
        public RelayCommand ProcessSelections { get; private set; }

        public event Action<string> Completed = delegate { };
        public async void OnViewLoaded()
        {
            foreach (var location in await _db.GetFileLocationByNameAsync(LocationName.Placements.ToString()))
            {
                _fileObjects.AddRange(await _transfer.GetFileObjectsAsync(location));
            }
            Clients = new ObservableCollection<Client>(await _db.GetAllClientsAsync());
            PlacementFiles = new ObservableCollection<FileObject>(_fileObjects);
        }

        private async void OnProcessSelections()
        {
            await _clientProcess.ProcessClientFile(SelectedFile, SelectedClient);
            Completed("Process Completed.");
        }
        private async void OnClearSelections()
        {
            SelectedClient = null;
            SelectedFile = null;
        }
        #endregion
    }
}
