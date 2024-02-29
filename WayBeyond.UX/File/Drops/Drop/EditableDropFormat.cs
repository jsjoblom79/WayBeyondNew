using System;
using System.Collections;
using System.Collections.Generic;
using WayBeyond.Data.Models;

namespace WayBeyond.UX.File.Drops.Drop
{
    public class EditableDropFormat : ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private long? _dropId;

        public long? DropId
        {
            get { return _dropId; }
            set { SetProperty(ref _dropId, value); }
        }

        private string? _dropName;

        public string? DropName
        {
            get { return _dropName; }
            set { SetProperty(ref _dropName, value); }
        }

        private DateTime? _createDate;

        public DateTime? CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        private DateTime? _updateDate;

        public DateTime? UpdateDate
        {
            get { return _updateDate; }
            set { SetProperty(ref _updateDate, value); }
        }

        private string? _updatedBy;

        public string? UpdatedBy
        {
            get { return _updatedBy; }
            set { SetProperty(ref _updatedBy, value); }
        }

        private List<Client>? _clients;

        public List<Client>? Clients
        {
            get { return _clients; }
            set { SetProperty(ref _clients, value); }
        }

        private List<DropFormatDetail>? _dropFormatDetails;

        public List<DropFormatDetail>? DropFormatDetails
        {
            get { return _dropFormatDetails; }
            set { SetProperty(ref _dropFormatDetails, value); }
        }


    }
}