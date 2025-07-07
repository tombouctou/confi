using Confi.Manager;
using Nist;
using Persic;

namespace Confi;

public static class MainHelper
{
    public static IEndpointRouteBuilder MapConfiManager(this IEndpointRouteBuilder endpoints) 
    {
        endpoints.MapNodes();
        endpoints.MapApps();
        endpoints.MapConfiguration();

        return endpoints;
    }

    public static MongoRegistrationBuilder AddConfiManagerCollections(this MongoRegistrationBuilder builder)
    {
        return builder
            .AddNodeCollection()
            .AddCollection<SchemeRecord>("schemas")
            .AddCollection<ConfigurationRecord>("configs")
            .AddCollection<AppRecord>("apps");
    }

    public static Error? ToConfiManagerError(this Exception exception)
    {
        return NodeHelper.MapNodesErrors(exception)
            ?? AppEndpoints.MapAppErrors(exception)
            ?? null;
    }
}