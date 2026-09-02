// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Equipments;

/// <summary>
/// Information for a the zone of an alr instrument used in an experiment.
/// </summary>
public class AlrInstrumentZoneJson : InstrumentZoneJson
{
    /// <summary>
    /// The configuration of the receptacle
    /// </summary>
    public ReceptacleConfigurationJson Receptacle { get; set; } = new ReceptacleConfigurationJson();
}
