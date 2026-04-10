using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Email
{
    public class AddBadEmailAddressViewModel: BindableBase
    {
        private IBeyondRepository _db;
        public AddBadEmailAddressViewModel(IBeyondRepository db)
        {
                _db = db;
            InitializeCommands();
        }


        private BadEmailAddresses _badEmail;

        public BadEmailAddresses BadEmail
        {
            get { return _badEmail; }
            set { SetProperty(ref _badEmail, value); }
        }

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }


        public RelayCommand SaveBadEmailCommand { get; private set; }

        public RelayCommand CancelBadEmailCommand { get; private set; }

        public event Action<string> Completed;
        private void OnSaveBadEmailCommand()
        {
            _db.AddBadEmailAddress(BadEmail);
            Completed($"{BadEmail.ToString()} has been added to the list of bad email addresses.");
        }

        private void OnCancelBadEmailCommand()
        {
            Completed("Add Bad Email Address has been cancelled.");
        }

        private void InitializeCommands()
        {
            SaveBadEmailCommand = new RelayCommand(OnSaveBadEmailCommand);
            CancelBadEmailCommand = new RelayCommand(OnCancelBadEmailCommand);
        }
        public void SetBadEmail(BadEmailAddresses badEmail)
        {
            BadEmail = badEmail;
        }
        public async void OnViewLoaded()
        {

        }
    }
}
