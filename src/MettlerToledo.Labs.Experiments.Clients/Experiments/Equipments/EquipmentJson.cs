// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Information about equipment used
/// </summary>
[JsonDerivedType(typeof(AlrJson), "Alr")]
public class EquipmentJson : BaseTypeJson
{
    /// <summary>
    /// The category or class of the equipment.
    /// </summary>
    public EquipmentCategoryJson Category { get; set; }

    /// <summary>
    /// The model of the equipment.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// The serial number of the instrument.
    /// </summary>
    public string SerialNumber { get; set; } = string.Empty;
}
