// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Units;

namespace AutoChem.Lab.S88Definitions.Parameters;

/// <summary>
/// A dynamic unit value parameter.
/// Note the unit for the parameter and the limits for the parameter will come from the configuration 
/// in the system. This will be based on the OperationType and Internal Name of the parameter.
/// </summary>
public class DynamicUnitValueParameterJson : DynamicParameterJson, IUnitValueParameterJson
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
