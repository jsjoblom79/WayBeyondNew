using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.File.Location
{
    public class EditableFileLocation: ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _fileLocationName;

        public string FileLocationName
        {
            get { return _fileLocationName; }
            set { SetProperty(ref _fileLocationName, value); }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set { SetProperty(ref _path, value); }
        }

        private string _fileType;

        public string FileType
        {
            get { return _fileType; }
            set { SetProperty(ref _fileType, value); }
        }

        private long? _remoteConnectionId;

        public long? RemoteConnectionId
        {
            get { return _remoteConnectionId; }
            set { SetProperty(ref _remoteConnectionId, value); }
        }


    }
}
