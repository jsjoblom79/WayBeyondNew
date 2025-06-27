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
            _allEpicFiles.Clear();
            var locations = await _db.GetFileLocationsByNameAsync(LocationName.EpicPlacements);
            foreach (var location in locations)
            {
                _allEpicFiles.AddRange(await _transfer.GetFileObjectsAsync(location));
            }
            EpicFiles = new ObservableCollection<FileObject>(_allEpicFiles);
            
        }

        private async void OnProcessCommand()
        {
            var isNorLeaComplete = false;
            var isRghComplete = false;
            var isLovingtonComplete = false;
            var isAANMComplete = false;
            var isFaithComplete = false;
            foreach (var file in _epicFiles)
            {
                if (file.FullPath.ToLower().Contains("norlea"))
                {
                    IEpicClientProcess Proc = new NorLeaClientProcess(_db, _transfer);
                    isNorLeaComplete = await Proc.ProcessEpicClientAsync(file,new[] { new Client() });
                }
                if (file.FullPath.ToLower().Contains("rghosp"))
                {
                    IEpicClientProcess proc = new RghClientProcess(_db, _transfer);
                    isRghComplete = await proc.ProcessEpicClientAsync(file, new Client() );
                    Completed($"Processing File: {file.FileName}");
                }
                if (file.FullPath.ToLower().Contains("anesphesia"))
                {
                    IEpicClientProcess proc = new AanmaClientProcess(_db, _transfer);
                    isAANMComplete = await proc.ProcessEpicClientAsync(file, new Client());
                    Completed($"Processing File: {file.FileName}");
                }
                if (file.FullPath.ToLower().Contains("faithcommunity"))
                {
                    Completed($"Processing File: {file.FileName}");
                }
                //if (file.FullPath.ToLower().Contains("lovingtonfire"))
                //{
                //    IEpicClientProcess love = new FarmingtonFireClientProcess(_db,_transfer);
                //    isLovingtonComplete = await love.ProcessEpicClientAsync(file, await _db.GetClientByClientIdAsync(1338));
                //    Completed($"Processing File: {file.FileName}");
                //}
            }
        }
        #endregion


    }
}
