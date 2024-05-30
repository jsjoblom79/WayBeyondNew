using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.Processing.EpicLoads
{
    public class EpicClientLoadViewModel : BindableBase
    {
        private IBeyondRepository _db;
        private ITransfer _transfer;
        private IEpicClientProcess _clientProcess;

        public EpicClientLoadViewModel(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;
            _transfer = transfer;
            OnProcess = new RelayCommand(OnProcessCommand);

        }

        #region Properties
        private List<FileObject> _allEpicFiles = new List<FileObject>();
        private ObservableCollection<FileObject> _epicFiles;

        public event Action<string> Completed = delegate { };
        public ObservableCollection<FileObject> EpicFiles
        {
            get { return _epicFiles; }
            set { SetProperty(ref _epicFiles, value); }
        }

        #endregion

        #region Methods

        public RelayCommand OnProcess { get; private set; }

        public async void OnViewLoaded()
        {
            var locations = await _db.GetFileLocationByNameAsync(LocationName.EpicPlacements);
            foreach (var location in locations)
            {
                _allEpicFiles.AddRange(await _transfer.GetFileObjectsAsync(location));
            }
            EpicFiles = new ObservableCollection<FileObject>(_allEpicFiles);
            
        }

        private async void OnProcessCommand()
        {
            foreach (var file in _epicFiles)
            {
                if (file.FullPath.ToLower().Contains("norlea"))
                {
                    IEpicClientProcess Proc = new NorLeaClientProcess(_db,_transfer);
                    Proc.ProcessEpicClientAsync(file);
                }
                if (file.FullPath.ToLower().Contains("rghosp"))
                {
                    Completed($"Processing File: {file.FileName}");
                }
                if (file.FullPath.ToLower().Contains("anesphesia"))
                {
                    Completed($"Processing File: {file.FileName}");
                }
                if (file.FullPath.ToLower().Contains("faithcommunity"))
                {
                    Completed($"Processing File: {file.FileName}");
                }
                if (file.FullPath.ToLower().Contains("lovington fire"))
                {
                    Completed($"Processing File: {file.FileName}");
                }
            }
        }
        #endregion


    }
}
