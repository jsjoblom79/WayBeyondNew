using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Remote
{
    public class RemoteConnectionsViewModel : BindableBase
    {

        private IBeyondRepository _db;
        public RemoteConnectionsViewModel(IBeyondRepository db)
        {
            _db = db;

            ClearSearchCommand = new RelayCommand(OnClearSearchCommand);
            EditConnectionCommand = new RelayCommand<RemoteConnection>(OnEditConnectionCommand);
            AddRemoteConnectionCommand = new RelayCommand(OnAddRemoteConnectionCommand);
            DeleteConnectionCommand = new RelayCommand<RemoteConnection>(OnDeleteConnectionCommand);
        }

        #region Properties
        private List<RemoteConnection> _allConnections;
        private ObservableCollection<RemoteConnection> _remoteConnections;

        public ObservableCollection<RemoteConnection> RemoteConnections
        {
            get { return _remoteConnections; }
            set { SetProperty(ref _remoteConnections, value); }
        }

        private string _searchTerm;

        public string SearchTerm
        {
            get { return _searchTerm; }
            set 
            { 
                SetProperty(ref _searchTerm, value);
                FilterRemoteConnections(_searchTerm);
            }
        }



        #endregion

        #region Methods
        public RelayCommand ClearSearchCommand { get; private set; }
        public RelayCommand<RemoteConnection> EditConnectionCommand { get; private set; }
        public RelayCommand AddRemoteConnectionCommand { get; private set; }
        public RelayCommand<RemoteConnection> DeleteConnectionCommand { get; private set; }

        public event Action<RemoteConnection,bool> AddConnectionRequest;
        public event Action<RemoteConnection,bool> EditConnectionRequest;
        public event Action<string> Completed;

        public async void OnViewLoaded()
        {
            _allConnections = await _db.GetAllRemoteConnectionsAsync();
            RemoteConnections = new ObservableCollection<RemoteConnection>(_allConnections);
        }
        
        private void OnEditConnectionCommand(RemoteConnection connection)
        {
            EditConnectionRequest(connection, true);
        }

        private void OnAddRemoteConnectionCommand()
        {
            AddConnectionRequest(new RemoteConnection(), false);
        }

        private async void OnDeleteConnectionCommand(RemoteConnection connection)
        {
            if(await _db.DeleteRemoteConnectionAsync(connection) > 0)
            {
                OnViewLoaded();
                Completed($"Remote Connection: {connection.Name} has been deleted.");
            }
            
        }
        private async void OnClearSearchCommand()
        {
            SearchTerm = null;
        }

        private void FilterRemoteConnections(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                RemoteConnections = new ObservableCollection<RemoteConnection>(_allConnections);
                return;
            }
            else
            {
                RemoteConnections = new ObservableCollection<RemoteConnection>(_allConnections.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower())));
            }

        }
        #endregion
    }
}
