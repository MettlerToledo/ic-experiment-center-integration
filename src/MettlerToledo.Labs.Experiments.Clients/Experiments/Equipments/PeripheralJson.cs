// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Identifies and describes a peripheral used in an experiment.
/// </summary>
public class PeripheralJson : EquipmentJson
{
    /// <summary>
    /// An identifier for the peripheral in the context of the instrument it is attached to.
    /// </summary>
    public string Identifier { get; set; } = string.Empty;

    /// <summary>
    /// The path describing how the peripheral is connected to the instrument,
    /// starting from the instrument and including all intermediate equipment (e.g. ECBs) and ports (e.g. control ports, sensor ports) that are part of the connection.
    /// In iC Experiment Center, this is similar to the the identifiers in a DeviceAddress.
    /// </summary>
    public List<PathPartJson> Path { get; set; } = new List<PathPartJson>();

    /// <summary>
    /// The name assigned to the peripheral by the user.
    /// </summary>
    public string AssignedName { get; set; } = string.Empty;

    /// <summary>
    /// The type of signal the peripheral uses to communicate with the instrument.
    /// Not all peripherals will have a signal type.
    /// </summary>
    public string SignalType { get; set; } = string.Empty;

    /// <summary>
    /// The primary unit associated with the peripheral from an experiment / operational view, if any.
    /// </summary>
    public string? Unit { get; set; } = string.Empty;

    /// <summary>
    /// The material portion currently associated with the peripheral, if any.
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }
}
