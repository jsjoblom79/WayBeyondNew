using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.File.Remote
{
    public class NewEditableConnection : ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string? _host;

        public string? Host
        {
            get { return _host; }
            set { SetProperty(ref _host, value); }
        }

        private long? _port;

        public long? Port
        {
            get => _port;
            set => SetProperty(ref _port, value);
        }

        private string? _username;

        public string? Username
        {
            get { return _username; }
            set { SetProperty(ref _username, value); }
        }

        private string? _password;

        public string? Password
        {
            get { return _password; }
            set { SetProperty(ref _password, value); }
        }

        private long? _fingerprintRequired;

        public long? FingerprintRequired
        {
            get { return _fingerprintRequired; }
            set { SetProperty(ref _fingerprintRequired, value); }
        }

        private string? _fingerprint;

        public string? Fingerprint
        {
            get { return _fingerprint; }
            set { SetProperty(ref _fingerprint, value); }
        }

        private string? _name;

        public string? Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        private DateTime? _createDate;

        public DateTime? CreateDate
        {
            get { return _createDate; }
            set { SetProperty(ref _createDate, value); }
        }

        private DateTime _updateDate;

        public DateTime UpdateDate
        {
            get { return _updateDate; }
            set { SetProperty(ref _updateDate, value); }
        }
        
    }
}
