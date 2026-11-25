// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// Url paths, header names, and media types for the iC Experiment Center public experiments API.
/// </summary>
public static class PublicApiUrlConstants
{
    /// <summary>
    /// The name of the header that carries the public API key.
    /// </summary>
    public const string ApiKeyHeaderName = "X-API-KEY";

    /// <summary>
    /// The media type of a server-sent events stream.
    /// </summary>
    public const string EventStreamMediaType = "text/event-stream";

    /// <summary>
    /// The "api" root path segment.
    /// </summary>
    public const string ApiName = "api";

    /// <summary>
    /// The "public" path segment.
    /// </summary>
    public const string PublicName = "public";

    /// <summary>
    /// The API version path segment.
    /// </summary>
    public const string VersionName = "v1";

    /// <summary>
    /// The "experiments" path segment.
    /// </summary>
    public const string ExperimentsName = "experiments";

    /// <summary>
    /// The "events" path segment.
    /// </summary>
    public const string EventsName = "events";

    /// <summary>
    /// The "state-changes" path segment.
    /// </summary>
    public const string StateChangesName = "state-changes";

    /// <summary>
    /// The "report" path segment.
    /// </summary>
    public const string ReportName = "report";

    /// <summary>
    /// The name of the query parameter that carries the requested report format.
    /// </summary>
    public const string FormatQueryParameterName = "format";

    /// <summary>
    /// The path to the versioned public API root.
    /// </summary>
    public const string PublicApiPath = $"{ApiName}/{PublicName}/{VersionName}";

    /// <summary>
    /// The path to the public experiments api.
    /// </summary>
    public const string PublicExperimentsPath = $"{PublicApiPath}/{ExperimentsName}";

    /// <summary>
    /// The path to the experiment state change stream
    /// </summary>
    public const string ExperimentStateChangesPath = $"{PublicExperimentsPath}/{EventsName}/{StateChangesName}";
}
