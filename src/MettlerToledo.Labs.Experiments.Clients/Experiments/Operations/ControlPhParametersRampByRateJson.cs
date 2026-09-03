// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Defines the parameters for ramping the pH to a target value by rate.
/// </summary>
public class ControlPhParametersRampByRateJson : ControlPhParametersActiveJson
{
    /// <summary>
    /// The pH value that the operation tries to control to.
    /// </summary>
    public UnitValueParameterJson Ph { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The rate for the pH control.
    /// </summary>
    public UnitValueParameterJson Rate { get; set; } = new UnitValueParameterJson();
}
