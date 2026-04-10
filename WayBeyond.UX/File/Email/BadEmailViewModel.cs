using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Email
{
    public class BadEmailViewModel : BindableBase
    {
        private IBeyondRepository _db;
        public BadEmailViewModel(IBeyondRepository db)
        {
            _db = db;
            InitializeCommands();
        }

        #region Properties
        private List<BadEmailAddresses> _badEmails;
        private ObservableCollection<BadEmailAddresses> _badAddresses;

        public ObservableCollection<BadEmailAddresses> BadEmails
        {
            get { return _badAddresses; }
            set { SetProperty(ref _badAddresses, value); }
        }

        #endregion

        #region Commands

        public RelayCommand AddBadEmailCommand { get; private set; }
        public RelayCommand<BadEmailAddresses> DeleteEmailCommand { get; private set; }

        public event Action<BadEmailAddresses, bool> AddBadEmailAddress;
        public event Action<string> StatusUpdate;
        private void OnDeleteBadEmailAddress(BadEmailAddresses badEmail)
        {
            _db.DeleteObjectAsync(badEmail);
             BadEmails.Remove(badEmail);
            StatusUpdate($"{badEmail.ToString()} has been deleted.");
        }
        private void OnAddBadEmailCommand()
        {
            AddBadEmailAddress(new BadEmailAddresses(), false);
        }
        #endregion

        public void InitializeCommands()
        {
            AddBadEmailCommand = new RelayCommand(OnAddBadEmailCommand);
            DeleteEmailCommand = new RelayCommand<BadEmailAddresses>(OnDeleteBadEmailAddress);
        }
        public async void OnViewLoaded()
        {
            _badEmails = await _db.GetAllBadEmailAddresses();
            BadEmails = new ObservableCollection<BadEmailAddresses>(_badEmails);
        }
    }
}
