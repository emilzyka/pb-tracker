using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using pb_tracker_api;
using pb_tracker_api.Abstractions;


var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        IConfiguration config = services
        .BuildServiceProvider()
        .GetService<IConfiguration>()
        .ToOption()
        .Expect("IConfiguration could not be found. Unrecoverable error.");

        services.AddServices();
        services.AddRepos();
        services.AddValidators();
        services.AddValidators();

    })
    .Build();

host.Run();
