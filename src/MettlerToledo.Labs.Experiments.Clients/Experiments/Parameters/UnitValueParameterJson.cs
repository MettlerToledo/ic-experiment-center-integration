// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Units;

namespace AutoChem.Lab.S88Definitions.Parameters;

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
