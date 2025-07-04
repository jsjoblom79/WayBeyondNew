﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Maintenance
{
    public class AddEditClientViewModel : BindableBase
    {
        private IBeyondRepository _db;

        public AddEditClientViewModel(IBeyondRepository db)
        {
            _db = db;
            CancelCommand = new RelayCommand(OnCancelCommand);
            AddEditClient = new RelayCommand(OnAddEditClient, CanSave);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private Client _editingClient;

        private EditableClient _editableClient;

        public EditableClient EditableClient
        {
            get { return _editableClient; }
            set { SetProperty(ref _editableClient, value); }
        }


        private List<FileFormat> _fileFormats;

        public List<FileFormat> FileFormats
        {
            get { return _fileFormats; }
            set { SetProperty(ref _fileFormats, value); }
        }


        private List<DropFormat> _dropFormats;

        public List<DropFormat> DropFormats
        {
            get { return _dropFormats; }
            set { SetProperty(ref _dropFormats, value); }
        }

        #endregion

        #region Methods
        public RelayCommand AddEditClient { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public event Action<string> Completed;

        private bool CanSave()
        {
            return !EditableClient.HasErrors;
        }
        private void OnCancelCommand()
        {
            Completed("Add/Edit Client Cancelled.");
        }

        private void OnAddEditClient()
        {
            UpdateClient(EditableClient, _editingClient);
        }

        private async void UpdateClient(EditableClient editableClient, Client editingClient)
        {
            editingClient.ClientId = editableClient.ClientId;
            editingClient.ClientName = editableClient.ClientName;
            editingClient.DropNumber = editableClient.DropNumber;
            editingClient.DropFileName = editableClient.DropFileName;
            editingClient.AssemblyName = editableClient.AssemblyName;
            editingClient.DropFormatId = editableClient.DropFormatId;
            editingClient.FileFormatId = editableClient.FileFormatId;
            editingClient.UseAssembly = editableClient.UseAssembly;

            if (EditMode)
            {
                await _db.UpdateObjectAsync(editingClient);
                Completed($"Client: {editingClient.ClientName} has been update.");
            }
            else
            {
                await _db.AddClientAsync(editingClient);
                Completed($"Client: {editingClient.ClientName} has been added.");
            }
        }

        public async void SetClient(Client client)
        {
            _editingClient = client;
            if (EditableClient != null) EditableClient.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableClient = new EditableClient();
            EditableClient.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingClient(_editingClient, EditableClient);
            FileFormats = await _db.GetAllFileFormatsAsync();
            DropFormats = await _db.GetAllDropFormatsAsync();
        }

        private async void CopyEditingClient(Client editingClient, EditableClient editableClient)
        {
            editableClient.Id = editingClient.Id;
            if (EditMode)
            {
                editableClient.ClientId = editingClient.ClientId;
                editableClient.ClientName = editingClient.ClientName;
                editableClient.DropNumber = editingClient.DropNumber;
                editableClient.DropFileName = editingClient.DropFileName;
                editableClient.AssemblyName = editingClient.AssemblyName;
                editableClient.DropFormatId = editingClient.DropFormatId;
                editableClient.FileFormatId = editingClient.FileFormatId;
                editableClient.UseAssembly = editingClient.UseAssembly;

                if(_editingClient.FileFormatId != null)
                    editableClient.FileFormat = await _db.GetFileFormatByIdAsync(editingClient.FileFormatId);
                if(_editingClient.DropFormatId != null)
                    editableClient.DropFormat = await _db.GetDropFormatByIdAsync(editingClient.DropFormatId);
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditClient.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
