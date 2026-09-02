// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Information about an observation.
/// </summary>
public class ObservationOperationJson : OperationJson
{
    /// <summary>
    /// The time of the observation.
    /// </summary>
    public DateTime? ObservationTime { get; set; }

    /// <summary>
    /// The text of the observation.
    /// </summary>
    public string Notes { get; set; } = string.Empty;
}
