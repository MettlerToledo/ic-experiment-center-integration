// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Defines the parameters for the <see cref="ControlPhOperationJson"/>
/// </summary>
public abstract class ControlPhParametersActiveJson : ControlPhParametersJson
{
    /// <summary>
    /// The type of pH dosing (acid or base)
    /// </summary>
    public PhDosingTypeJson DosingType { get; set; }

    /// <summary>
    /// A reference to the material used to control pH
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }

    /// <summary>
    /// The time to mix after dosing before the next measurement for pH control is taken and the next dosing decision is made.
    /// </summary>
    public UnitValueParameterJson MixingTime { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The minimum pH deviation required before taking action.
    /// </summary>
    public UnitValueParameterJson MinPhDeviation { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The rate of change of pH below which the pH is considered stable.
    /// The current pH must be stable before the next dosing action is taken.
    /// </summary>
    public UnitValueParameterJson PhStabilityRate { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The maximum quantity that can be charged for controling the pH.
    /// </summary>
    public UnitValueParameterJson MaxChargedQuantity { get; set; } = new UnitValueParameterJson();
}
