// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Materials;

namespace AutoChem.Lab.S88Definitions.Parameters;

/// <summary>
/// Represents a parameter that references a material used or produced in a dynamic operation.
/// </summary>
public class DynamicMaterialReferenceParameterJson : DynamicParameterJson
{
    /// <summary>
    /// A material used or resulting from a dynamic operation.
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }

    /// <summary>
    /// The usage of the material portion in the operation.
    /// </summary>
    public MaterialPortionUsageJson MaterialPortionUsage { get; set; } = MaterialPortionUsageJson.Input;
}
