using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.UX.Services;
using WayBeyond.Data.Models;
using System.Collections.ObjectModel;

namespace WayBeyond.UX.File.Maintenance
{
    public class ClientMaintenanceViewModel : BindableBase
    {
        private IBeyondRepository _db;

        public ClientMaintenanceViewModel(IBeyondRepository db)
        {
            _db = db;
            ClearSearchTerm = new RelayCommand(OnClearSearchTerm);
            AddNewClient = new RelayCommand(OnAddNewClient);
            EditCommand = new RelayCommand<Client>(OnEditClient);
            DeleteCommand = new RelayCommand<Client>(OnDeleteClient);

        }

        #region Properties
        private List<Client> _allClients;
        private ObservableCollection<Client> _clients;

        public ObservableCollection<Client> Clients
        {
            get { return _clients; }
            set { SetProperty(ref _clients, value); }
        }


        private string? _searchTerm;

        public string? SearchTerm
        {
            get { return _searchTerm; }
            set 
            { 
                SetProperty(ref _searchTerm, value);
                FilterClients(_searchTerm);
            }
        }

        

        #endregion

        #region Methods
        public RelayCommand ClearSearchTerm { get; private set; }
        public RelayCommand AddNewClient { get; private set; }
        public RelayCommand<Client> EditCommand { get; private set; }
        public RelayCommand<Client> DeleteCommand { get; private set; }

        public event Action<Client,bool> AddEditClientRequest;
        public event Action<string> Completed;
        
        public async void OnViewLoaded()
        {
            _allClients = await _db.GetAllClientsAsync();
            Clients = new ObservableCollection<Client>(_allClients);
        }

        private async void OnEditClient(Client client)
        {
            AddEditClientRequest(client, true);
        }
        private async void OnDeleteClient(Client client)
        {
            if (await _db.DeleteClientAsync(client) > 0)
            {
                OnViewLoaded();
                Completed($"Client: {client.ClientName} has been deleted.");
            }
        }
        private async void OnAddNewClient()
        {
            AddEditClientRequest(new Client(), false);
        }
        private async void OnClearSearchTerm()
        {
            SearchTerm = null;
        }
        private void FilterClients(string? searchTerm)
        {
            if(string.IsNullOrWhiteSpace(searchTerm))
            {
                Clients = new ObservableCollection<Client>(_allClients);
            }
            else
            {
                Clients = new ObservableCollection<Client>(_allClients.Where(c => c.ClientName.ToLower().Contains(searchTerm.ToLower())));
            }
        }
        #endregion

    }
}
