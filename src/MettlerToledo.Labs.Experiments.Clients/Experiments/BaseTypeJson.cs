// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;

namespace AutoChem.Lab.S88Definitions;

/// <summary>
/// A base type for entities that a trackable.
/// </summary>
public abstract class BaseTypeJson
{
    /// <summary>
    /// A tracking ID that may have come from an external system.
    /// </summary>
    public Guid TrackingId { get; set; }
}
