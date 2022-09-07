using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tenpad.Core.Factory;
using Tenpad.Core.Services;
using Tenpad.Database;

namespace Tenpad.Core
{
    public class IoC
    {
        private static ServiceProvider _provider;

        public static void BuildServiceProvider()
        {
            var services = new ServiceCollection();

            services.AddSingleton<ITabViewModelFactory, TabViewModelFactory>();
            services.AddSingleton<IPageViewModelFactory, PageViewModelFactory>();
            services.AddSingleton<IFileSystemModelFactory, FileSystemModelFactory>();

            services.AddDbContext<TenpadDbContext>(options => options.UseSqlite("Data Source=tenpad.db"));
            
            services.AddScoped<IHistory, HistoryService>();

            services.AddTransient<MainViewModel>();
            services.AddTransient<ITabItemViewModel, DefaultTabViewModel>();

            services.AddTransient<IPageViewModel, HomePageViewModel>();
            services.AddTransient<IPageViewModel, DocumentPageViewModel>();
            services.AddTransient<IPageViewModel, BrowsePageViewModel>();

            _provider = services.BuildServiceProvider();

            _provider.GetService<TenpadDbContext>().Database.EnsureCreated();
        }

        public static T Resolve<T>() => _provider.GetRequiredService<T>();
    }
}
