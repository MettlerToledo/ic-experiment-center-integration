// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// The parameters for a shake operation.
/// </summary>
public class ShakeParametersJson : AgitateParametersJson
{
    /// <summary>
    /// The shaking speed.
    /// </summary>
    public UnitValueParameterJson? Speed { get; set; }
}
