// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

/// <summary>
/// Defines the properties of a unit value parameter so that see <see cref="DynamicUnitValueParameterJson"/> and
/// <see cref="UnitValueParameterJson"/> can be treaded the same when they are used as a parameter in an operation.
/// </summary>
public interface IUnitValueParameterJson
{
    /// <summary>
    /// The design value of the parameter.
    /// </summary>
    UnitValueJson? Design { get; set; }

    /// <summary>
    /// The actual value of the parameter.
    /// </summary>
    UnitValueJson? Actual { get; set; }
}
