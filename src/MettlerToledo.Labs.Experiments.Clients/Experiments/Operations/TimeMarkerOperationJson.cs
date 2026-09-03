// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// An operation creating a time marker.
/// </summary>
public class TimeMarkerOperationJson : OperationJson
{
    /// <summary>
    /// The name of the time marker.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The time associated with the marker.
    /// </summary>
    public DateTime? MarkedTime { get; set; }
}
