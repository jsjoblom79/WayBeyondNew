using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WayBeyond.Data.Models;
using Serilog;
using System.Reflection;

namespace WayBeyond.UX.Services
{
    public class ClientProcessLocator
    {

        public static async Task<bool> LocateProcess(FileObject file, Client client)
        {
			try
			{
                var typ = Assembly.GetAssembly(typeof(ClientProcessLocator)).GetTypes();
				var process = Assembly.GetAssembly(typeof(ClientProcessLocator))
					.GetTypes()
					.FirstOrDefault(a => a.Name == client.AssemblyName && a.Namespace == "WayBeyond.UX.Services");

                if (process == null)
                {
                    Log.Error($"{client.AssemblyName} was not found in the list of processes. Please check the Client Maintenance and try again.");
                    return false;
                }
                else
                {
                    var processInstance = Activator.CreateInstance(process);

                    return await ((IEpicClientProcess)processInstance).ProcessEpicClientAsync(file, client);

                }
            }
			catch (Exception ex)
			{

                return false;
			}
        }
    }
}
