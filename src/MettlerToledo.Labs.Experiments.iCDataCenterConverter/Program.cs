// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;
using MettlerToledo.Labs.Experiments.iCDataCenterConverter.Exporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace MettlerToledo.Labs.Experiments.iCDataCenterConverter;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        if (args.Length != 3)
        {
            WriteUsage();
            return 1;
        }

        if (!Uri.TryCreate(EnsureTrailingSlash(args[0]), UriKind.Absolute, out var baseAddress))
        {
            await Console.Error.WriteLineAsync($"Invalid address '{args[0]}'. Expected an absolute URL, e.g. https://localhost:60479.");
            return 1;
        }

        var apiKey = args[1];

        if (!Enum.TryParse<ReportFileType>(args[2], ignoreCase: true, out var fileType) || !Enum.IsDefined(fileType))
        {
            await Console.Error.WriteLineAsync($"Invalid file type '{args[2]}'. Expected one of: {string.Join(", ", Enum.GetNames<ReportFileType>())}.");
            return 1;
        }

        var builder = Host.CreateApplicationBuilder();
        builder.Services.AddConcludedExperimentExport(baseAddress, apiKey, Directory.GetCurrentDirectory());

        using var host = builder.Build();

        using var cancellation = new CancellationTokenSource();
        Console.CancelKeyPress += (_, eventArgs) =>
        {
            eventArgs.Cancel = true;
            cancellation.Cancel();
        };

        var exporter = host.Services.GetRequiredService<IConcludedExperimentExporter>();
        try
        {
            await exporter.Run(fileType, cancellation.Token);
        }
        catch (HttpRequestException)
        {
            return 1;
        }

        return 0;
    }

    private static void WriteUsage()
    {
        Console.Error.WriteLine("Usage: <address> <apiKey> <fileType>");
        Console.Error.WriteLine("  <address>   Base URL of the iC Experiment Center, e.g. https://localhost:60479");
        Console.Error.WriteLine("  <apiKey>    A public API key with access to experiments");
        Console.Error.WriteLine($"  <fileType>  The report format to export, one of: {string.Join(", ", Enum.GetNames<ReportFileType>())}");
    }

    private static string EnsureTrailingSlash(string address) =>
        address.EndsWith('/') ? address : address + "/";
}
