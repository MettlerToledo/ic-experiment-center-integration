// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Information about an instrument.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class InstrumentJson : EquipmentInstanceJson
{
    /// <summary>
    /// The zones of the instrument.
    /// </summary>
    public IList<InstrumentZoneJson> Zones { get; } = new List<InstrumentZoneJson>();

    /// <summary>
    /// The software version on the instrument.
    /// </summary>
    public string? SoftwareVersion { get; set; }
}
