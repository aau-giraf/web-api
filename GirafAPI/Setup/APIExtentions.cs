using Microsoft.Extensions.Hosting;
using System;

namespace GirafAPI.Setup;

public static class APIExtentions
{
    public static bool IsLocalDocker(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);
            
        return hostEnvironment.IsEnvironment("LocalDocker");
    }

    public static bool IsCI(this IHostEnvironment hostEnvironment)
    {
        ArgumentNullException.ThrowIfNull(hostEnvironment);

        return hostEnvironment.IsEnvironment("CI");
    }
}