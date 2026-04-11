using Microsoft.Extensions.Logging;
using MatteoAPP1.Services;
using CommunityToolkit.Maui;
using MatteoAPP1.ViewModels;

namespace MatteoAPP1
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitMediaElement()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton(new HttpClient
            {
                BaseAddress = new Uri("https://www.demonslayer-api.com/api/v1/")
            });
            builder.Services.AddSingleton<IAppDatabaseService, AppDatabaseService>();
            builder.Services.AddSingleton<IDemonSlayerApiService, DemonSlayerApiService>();
            builder.Services.AddSingleton<ICharacterCatalogService, CharacterCatalogService>();
            builder.Services.AddSingleton<CharactersViewModel>();
            builder.Services.AddTransient<CharacterDetailViewModel>();
            builder.Services.AddTransient<AddCharacterViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
