using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestSharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using WpfAppApiClientNetCore.Models;
using WpfAppApiClientNetCore.ViewModels;

namespace WpfAppApiClientNetCore
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public IConfiguration Configuration { get; private set; }

        public App()
        {
            ServiceCollection services = new ServiceCollection();
            //Registering the services
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(ServiceCollection services)
        {
            
            //Registring the RestSharp
            services.AddTransient<IRestClient, RestClient>();
            services.AddTransient<IRestRequest, RestRequest>();

            //Registering Models
            services.AddSingleton<IWebRequestLib, WebRequestLib>();
            services.AddSingleton<IApiConsumerModel, ApiConsumerModel>();

            //Registering the Model View
            services.AddSingleton<IApiConsumerViewModel, ApiConsumerViewModel>();

            //Registering the Main Windows for view model locator
            services.AddTransient<MainWindow>();

            //Register and configure IConfiguration
            var builder = new ConfigurationBuilder()
                     .SetBasePath(Directory.GetCurrentDirectory())
                     .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
            services.AddSingleton<IConfiguration>(Configuration);

            //Registering the Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs.txt")
                .CreateLogger();
            services.AddSingleton<Serilog.ILogger>(Log.Logger);

        }

        private void OnStartup(object sender, StartupEventArgs e)
        {

            //Making the main window open on startup
            var mainWindow = ServiceProvider.GetService<MainWindow>();
            mainWindow.Show();
        }
    }
}
