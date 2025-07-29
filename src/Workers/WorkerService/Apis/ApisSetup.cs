using Microsoft.Extensions.Options;
using Refit;
using WorkerService.Options;

namespace WorkerService.Apis;

public static class ApisSetup
{
    public static void AddApisSetup(this IHostApplicationBuilder builder)
    {
        builder.Services.AddRefitClient<IContactApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ApiClientsOptions>>().Value;
                client.BaseAddress = new Uri(opts.ContactsApi.BaseUrl);
            });

        builder.Services.AddRefitClient<IContactReportApi>()
            .ConfigureHttpClient((sp, client) =>
            {
                var opts = sp.GetRequiredService<IOptions<ApiClientsOptions>>().Value;
                client.BaseAddress = new Uri(opts.ContactReportsApi.BaseUrl);
            });
    }
}