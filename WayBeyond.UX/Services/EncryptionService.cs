using System;
using System.Configuration;
using System.Runtime.InteropServices;
using System.Security;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;

namespace WayBeyond.UX.Services
{
    public class EncryptionService
    {
        private ServiceCollection _service;
        private IServiceProvider _serviceProvider;
        public EncryptionService()
        {
            _service = new ServiceCollection();
            _service.AddDataProtection();
            _serviceProvider = _service.BuildServiceProvider();

        }

        public string ProtectData(string value)
        {
            var protector = _serviceProvider.GetDataProtector(AppConstants.ProtectData);
            return protector.Protect(value);
        }

        public string UnProtectData(string value)
        {
            var protector = _serviceProvider.GetDataProtector(AppConstants.ProtectData);
            return protector.Unprotect(value);
        }
        public string SecureStringToString(SecureString value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));

            IntPtr unmanagedString = IntPtr.Zero;

            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(value);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
}