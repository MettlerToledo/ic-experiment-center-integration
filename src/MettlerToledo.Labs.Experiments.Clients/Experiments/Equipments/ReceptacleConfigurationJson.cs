// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Provides information about the receptacle configuration.
/// </summary>
public class ReceptacleConfigurationJson
{
    /// <summary>
    /// The name of the receptacle type.
    /// </summary>
    public string? ReceptacleName { get; set; }

    /// <summary>
    /// The names of the reactor vessel types being used in this receptacle
    /// </summary>
    public IList<string> ReactorVesselNames { get; set; } = new List<string>();

    /// <summary>
    /// Information about the inserts in the vessels.
    /// </summary>
    public IList<InsertReferenceJson> Inserts { get; set; } = new List<InsertReferenceJson>();
}
