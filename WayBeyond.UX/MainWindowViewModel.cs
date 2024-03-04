using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using WayBeyond.Data.Models;
using WayBeyond.UX.File.Drops.Drop;
using WayBeyond.UX.File.Drops.Formats;
using WayBeyond.UX.File.Location;
using WayBeyond.UX.File.Maintenance;
using WayBeyond.UX.File.Remote;
using WayBeyond.UX.File.Settings;

namespace WayBeyond.UX
{
    public class MainWindowViewModel : BindableBase
    {
        private SettingsViewModel _settingsViewModel;
        private AddEditSettingViewModel _addEditSettingViewModel;
        private RemoteConnectionsViewModel _remoteConnectionViewModel;
        private AddEditRemoteConnectionViewModel _addEditRemoteConnectionViewModel;
        private ClientMaintenanceViewModel _clientMaintenanceViewModel;
        private AddEditClientViewModel _addEditClientViewModel;
        private FileLocationViewModel _fileLocationViewModel;
        private AddEditFileLocationViewModel _addEditFileLocationViewModel;
        private DropFormatViewModel _dropFormatViewModel;
        private AddEditDropFormatViewModel _addEditDropFormatViewModel;
        private FileFormatViewModel _fileFormatViewModel;
        private AddEditFileFormatViewModel _addEditFileFormatViewModel;

        public MainWindowViewModel()
        {
            NavigateCommand = new RelayCommand<string>(OnNavigation);
            _settingsViewModel = ContainerHelper.Container.Resolve<SettingsViewModel>();
            _addEditSettingViewModel = ContainerHelper.Container.Resolve<AddEditSettingViewModel>();
            _remoteConnectionViewModel = ContainerHelper.Container.Resolve<RemoteConnectionsViewModel>();
            _addEditRemoteConnectionViewModel = ContainerHelper.Container.Resolve<AddEditRemoteConnectionViewModel>();
            _clientMaintenanceViewModel = ContainerHelper.Container.Resolve<ClientMaintenanceViewModel>();
            _addEditClientViewModel = ContainerHelper.Container.Resolve<AddEditClientViewModel>();
            _fileLocationViewModel = ContainerHelper.Container.Resolve<FileLocationViewModel>();
            _addEditFileLocationViewModel = ContainerHelper.Container.Resolve<AddEditFileLocationViewModel>();
            _dropFormatViewModel = ContainerHelper.Container.Resolve<DropFormatViewModel>();
            _addEditDropFormatViewModel = ContainerHelper.Container.Resolve<AddEditDropFormatViewModel>();
            _fileFormatViewModel = ContainerHelper.Container.Resolve<FileFormatViewModel>();
            _addEditFileFormatViewModel = ContainerHelper.Container.Resolve<AddEditFileFormatViewModel>();

            _settingsViewModel.Completed += UpdateStatus;
            _settingsViewModel.AddEditSettingRequest += AddEditSettingCommand;
            _addEditSettingViewModel.Completed += SettingsComplete;

            _remoteConnectionViewModel.AddConnectionRequest += AddEditRemoteConnectionCommand;
            _remoteConnectionViewModel.EditConnectionRequest += AddEditRemoteConnectionCommand;
            _remoteConnectionViewModel.Completed += UpdateStatus;
            _addEditRemoteConnectionViewModel.Completed += RemoteSettingComplete;

            _clientMaintenanceViewModel.AddEditClientRequest += AddEditClientCommand;
            _clientMaintenanceViewModel.Completed += UpdateStatus;
            _addEditClientViewModel.Completed += AddEditClientComplete;

            //File Locations
            _fileLocationViewModel.AddEditCommandRequest += AddEditFileLocationCommand;
            _fileLocationViewModel.Complete += UpdateStatus;
            _addEditFileLocationViewModel.Completed += AddEditFileLocationComplete;

            //Drop Formats
            _dropFormatViewModel.AddEditDropFormatRequest += AddEditDropFormatCommand;
            _dropFormatViewModel.Completed += UpdateStatus;
            _addEditDropFormatViewModel.Completed += AddEditDropFileFormatCompleted;

            //FileFormats
            _fileFormatViewModel.AddEditFileFormatRequest += AddEditFileFormatCommand;
            _fileFormatViewModel.Completed += UpdateStatus;
            _addEditFileFormatViewModel.Completed += AddEditFileFormatCompleted;
        }

