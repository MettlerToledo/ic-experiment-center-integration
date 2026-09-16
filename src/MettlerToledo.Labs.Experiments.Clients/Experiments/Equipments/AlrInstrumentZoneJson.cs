// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

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
