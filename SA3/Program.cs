using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SA3.Core.Interfaces;
using SA3.DAL.Json;
using SA3.BLL.Services;
using SA3.BLL.Mapping;
using SA3.UI;

namespace SA3.UI
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.InputEncoding = System.Text.Encoding.UTF8;

            var serviceProvider = ConfigureServices();

            var appUI = serviceProvider.GetRequiredService<BlogConsoleUI>();
            appUI.Run();
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();
            
            services.AddLogging();
            services.AddAutoMapper(cfg => { cfg.AddProfile<BlogProfile>(); });
            
            services.AddSingleton<IUnitOfWork, JsonUnitOfWork>();
            
            services.AddTransient<BlogService>();
            services.AddTransient<BlogConsoleUI>();

            return services.BuildServiceProvider();
        }
    }
}