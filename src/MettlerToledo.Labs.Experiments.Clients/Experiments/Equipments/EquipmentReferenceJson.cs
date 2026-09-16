// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

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
