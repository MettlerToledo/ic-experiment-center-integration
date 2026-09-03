// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Turns on the final control element with the specified value.
/// </summary>
public class ControlFinalControlElementOnParametersJson : ControlFinalControlElementParametersJson
{
    /// <summary>
    /// The value to control the final control element to.
    /// </summary>
    public UnitValueParameterJson Value { get; set; } = new UnitValueParameterJson();
}
