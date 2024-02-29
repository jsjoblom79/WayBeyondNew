using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.File.Drops.Drop
{
    public class EditableDropDetailFormat: ValidatableBindableBase
    {
        private string? _detailField;
        public string? DetailField
        {
            get { return _detailField; }
            set { SetProperty(ref _detailField, value); }
        }

        private long? _detailPosition;
        public long? DetailPosition
        {
            get { return _detailPosition; }
            set { SetProperty(ref _detailPosition, value); }
        }


        private long? _detailFieldType;
        public long? DetailFieldType
        {
            get { return _detailFieldType; }
            set { SetProperty(ref _detailFieldType, value); }
        }

        private long _dropFormatId;
        [Required]
        public long DropFormatId
        {
            get { return _dropFormatId; }
            set { SetProperty(ref _dropFormatId, value); }
        }

    }
}
