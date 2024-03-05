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
        public ClientLoadViewModel(IBeyondRepository db, ITransfer transfer)
        {
            _db = db;   
            _transfer = transfer;
            _fileObjects = new List<FileObject>();
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

        #endregion

        #region Methods
        public async void OnViewLoaded()
        {
            foreach (var location in await _db.GetFileLocationByNameAsync("Placements"))
            {
                _fileObjects.AddRange(await _transfer.GetFileObjectsAsync(location));
            }
            Clients = new ObservableCollection<Client>(await _db.GetAllClientsAsync());
            PlacementFiles = new ObservableCollection<FileObject>(_fileObjects);
        }
        #endregion
    }
}
