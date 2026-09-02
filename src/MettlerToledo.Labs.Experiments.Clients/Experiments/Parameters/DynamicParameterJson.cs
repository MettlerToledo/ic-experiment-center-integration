// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;
using AutoChem.Lab.S88Definitions.Operations;

namespace AutoChem.Lab.S88Definitions.Parameters;

/// <summary>
/// Defines a dynamic parameter use in a <see cref="DynamicOperationJson"/>. The specific parameters will be defined by the user and are not known by the system.
/// </summary>
[JsonDerivedType(typeof(DynamicUnitValueParameterJson), "UnitValue")]
[JsonDerivedType(typeof(DynamicStringParameterJson), "String")]
[JsonDerivedType(typeof(DynamicMaterialReferenceParameterJson), "MaterialReference")]
[JsonDerivedType(typeof(DynamicBooleanParameterJson), "Boolean")]
public class DynamicParameterJson
{
    /// <summary>
    /// The name of the operation displayed to the user when referring to this parameter.
    /// </summary>
    public string? DisplayName { get; set; }

    /// <summary>
    /// The internal name of the parameter used by the system.
    /// It is expected that the user will use the display name when referring to the parameter, and 
    /// internal name will be use for matching the parameters in the system.
    /// </summary>
    public string InternalName { get; set; } = string.Empty;
}
