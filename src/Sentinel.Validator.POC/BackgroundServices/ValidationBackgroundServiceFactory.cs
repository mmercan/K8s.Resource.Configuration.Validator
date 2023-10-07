using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Sentinel.Validator.POC.Repo;

namespace Sentinel.Validator.POC.BackgroundServices
{
    public class ValidationBackgroundServiceFactory
    {
        public ValidationBackgroundServiceFactory(ILogger<ValidationBackgroundServiceFactory> logger, IServiceCollection Services, ValidationRepo validationRepo)
        {
            Assembly.GetExecutingAssembly());
            var types = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof(BackgroundService)));
            foreach (var type in types)
            {
                var service = Activator.CreateInstance(type, new object[] { Services.BuildServiceProvider(), logger, validationRepo });
                Services.AddSingleton(type, service);
            }
        }


        //Services.AddHostedServices()
    }


}
}