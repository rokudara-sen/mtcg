using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MTCG._00_Server;
using MTCG._01_Presentation_Layer.Endpoints;
using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._02_Business_Logic_Layer.Services;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.Repositories;
using MTCG._03_Data_Access_Layer.Services;

namespace MTCG;

class Program
{
    public static async Task Main(string[] args)
    {
        var serviceCollection = new ServiceCollection();
        var connectionString = "Host=localhost;Username=admin;Password=admin;Database=mtcgdatabase";

        // IDataContext
        serviceCollection.AddSingleton<IDataContext>(provider => new DataContext(connectionString));

        // Repositories
        serviceCollection.AddTransient<ICardRepository, CardRepository>();
        serviceCollection.AddTransient<IPackageRepository, PackageRepository>();
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        serviceCollection.AddTransient<IStackRepository, StackRepository>();

        // Route Handlers
        serviceCollection.AddTransient<CardRouteHandler>();
        serviceCollection.AddTransient<PackageRouteHandler>();
        serviceCollection.AddTransient<UserRouteHandler>();

        // Endpoints
        serviceCollection.AddTransient<IEndpoint, PackageEndpoint>();
        serviceCollection.AddTransient<IEndpoint, UserEndpoint>();
        
        // Services
        serviceCollection.AddTransient<PackageAcquisitionService>();

        var serviceProvider = serviceCollection.BuildServiceProvider();
        var endpoints = serviceProvider.GetServices<IEndpoint>().ToList();
        var router = new Router(endpoints);
        var httpProcessor = new HttpProcessor(router);
        var server = new TcpServer(IPAddress.Any, 10001, httpProcessor);
        await server.StartAsync();
    }
}
