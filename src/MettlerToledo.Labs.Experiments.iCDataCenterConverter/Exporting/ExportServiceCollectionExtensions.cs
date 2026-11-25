// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using Microsoft.Extensions.DependencyInjection;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;

/// <summary>
/// Registers the concluded-experiment export services.
/// </summary>
public static class ExportServiceCollectionExtensions
{
    /// <summary>
    /// Registers the public API client and the export services.
    /// </summary>
    public static IServiceCollection AddConcludedExperimentExport(
        this IServiceCollection services,
        Uri baseAddress,
        string apiKey,
        string outputDirectory)
    {
        services.AddHttpClient<IExperimentsPublicApiClient, ExperimentsPublicApiClient>(httpClient =>
        {
            httpClient.BaseAddress = baseAddress;
            httpClient.DefaultRequestHeaders.Add(PublicApiUrlConstants.ApiKeyHeaderName, apiKey);
            httpClient.Timeout = Timeout.InfiniteTimeSpan;
        });

        services.AddOptions<ConcludedExperimentExportOptions>();
        services.AddSingleton<IReportFileWriter>(_ => new ReportFileWriter(outputDirectory));
        services.AddSingleton<IConcludedExperimentExporter, ConcludedExperimentExporter>();

        return services;
    }
}
