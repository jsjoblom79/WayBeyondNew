using WayBeyond.Data.Models;

namespace WayBeyond.UX.File.Maintenance
{
    public class EditableClient : ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private long? _clientId;

        public long? ClientId
        {
            get { return _clientId; }
            set { SetProperty(ref _clientId, value); }
        }

        private string? _clientName;

        public string? ClientName
        {
            get { return _clientName; }
            set { SetProperty(ref _clientName, value); }
        }

        private long? _dropNumber;

        public long? DropNumber
        {
            get { return _dropNumber; }
            set { SetProperty(ref _dropNumber, value); }
        }

        private string? _dropFileName;

        public string? DropFileName
        {
            get { return _dropFileName; }
            set { SetProperty(ref _dropFileName, value); }
        }

        private string? _assemblyName;

        public string? AssemblyName
        {
            get { return _assemblyName; }
            set { SetProperty(ref _assemblyName, value); }
        }

        private long? _dropFormatId;

        public long? DropFormatId
        {
            get { return _dropFormatId; }
            set { SetProperty(ref _dropFormatId, value); }
        }

        private long? _fileFormatId;

        public long? FileFormatId
        {
            get { return _fileFormatId; }
            set { SetProperty(ref _fileFormatId, value); }
        }


        private DropFormat _dropFormat;

        public DropFormat DropFormat
        {
            get { return _dropFormat; }
            set { SetProperty(ref _dropFormat, value);
                if(value != null) DropFormatId = value.Id;
                    }
        }

        private FileFormat _fileFormat;

        public FileFormat FileFormat
        {
            get { return _fileFormat; }
            set { SetProperty(ref _fileFormat, value); 
                if(value != null) FileFormatId = value.Id;
            }
        }



    }

}