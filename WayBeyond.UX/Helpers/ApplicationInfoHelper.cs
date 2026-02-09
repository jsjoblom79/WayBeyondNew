using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace WayBeyond.UX.Helpers
{
    public class ApplicationInfoHelper
    {
        public Assembly AppAssembly { get; set; }
        public string? Version { get; set; }
        public string? InfoVersion { get; set; }
        public ApplicationInfoHelper() {
            AppAssembly = typeof(App).Assembly;

            Version = AppAssembly.GetName().Version?.ToString();

            InfoVersion = AppAssembly
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
        }

        public override string ToString()
        {
            return $"{Version}";
        }
    }
}
