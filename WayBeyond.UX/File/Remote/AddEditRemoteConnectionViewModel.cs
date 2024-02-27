using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Remote
{
    public class AddEditRemoteConnectionViewModel : BindableBase
    {

        private IBeyondRepository _db;
        public AddEditRemoteConnectionViewModel(IBeyondRepository db)
        {
            _db = db;
            SaveConnectionCommand = new RelayCommand(OnSaveConnectionCommand, CanSave);
            CancelConnectionCommand = new RelayCommand(OnCancelConnectionCommand);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private RemoteConnection _editingRemoteConnection;


        private NewEditableConnection _editableConnection;

        public NewEditableConnection EditableConnection
        {
            get { return _editableConnection; }
            set { SetProperty(ref _editableConnection, value); }
        }

        #endregion

        #region Methods
        public RelayCommand SaveConnectionCommand { get; private set; }
        public RelayCommand CancelConnectionCommand { get; private set; }

        public event Action<string> Completed;

        public bool CanSave()
        {
            return !EditableConnection.HasErrors;
        }
        private void OnCancelConnectionCommand()
        {
            Completed("Add/Edit Remote Connection Cancelled.");
        }

        private void OnSaveConnectionCommand()
        {
            UpdateRemoteConnection(EditableConnection, _editingRemoteConnection);
        }

        private void UpdateRemoteConnection(NewEditableConnection editableConnection, RemoteConnection editingRemoteConnection)
        {
            if (EditMode)
            {
                editingRemoteConnection.Host = editableConnection.Host;
                editingRemoteConnection.Port = editableConnection.Port;
                editingRemoteConnection.UserName = editableConnection.Username;
                editingRemoteConnection.Password = editableConnection.Password;
                editingRemoteConnection.FingerprintRequired = editableConnection.FingerprintRequired;
                editingRemoteConnection.FingerPrint = editableConnection.Fingerprint;
                editingRemoteConnection.UpatedDate = DateTime.Now;
                editingRemoteConnection.Name = editableConnection.Name;

                _db.UpdateRemoteConnectionsAsync(editingRemoteConnection);
                Completed($"Remote Connection: {editingRemoteConnection.Name} has been Upated.");
            }
            else
            {
                editingRemoteConnection.Host = editableConnection.Host;
                editingRemoteConnection.Port = editableConnection.Port;
                editingRemoteConnection.UserName = editableConnection.Username;
                editingRemoteConnection.Password = editableConnection.Password;
                editingRemoteConnection.FingerprintRequired = editableConnection.FingerprintRequired;
                editingRemoteConnection.FingerPrint = editableConnection.Fingerprint;
                editingRemoteConnection.CreateDate = DateTime.Now;
                editingRemoteConnection.Name = editableConnection.Name;

                _db.AddRemoteConnectionAsync(editingRemoteConnection);
                Completed($"Remote Connection: {editingRemoteConnection.Name} has been Added.");
            }
        }

        public async void SetRemoteConnection(RemoteConnection connection)
        {
            _editingRemoteConnection = connection;
            if (EditableConnection != null) EditableConnection.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableConnection = new NewEditableConnection();
            EditableConnection.ErrorsChanged += RaiseCanExecuteChanged;
            CopyRemoteConnection(_editingRemoteConnection, EditableConnection);
        }

        private void CopyRemoteConnection(RemoteConnection editingRemoteConnection, NewEditableConnection editableConnection)
        {
            editableConnection.Id = editingRemoteConnection.Id;
            if (EditMode)
            {
                editableConnection.Host = editingRemoteConnection.Host;
                editableConnection.Port = editingRemoteConnection.Port;
                editableConnection.Username = editingRemoteConnection.UserName;
                editableConnection.Password = editingRemoteConnection.Password;
                editableConnection.FingerprintRequired = editingRemoteConnection.FingerprintRequired;
                editableConnection.Fingerprint = editingRemoteConnection.FingerPrint;
                editableConnection.Name = editingRemoteConnection.Name;
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            SaveConnectionCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
