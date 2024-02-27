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
        public async void OnViewLoaded()
        {
            _allClients = await _db.GetAllClientsAsync();
            Clients = new ObservableCollection<Client>(_allClients);
        }
        private void FilterClients(string? searchTerm)
        {
            if(searchTerm == null)
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
