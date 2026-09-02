// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Equipments;

/// <summary>
/// Information about the zone of an instrument.
/// </summary>
[JsonDerivedType(typeof(AlrInstrumentZoneJson), "AlrInstrumentZone")]
public class InstrumentZoneJson
{
    /// <summary>
    /// The peripherals used with this zone for the experiment.
    /// </summary>
    public IList<PeripheralJson> Peripherals { get; set; } = new List<PeripheralJson>();

    /// <summary>
    /// The occupations of the zone.
    /// </summary>
    public IList<OccupationJson> Occupations { get; set; } = new List<OccupationJson>();
}
