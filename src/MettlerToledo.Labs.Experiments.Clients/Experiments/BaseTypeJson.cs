// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

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
