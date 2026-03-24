using Microsoft.Extensions.DependencyInjection;

namespace MatteoAPP1.Services
{
    public static class ServiceHelper
    {
        public static T GetService<T>() where T : notnull
        {
            var services = IPlatformApplication.Current?.Services;
            if (services is null)
            {
                throw new InvalidOperationException("Le conteneur de services MAUI n'est pas disponible.");
            }

            return services.GetRequiredService<T>();
        }
    }
}
