// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.


using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Wait by duration
/// </summary>
public class WaitParametersByDuration : WaitParametersJson
{
    /// <summary>
    /// The duration of the wait operation
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
