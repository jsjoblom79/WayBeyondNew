using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Lifetime;
using WayBeyond.UX.Services;

namespace WayBeyond.UX
{
    public class ContainerHelper
    {
        private static IUnityContainer _container;

        static ContainerHelper()
        {
            _container = new UnityContainer();
            _container.RegisterType<IBeyondRepository, BeyondRepository>(
                new ContainerControlledLifetimeManager()).
                RegisterType<IRando, Rando>(
                new ContainerControlledLifetimeManager()).
                RegisterType<ITransfer, Transfer>(
                new ContainerControlledLifetimeManager()).
                RegisterType<IClientProcess, ClientProcess>(
                new ContainerControlledLifetimeManager()).
                RegisterType<IEpicClientProcess, NorLeaClientProcess>(
                new ContainerControlledLifetimeManager()).
                RegisterType<ITTRepo, TTRepo>(
                new ContainerControlledLifetimeManager());
            
                
        }

        public static IUnityContainer Container { get { return _container; } }
    }
}
