// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

/// <summary>
/// Contains the design and actual values of a parameter.
/// </summary>
public class UnitValueParameterJson : IUnitValueParameterJson
{
    /// <summary>
    /// The design value of the parameter.
    /// </summary>
    public UnitValueJson? Design { get; set; }

    /// <summary>
    /// The actual value of the parameter.
    /// </summary>
    public UnitValueJson? Actual { get; set; }
}
