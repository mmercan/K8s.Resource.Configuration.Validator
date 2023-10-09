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
        public ValidationBackgroundServiceFactory(IServiceCollection services)
        {
            // IServiceProvider serviceProvider = services.BuildServiceProvider();

            ILoggerFactory loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            loggerFactory.CreateLogger<ValidationBackgroundServiceFactory>();



            // Assembly.GetExecutingAssembly());
            // var types = Assembly.GetExecutingAssembly().GetTypes().Where(p => p.IsSubclassOf(typeof(BackgroundService)));
            // foreach (var type in types)
            // {
            //     var service = Activator.CreateInstance(type, new object[] { Services.BuildServiceProvider(), logger, validationRepo });
            //     Services.AddSingleton(type, service);
            // }

            services.AddHostedService<BGAppService>((sp) => new BGAppService(sp, "Service1"));

            services.AddHostedService<BGAppService>((sp) => new BGAppService(sp, "PodV1"));


        }


        //Services.AddHostedServices()
    }


}
