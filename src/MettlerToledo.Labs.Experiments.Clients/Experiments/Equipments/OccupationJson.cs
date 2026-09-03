// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Information about the occupation of an instrument.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class OccupationJson : BaseTypeJson
{
    /// <summary>
    /// The start time in UTC of the stage.
    /// </summary>
    public DateTime? StartTimeUtc { get; set; }

    /// <summary>
    /// The completion time in UTC of the stage.
    /// </summary>
    public DateTime? ReleaseTimeUtc { get; set; }

    /// <summary>
    /// The trends of the occupation.
    /// </summary>
    public IList<TrendJson> Trends { get; } = new List<TrendJson>();
}
