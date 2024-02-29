using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using WayBeyond.UX.Services;
using WayBeyond4.UX.File.Settings;

namespace WayBeyond.UX.File.Settings
{
    public class AddEditSettingViewModel : BindableBase
    {
        private IBeyondRepository _db;
        public AddEditSettingViewModel(IBeyondRepository db)
        {
            _db = db;

            CancelCommand = new RelayCommand(OnCancelCommand);
            AddEditSaveCommand = new RelayCommand(OnAddEditSaveCommand, CanSave);
        }

        #region Properties

        private bool _editMode;

        public bool EditMode
        {
            get { return _editMode; }
            set { SetProperty(ref _editMode, value); }
        }

        private Setting _editingSetting;
        private EditableSetting _editableSetting;

        public EditableSetting NewEditableSetting
        {
            get { return _editableSetting; }
            set { SetProperty(ref _editableSetting, value); }
        }

        #endregion

        #region Method
        public RelayCommand AddEditSaveCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public event Action<string> Completed;

        private bool CanSave()
        {
            return !NewEditableSetting.HasErrors;
        }
        private void OnCancelCommand()
        {
            Completed("Add/Edit Setting Cancelled.");
        }

        private void OnAddEditSaveCommand()
        {
            UpdateSetting(NewEditableSetting, _editingSetting);
        }

        private void UpdateSetting(EditableSetting editableSetting, Setting editingSetting)
        {
            if(EditMode)
            {
                editingSetting.Key = editableSetting.Key;
                editingSetting.Value = editableSetting.Value;
                _db.UpdateObjectAsync(editingSetting);
                Completed($"Setting: {editingSetting.Key} has been updated.");
            }
            else
            {
                editingSetting.Key = editableSetting.Key;
                editingSetting.Value = editableSetting.Value;
                _db.AddSettingsAsync(editingSetting);
                Completed($"Setting: {editingSetting.Key} has been added.");
            }
            
        }

        public async void SetSetting(Setting setting)
        {
            _editingSetting = setting;
            if (NewEditableSetting != null) NewEditableSetting.ErrorsChanged -= RaiseCanExecuteChanged;
        
            NewEditableSetting = new EditableSetting();
            NewEditableSetting.ErrorsChanged += RaiseCanExecuteChanged;
            CopyEditingSetting(_editingSetting, NewEditableSetting);
        
                
        }

        private void CopyEditingSetting(Setting editingSetting, EditableSetting editableSetting)
        {
            editableSetting.Id = editingSetting.Id;
            if (EditMode)
            {
                editableSetting.Key = editingSetting.Key;
                editableSetting.Value = editingSetting.Value;
            }
        }

        private void RaiseCanExecuteChanged(object? sender, DataErrorsChangedEventArgs e)
        {
            AddEditSaveCommand.RaiseCanExecuteChanged();
        }
        #endregion
    }
}
