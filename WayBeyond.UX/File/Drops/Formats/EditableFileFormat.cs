using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.File.Drops.Formats
{
    public class EditableFileFormat : ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string? _fileFormatName;

        public string? FileFormatName
        {
            get { return _fileFormatName; }
            set { SetProperty(ref _fileFormatName, value); }
        }

        private DateTime? _createDate;

        public DateTime? CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        private DateTime? _updatedDate;

        public DateTime? UpdatedDate
        {
            get { return _updatedDate; }
            set { SetProperty(ref _updatedDate, value); }
        }

        private string? _updatedBy;

        public string? UpdatedBy
        {
            get { return _updatedBy; }
            set { SetProperty(ref _updatedBy, value); }
        }

        private List<FileFormatDetail> _fileFormatDetails;

        public List<FileFormatDetail> FileFormatDetails
        {
            get { return _fileFormatDetails; }
            set { SetProperty(ref _fileFormatDetails, value); }
        }


        private int? _fileStartLine;

        public int? FileStartLine
        {
            get => _fileStartLine;
            set => SetProperty(ref _fileStartLine, value);
        }


        private string? _columnForClientDebtorNumber;

        public string? ColumnForClientDebtorNumber
        {
            get => _columnForClientDebtorNumber;
            set => SetProperty(ref _columnForClientDebtorNumber, value);
        }


    }
}
