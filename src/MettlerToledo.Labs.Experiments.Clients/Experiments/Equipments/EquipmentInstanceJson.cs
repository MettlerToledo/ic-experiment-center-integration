// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Equipments;

/// <summary>
/// Refers to a specific identified instance of equipment.
/// </summary>
public class EquipmentInstanceJson : EquipmentJson
{
    /// <summary>
    /// Asset tag assigned by the user.
    /// </summary>
    public string AssetTag { get; set; } = string.Empty;

    /// <summary>
    /// A description of the instrument.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
