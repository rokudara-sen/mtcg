using System.Net;
using Microsoft.Extensions.DependencyInjection;
using MTCG._00_Server;
using MTCG._01_Presentation_Layer.Endpoints;
using MTCG._01_Presentation_Layer.Interfaces;
using MTCG._02_Business_Logic_Layer.RouteHandlers;
using MTCG._02_Business_Logic_Layer.Services;
using MTCG._03_Data_Access_Layer.DataContext;
using MTCG._03_Data_Access_Layer.Interfaces;
using MTCG._03_Data_Access_Layer.Repositories;

namespace MTCG;

class Program
{
    public static async Task Main(string[] args)
    {
        // Create a new ServiceCollection
        var serviceCollection = new ServiceCollection();

        // Register configuration or settings if needed
        var connectionString = "Host=localhost;Username=admin;Password=admin;Database=mtcgdatabase";

        // Register IDataContext
        serviceCollection.AddSingleton<IDataContext>(provider => new DataContext(connectionString));

        // Register Repositories
        serviceCollection.AddTransient<ICardRepository, CardRepository>();
        serviceCollection.AddTransient<IPackageRepository, PackageRepository>();
        serviceCollection.AddTransient<IUserRepository, UserRepository>();
        // Register other repositories as needed

        // Register Route Handlers
        serviceCollection.AddTransient<CardRouteHandler>();
        serviceCollection.AddTransient<PackageRouteHandler>();
        serviceCollection.AddTransient<UserRouteHandler>();
        // Register other route handlers as needed

        // Register Endpoints
        serviceCollection.AddTransient<IEndpoint, PackageEndpoint>();
        serviceCollection.AddTransient<IEndpoint, UserEndpoint>();
        // Register other endpoints as needed

        // Build the ServiceProvider
        var serviceProvider = serviceCollection.BuildServiceProvider();

        // Resolve the required services
        var endpoints = serviceProvider.GetServices<IEndpoint>().ToList();

        // Create the Router with injected endpoints
        var router = new Router(endpoints);

        // Create the HttpProcessor with injected router
        var httpProcessor = new HttpProcessor(router);

        // Create the TcpServer with injected HttpProcessor
        var server = new TcpServer(IPAddress.Any, 10001, httpProcessor);

        // Start the server
        await server.StartAsync();
    }
}