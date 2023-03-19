using azure_function_net7_demo.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddCosmosRepository(builder => builder.ContainerBuilder.Configure<Customer>(opt => opt.WithServerlessThroughput()));
    }).Build();

host.Run();
