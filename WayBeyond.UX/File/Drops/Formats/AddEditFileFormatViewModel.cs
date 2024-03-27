using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.File.Drops.Drop;
using WayBeyond.UX.Services;

namespace WayBeyond.UX.File.Drops.Formats
{
    public class AddEditFileFormatViewModel : BindableBase
    {
        private IBeyondRepository _db;
        private IRando _rando;

        public AddEditFileFormatViewModel(IBeyondRepository db, IRando rando)
        {
            _db = db;
            _rando = rando;
            AddEditCommand = new RelayCommand(OnAddEditFileFormat, CanSave);
            CancelCommand = new RelayCommand(OnCancelCommand);
            AddDetailCommand = new RelayCommand(OnAddDetailCommand);
            ClearDetailCommand = new RelayCommand(OnClearDetailCommand);
            DeleteFileFormatDetail = new RelayCommand<FileFormatDetail>(OnDeleteFileFormatDetail);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private FileFormat _editingFileFormat;
        private EditableFileFormat _editableAddEditFileFormat;

        public EditableFileFormat EditableAddEditFileFormat
        {
            get { return _editableAddEditFileFormat; }
            set { SetProperty(ref _editableAddEditFileFormat, value); }
        }

        private FileFormatDetail _editingFileFormatDetail;
        private EditableFileFormatDetail _editableFileFormatDetail;

        public EditableFileFormatDetail EditableFileFormatDetail
        {
            get { return _editableFileFormatDetail; }
            set { SetProperty(ref _editableFileFormatDetail, value); }
        }

        private ObservableCollection<string> _fields;

        public ObservableCollection<string> Fields
        {
            get { return _fields; }
            set { SetProperty(ref _fields, value); }
        }

        private ObservableCollection<string> _columnTypes;

        public ObservableCollection<string> ColumnTypes
        {
            get { return _columnTypes; }
            set { SetProperty(ref _columnTypes, value); }
        }


        #endregion

        #region Methods
        public RelayCommand AddEditCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }
        public RelayCommand AddDetailCommand { get; private set; }
        public RelayCommand ClearDetailCommand { get; private set; }
        public RelayCommand<FileFormatDetail> DeleteFileFormatDetail { get; private set; }

        public event Action<string> Completed;

        public async void SetFileFormat(FileFormat format)
        {
            _editingFileFormat = format;
            _editingFileFormatDetail = new FileFormatDetail();
            
            if (EditableAddEditFileFormat != null) EditableAddEditFileFormat.ErrorsChanged -= RaiseCanExecuteChanged;

            EditableAddEditFileFormat = new EditableFileFormat();
            EditableFileFormatDetail = new EditableFileFormatDetail();
            EditableAddEditFileFormat.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingFileFormat(_editingFileFormat, EditableAddEditFileFormat);
        }

        private async void CopyEditingFileFormat(FileFormat editingFileFormat, EditableFileFormat editableAddEditFileFormat)
        {
            editableAddEditFileFormat.Id = editingFileFormat.Id;
            EditableFileFormatDetail.FileFormatId = editingFileFormat.Id;
            if (EditMode)
            {
                editableAddEditFileFormat.FileFormatName = editingFileFormat.FileFormatName;
                editableAddEditFileFormat.CreateDate = editingFileFormat.CreateDate;
                editableAddEditFileFormat.UpdatedDate = editingFileFormat.UpdateDate;
                editableAddEditFileFormat.UpdatedBy = editingFileFormat.UpdatedBy;
                editableAddEditFileFormat.FileFormatDetails = await _db.GetAllFileFormatDetailsByFileFormatIdAsync(editingFileFormat.Id);
                editableAddEditFileFormat.FileStartLine = editingFileFormat.FileStartLine;
                editableAddEditFileFormat.ColumnForClientDebtorNumber = editingFileFormat.ColumnForClientDebtorNumber;
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditCommand.RaiseCanExecuteChanged();
        }
        private bool CanSave() => !EditableAddEditFileFormat.HasErrors;
        private void OnCancelCommand() => Completed("Add/Edit File Format has been Cancelled.");

        private void OnClearDetailCommand()
        {
            EditableFileFormatDetail = new EditableFileFormatDetail();
            EditableFileFormatDetail.FileFormatId = EditableAddEditFileFormat.Id;
        }
        private async void OnAddDetailCommand()
        {
            UpdateFileFormatDetail(EditableFileFormatDetail, _editingFileFormatDetail);
            EditableAddEditFileFormat.FileFormatDetails = await GetFileFormatDetails(EditableAddEditFileFormat.Id);
            OnClearDetailCommand();
        }

        private async void UpdateFileFormatDetail(EditableFileFormatDetail editableFileFormatDetail, FileFormatDetail editingFileFormatDetail)
        {
            editingFileFormatDetail.Id = 0;
            editingFileFormatDetail.Field = editableFileFormatDetail.Field;
            editingFileFormatDetail.ColumnType = editableFileFormatDetail.ColumnType;
            editingFileFormatDetail.FileColumn = editableFileFormatDetail.FileColumn;
            editingFileFormatDetail.SpecialCase = editableFileFormatDetail.SpecialCase;
            editingFileFormatDetail.FileFormatId = editableFileFormatDetail.FileFormatId;
           

            await _db.AddFileFormatDetailAsync(editingFileFormatDetail);
        }

        private void OnAddEditFileFormat() => UpdateFileFormat(EditableAddEditFileFormat, _editingFileFormat);

        private async void UpdateFileFormat(EditableFileFormat editableAddEditFileFormat, FileFormat editingFileFormat)
        {
            editingFileFormat.FileFormatName = editableAddEditFileFormat.FileFormatName;
            editingFileFormat.FileStartLine = editableAddEditFileFormat.FileStartLine;
            editingFileFormat.ColumnForClientDebtorNumber = editableAddEditFileFormat.ColumnForClientDebtorNumber;
            editingFileFormat.UpdateDate = editableAddEditFileFormat.UpdatedDate;
            editingFileFormat.UpdatedBy = editableAddEditFileFormat.UpdatedBy;
            if (EditMode)
            {
                editingFileFormat.UpdateDate = DateTime.Now;
                editingFileFormat.CreateDate = editableAddEditFileFormat.CreateDate;
                editingFileFormat.UpdatedBy = Environment.UserName;
                if(await _db.UpdateObjectAsync(editingFileFormat) > 0)
                {
                    Completed($"File Format: {editingFileFormat.FileFormatName} has been updated.");
                }
                
            } 
            else
            {
                editingFileFormat.CreateDate = DateTime.Now;
                editingFileFormat.UpdatedBy = Environment.UserName;
                if(await _db.AddFileFormatAsync(editingFileFormat) > 0)
                {
                    Completed($"File Format: {editingFileFormat.FileFormatName} has been Added.");
                }
            }
        }

        private async Task<List<FileFormatDetail>> GetFileFormatDetails(long id) => await _db.GetAllFileFormatDetailsByFileFormatIdAsync(id);

        public async void GetFieldsAndColumns()
        {
            Fields = new ObservableCollection<string>(await _rando.SetDebtorPropertiesAsync());
            ColumnTypes = new ObservableCollection<string>(await _rando.GetColumnTypesAsync());
        }
        
        private async void OnDeleteFileFormatDetail(FileFormatDetail detail)
        {
            if(await _db.DeleteObjectAsync(detail) > 0)
            {
                EditableAddEditFileFormat.FileFormatDetails = await _db.GetAllFileFormatDetailsByFileFormatIdAsync(_editingFileFormat.Id);
            }
        }

        #endregion
    }
}
