using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.File.Drops.Drop
{
    public class EditableFileFormatDetail: ValidatableBindableBase
    {


        private string? _field;

        public string? Field
        {
            get { return _field; }
            set { SetProperty(ref _field, value); }
        }

        private string? _fileColumn;

        public string? FileColumn
        {
            get { return _fileColumn; }
            set { SetProperty(ref _fileColumn, value); }
        }

        private string? _columnType;

        public string? ColumnType
        {
            get { return _columnType; }
            set { SetProperty(ref _columnType, value); }
        }

        private SpecialCase? _specialCase;

        public SpecialCase? SpecialCase
        {
            get { return _specialCase; }
            set { SetProperty(ref _specialCase, value); }
        }

        private long _fileFormatId;

        public long FileFormatId
        {
            get { return _fileFormatId; }
            set { SetProperty(ref _fileFormatId, value); }
        }

    }
}
