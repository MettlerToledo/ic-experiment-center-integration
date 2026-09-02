// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for a release operation.
/// </summary>
public class ReleaseParametersDefinedJson : ReleaseParametersJson
{
    /// <summary>
    /// The jacket temperature to maintain.
    /// </summary>
    public UnitValueParameterJson Temperature { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The stirring speed.
    /// </summary>
    public UnitValueParameterJson StirringSpeed { get; set; } = new UnitValueParameterJson();
}
