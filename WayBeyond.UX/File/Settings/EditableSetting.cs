using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.UX;

namespace WayBeyond4.UX.File.Settings
{
    public class EditableSetting: ValidatableBindableBase
    {

        private long _id;

        public long Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }


        private string _key;

        public string Key
        {
            get { return _key; }
            set { SetProperty(ref _key, value); }
        }


        private string _value;

        public string Value
        {
            get { return _value; }
            set { SetProperty(ref _value, value); }
        }


    }
}
