// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;
using AutoChem.Lab.S88Definitions.Units;

namespace AutoChem.Lab.S88Definitions.Materials;

/// <summary>
/// Information about a material (e.g. chemical, substance, mixture)
/// </summary>
public sealed class MaterialPortionJson : BaseTypeJson
{
    /// <summary>
    /// The name of the material.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// The role of the material.
    /// </summary>
    public MaterialPortionRoleJson Role { get; set; }

    /// <summary>
    /// The lot number of the material.
    /// </summary>
    public string LotNumber { get; set; } = string.Empty;

    /// <summary>
    /// The density of the material.
    /// </summary>
    public UnitValueJson? Density { get; set; }

    /// <summary>
    /// The quantity for the material.
    /// </summary>
    public UnitValueParameterJson Quantity { get; set; } = new UnitValueParameterJson();
}
