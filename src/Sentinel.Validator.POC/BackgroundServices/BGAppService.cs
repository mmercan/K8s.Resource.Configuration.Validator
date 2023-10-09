using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Sentinel.Validator.POC.Repo;

namespace Sentinel.Validator.POC.BackgroundServices;
public class BGAppService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<BGAppService> _logger;
    private readonly string _k8sType;
    private readonly ValidationRepo _validationRepo;

    public BGAppService(IServiceProvider services, String k8sType)
    {
        _services = services;
        _logger = services.GetService<ILogger<BGAppService>>();
        _k8sType = k8sType;
        //  _validationRepo = validationRepo;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(_k8sType +
            " Consume Scoped Service Hosted Service running.");

        await DoWork(stoppingToken);
    }

    private async Task DoWork(CancellationToken stoppingToken)
    {
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "Consume Scoped Service Hosted Service is stopping.");

        await base.StopAsync(stoppingToken);
    }

}