// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;

namespace AutoChem.Lab.S88Definitions.Equipments;

/// <summary>
/// A reference to a piece of equipment
/// </summary>
public sealed class EquipmentReferenceJson
{
    /// <summary>
    /// The tracking Id of the equipment.
    /// </summary>
    public Guid TrackingId { get; set; }
}
