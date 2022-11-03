using Microsoft.AspNetCore.Hosting;

namespace BasicBilling.API.Extensions
{
    public static class EnvironmentExtensions
    {
        public static bool IsMockServer(this IWebHostEnvironment environment)
        {
            return environment.EnvironmentName.ToLower() == "MockServer".ToLower();
        }
    }
}
