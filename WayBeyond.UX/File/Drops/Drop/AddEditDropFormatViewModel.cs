using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private IRando _rando;

        public AddEditDropFormatViewModel(IBeyondRepository db,IRando rando)
        {
            _db = db;
            _rando = rando;
            AddEditDropFormat = new RelayCommand(OnAddEditDropFormat, CanSave);
            CancelCommand = new RelayCommand(OnCancelCommand);
            AddDetailCommand = new RelayCommand(OnAddDetailCommand);
            ClearDetailCommand = new RelayCommand(OnClearDetailCommand);
            DeleteDropDetail = new RelayCommand<DropFormatDetail>(OnDeleteDropFormatDetail);
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

        private List<string> _dropFields = new List<string>();
        private ObservableCollection<string> _fields;

        public ObservableCollection<string> Fields
        {
            get { return _fields; }
            set { SetProperty(ref _fields, value); }
        }

        private DropFormatDetail _editingDropDetailFormat;
        private EditableDropDetailFormat _editableDropDetailFormat;

        public EditableDropDetailFormat EditableDropDetailFormat
        {
            get { return _editableDropDetailFormat; }
            set { SetProperty(ref _editableDropDetailFormat, value); }
        }

        #endregion

        #region Methods 
        public RelayCommand AddEditDropFormat { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand AddDetailCommand { get; private set; }
        public RelayCommand ClearDetailCommand { get; private set; }
        public RelayCommand<DropFormatDetail> DeleteDropDetail { get; private set; }

        public event Action<string> Completed;

        public void SetDropFormat(DropFormat format)
        {
            _editingdropFormat = format;
            _editingDropDetailFormat = new DropFormatDetail();
            if (EditableDropFormat != null) EditableDropFormat.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableDropFormat = new EditableDropFormat();
            EditableDropDetailFormat = new EditableDropDetailFormat();
            EditableDropFormat.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingDropFormat(_editingdropFormat, EditableDropFormat);
        }

        private async void CopyEditingDropFormat(DropFormat editingdropFormat, EditableDropFormat editableDropFormat)
        {
            editableDropFormat.Id = editingdropFormat.Id;
            EditableDropDetailFormat.DropFormatId = editingdropFormat.Id;
            if (EditMode)
            {
                editableDropFormat.DropId = editingdropFormat.DropId;
                editableDropFormat.DropName = editingdropFormat.DropName;
                editableDropFormat.CreateDate = editingdropFormat.CreateDate;
                editableDropFormat.UpdateDate = editingdropFormat.UpdateDate;
                editableDropFormat.UpdatedBy = editingdropFormat.UpdatedBy;
                editableDropFormat.Clients = await _db.GetClientByDropFormatIdAsync(editingdropFormat.Id);
                editableDropFormat.DropFormatDetails = await GetDropFormatDetail(editingdropFormat.Id);
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditDropFormat.RaiseCanExecuteChanged();
        }

        private bool CanSave() => !EditableDropFormat.HasErrors;

        private void OnCancelCommand() => Completed($"Add/Edit Drop Format has been Cancelled.");
        private void OnClearDetailCommand()
        {
            EditableDropDetailFormat = new();
            EditableDropDetailFormat.DropFormatId = EditableDropFormat.Id;
        }
        private async void OnAddDetailCommand()
        {
            UpdateDropDetailFormat(EditableDropDetailFormat, _editingDropDetailFormat);
            EditableDropFormat.DropFormatDetails = await GetDropFormatDetail(EditableDropFormat.Id);
            OnClearDetailCommand();
        }

        private async void UpdateDropDetailFormat(EditableDropDetailFormat editableDropDetailFormat, DropFormatDetail editingDropDetailFormat)
        {
            editingDropDetailFormat.Id = 0;
            editingDropDetailFormat.DropFormatId = editableDropDetailFormat.DropFormatId;
            editingDropDetailFormat.Field = editableDropDetailFormat.DetailField;
            editingDropDetailFormat.Position = editableDropDetailFormat.DetailPosition;
            editingDropDetailFormat.FieldType = editableDropDetailFormat.DetailFieldType;

            await _db.AddDropFormatDetailAsync(editingDropDetailFormat);

        }

        private void OnAddEditDropFormat() => UpdateDropFileFormat(EditableDropFormat, _editingdropFormat);
        
        private async Task<List<DropFormatDetail>> GetDropFormatDetail(long id) => await _db.GetAllDropFormatDetailsByDropFormatId(id);

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
                if(await _db.UpdateObjectAsync(editingdropFormat) > 0)
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
        public async void GetFieldProperties()
        {
            Fields = new ObservableCollection<string>(await _rando.GetDebtorPropertiesAsync());
        }

        private async void OnDeleteDropFormatDetail(DropFormatDetail detail)
        {
            if(await _db.DeleteObjectAsync(detail) > 0)
            {
                EditableDropFormat.DropFormatDetails = await GetDropFormatDetail(EditableDropFormat.Id);
            }
        }
        #endregion
    }
}
