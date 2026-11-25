// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// The payload of an experiment state change event.
/// </summary>
public class ExperimentStateChangeJson
{
    /// <summary>
    /// The id of the experiment whose state changed.
    /// </summary>
    public Guid ExperimentId { get; set; }

    /// <summary>
    /// The new state of the experiment.
    /// </summary>
    public ExperimentStateJson State { get; set; }

    /// <summary>
    /// The timestamp.
    /// </summary>
    public DateTime Timestamp { get; set; }
}
