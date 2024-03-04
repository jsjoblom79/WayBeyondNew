using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Location
{
    public class AddEditFileLocationViewModel : BindableBase
    {
        private IBeyondRepository _db;
        private IRando _rando;
        public AddEditFileLocationViewModel(IBeyondRepository db, IRando rando)
        {
            _db = db;
            _rando = rando;
            AddEditFileLocation = new RelayCommand(OnAddEditFileLocation, CanSave);
            CancelCommand = new RelayCommand(OnCancelCommand);
            RemoveRemoteConnection = new RelayCommand(OnRemoveRemoteConnection);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private FileLocation _editingFileLocation;
        private EditableFileLocation _editableFileLocation;

        public EditableFileLocation EditableFileLocation
        {
            get { return _editableFileLocation; }
            set { SetProperty(ref _editableFileLocation, value); }
        }

        private List<RemoteConnection> _remoteConnections;

        public List<RemoteConnection> RemoteConnections
        {
            get { return _remoteConnections; }
            set { SetProperty(ref _remoteConnections, value); }
        }


        #endregion

        #region Methods
        public RelayCommand AddEditFileLocation { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand RemoveRemoteConnection { get; private set; }

        public event Action<string> Completed;

        public async void SetFileLocation(FileLocation location)
        {
            _editingFileLocation = location;
            if (EditableFileLocation != null) EditableFileLocation.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableFileLocation = new EditableFileLocation();
            EditableFileLocation.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingFileLocation(_editingFileLocation, EditableFileLocation);
            RemoteConnections = await _db.GetAllRemoteConnectionsAsync();
        }

        private async void CopyEditingFileLocation(FileLocation editingFileLocation, EditableFileLocation editableFileLocation)
        {
            editableFileLocation.Id = editingFileLocation.Id;
            if (EditMode)
            {
                editableFileLocation.FileLocationName = editingFileLocation.FileLocationName;
                editableFileLocation.Path = editingFileLocation.Path;
                editableFileLocation.RemoteConnectionId = editingFileLocation.RemoteConnectionId;
                editableFileLocation.FileType = editingFileLocation.FileType;
                editableFileLocation.RemoteConnection = await _db.GetRemoteConnectionByIdAsync(editingFileLocation.RemoteConnectionId);
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditFileLocation.RaiseCanExecuteChanged();
        }

        private bool CanSave()
        {
            return !EditableFileLocation.HasErrors;
        }
        private void OnCancelCommand()
        {
            Completed("Add/Edit File Location has been cancelled.");
        }
        private async void OnAddEditFileLocation()
        {
            UpdateFileLocation(EditableFileLocation, _editingFileLocation);
        }

        private async void UpdateFileLocation(EditableFileLocation editableFileLocation, FileLocation editingFileLocation)
        {
            editingFileLocation.FileLocationName = editableFileLocation.FileLocationName;
            editingFileLocation.Path = editableFileLocation.Path;
            editingFileLocation.FileType = editableFileLocation.FileType;
            editingFileLocation.RemoteConnectionId = editableFileLocation.RemoteConnectionId;
            if (EditMode)
            {
                if(await _db.UpdateObjectAsync(editingFileLocation) >0)
                {
                    Completed($"File Location: {editingFileLocation.FileLocationName} was successfully updated.");
                }
            }else
            {
                if(await _db.AddFileLocationsAsync(editingFileLocation) > 0)
                {
                    Completed($"File Location: {editingFileLocation.FileLocationName} was successfully added.");
                }
            }
        }

        private void OnRemoveRemoteConnection()
        {
            EditableFileLocation.RemoteConnectionId = null;
        }

        #endregion
    }
}