        private string _currentStatus;

        public string CurrentStatus
        {
            get { return _currentStatus; }
            set { SetProperty(ref _currentStatus, value); }
        }

        private BindableBase _currentViewModel;

        public BindableBase CurrentViewModel
        {
            get { return _currentViewModel; }
            set { SetProperty(ref _currentViewModel, value); }
        }

        #region Methods
        public RelayCommand<string> NavigateCommand { get; private set; }
        public static event Action Exit = delegate { };

        private async void OnNavigation(string nav)
        {
            switch (nav)
            {
                case "exit":
                    Exit();
                    break;
                case "settings":
                    CurrentViewModel = _settingsViewModel;
                    break;
                case "connection":
                    CurrentViewModel = _remoteConnectionViewModel;
                    break;
                case "maintenance":
                    CurrentViewModel = _clientMaintenanceViewModel;
                    break;
                case "location":
                    CurrentViewModel = _fileLocationViewModel;
                    break;
                case "drop":
                    CurrentViewModel = _dropFormatViewModel;
                    break;
                case "format":
                    CurrentViewModel = _fileFormatViewModel;
                    break;
                default:
                    break;
            }
        }

        private void AddEditClientComplete(string obj)
        {
            CurrentViewModel = _clientMaintenanceViewModel;
            UpdateStatus(obj);
        }

        private void AddEditClientCommand(Client client, bool arg2)
        {
            _addEditClientViewModel.EditMode = arg2;
            _addEditClientViewModel.SetClient(client);
            CurrentViewModel = _addEditClientViewModel;
        }

        private void SettingsComplete(string obj)
        {
            CurrentViewModel = _settingsViewModel;
            UpdateStatus(obj);
        }
        private void RemoteSettingComplete(string obj)
        {
            CurrentViewModel = _remoteConnectionViewModel;
            UpdateStatus(obj);
        }
        private void AddEditSettingCommand(Setting obj, bool editMode)
        {
            _addEditSettingViewModel.EditMode = editMode;
            _addEditSettingViewModel.SetSetting(obj);
            CurrentViewModel = _addEditSettingViewModel;
        }

        private void UpdateStatus(string message)
        {
            CurrentStatus = message;
        }


        private void AddEditRemoteConnectionCommand(RemoteConnection obj, bool editMode)
        {
            _addEditRemoteConnectionViewModel.EditMode = editMode;
            _addEditRemoteConnectionViewModel.SetRemoteConnection(obj);
            CurrentViewModel = _addEditRemoteConnectionViewModel;
        }

        private void AddEditFileLocationCommand(FileLocation location, bool arg2)
        {
            _addEditFileLocationViewModel.EditMode = arg2;
            _addEditFileLocationViewModel.SetFileLocation(location);
            CurrentViewModel = _addEditFileLocationViewModel;
        }

        private void AddEditFileLocationComplete(string obj)
        {
            CurrentViewModel = _fileLocationViewModel;
            UpdateStatus(obj);
        }


        private void AddEditDropFormatCommand(DropFormat format, bool editmode)
        {
            _addEditDropFormatViewModel.EditMode = editmode;
            _addEditDropFormatViewModel.SetDropFormat(format);
            _addEditDropFormatViewModel.GetFieldProperties();
            CurrentViewModel = _addEditDropFormatViewModel;
        }

        private void AddEditDropFileFormatCompleted(string obj)
        {
            CurrentViewModel = _dropFormatViewModel;
            UpdateStatus(obj);
        }

        private void AddEditFileFormatCommand(FileFormat format, bool arg2)
        {
            _addEditFileFormatViewModel.EditMode = arg2;
            _addEditFileFormatViewModel.SetFileFormat(format);
            _addEditFileFormatViewModel.GetFieldsAndColumns();
            CurrentViewModel = _addEditFileFormatViewModel;
        }

        private void AddEditFileFormatCompleted(string obj)
        {
            CurrentViewModel = _fileFormatViewModel;
            UpdateStatus(obj);
        }

        #endregion
    }
}
