using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Drops.Drop
{
    public class AddEditDropFormatViewModel : BindableBase
    {
        private IBeyondRepository _db;

        public AddEditDropFormatViewModel(IBeyondRepository db)
        {
            _db = db;
            AddEditDropFormat = new RelayCommand(OnAddEditDropFormat, CanSave);
            CancelCommand = new RelayCommand(OnCancelCommand);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private DropFormat _editingdropFormat;
        private EditableDropFormat _editableDropFormat;

        public EditableDropFormat EditableDropFormat
        {
            get { return _editableDropFormat; }
            set { SetProperty(ref _editableDropFormat, value); }
        }

        #endregion

        #region Methods 
        public RelayCommand AddEditDropFormat { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public event Action<string> Completed;

        public void SetDropFormat(DropFormat format)
        {
            _editingdropFormat = format;
            if (EditableDropFormat != null) EditableDropFormat.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableDropFormat = new EditableDropFormat();
            EditableDropFormat.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingDropFormat(_editingdropFormat, EditableDropFormat);
        }

        private async void CopyEditingDropFormat(DropFormat editingdropFormat, EditableDropFormat editableDropFormat)
        {
            editableDropFormat.Id = editingdropFormat.Id;
            if (EditMode)
            {
                editableDropFormat.DropId = editingdropFormat.DropId;
                editableDropFormat.DropName = editingdropFormat.DropName;
                editableDropFormat.CreateDate = editingdropFormat.CreateDate;
                editableDropFormat.UpdateDate = editingdropFormat.UpdateDate;
                editableDropFormat.UpdatedBy = editingdropFormat.UpdatedBy;
                editableDropFormat.Clients = await _db.GetClientByDropFormatIdAsync(editingdropFormat.Id);
                editableDropFormat.DropFormatDetails = await _db.GetAllDropFormatDetailsByDropFormatId(editingdropFormat.Id);
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditDropFormat.RaiseCanExecuteChanged();
        }

        private bool CanSave() => !EditableDropFormat.HasErrors;

        private void OnCancelCommand() => Completed($"Add/Edit Drop Format has been Cancelled.");

        private void OnAddEditDropFormat() => UpdateDropFileFormat(EditableDropFormat, _editingdropFormat);

        private async void UpdateDropFileFormat(EditableDropFormat editableDropFormat, DropFormat editingdropFormat)
        {
            editingdropFormat.DropName = editableDropFormat.DropName;
            editingdropFormat.DropId = editableDropFormat.DropId;
            editingdropFormat.UpdateDate = editableDropFormat.UpdateDate;
            editingdropFormat.UpdatedBy = editableDropFormat.UpdatedBy;
            if (EditMode)
            {
                editingdropFormat.UpdateDate = DateTime.Now;
                editingdropFormat.CreateDate = editableDropFormat.CreateDate;
                editingdropFormat.UpdatedBy = Environment.UserName;
                if(await _db.UpdateDropFromatAsync(editingdropFormat) > 0)
                {
                    Completed($"Drop Format: {editingdropFormat.DropName} has been successfully updated.");
                }
            }
            else
            {
                //editingdropFormat.UpdateDate = DateTime.Now;
                editingdropFormat.CreateDate = DateTime.Now;
                editingdropFormat.UpdatedBy = Environment.UserName;
                if (await _db.AddDropFormatAsync(editingdropFormat) > 0)
                {
                    Completed($"Drop Format: {editingdropFormat.DropName} has been successfully Added.");
                }
            }

        }
        #endregion
    }
}
