// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace AutoChem.Lab.S88Definitions.Materials;

/// <summary>
/// A reference to a material portion.
/// </summary>
public sealed class MaterialPortionReferenceJson
{
    /// <summary>
    /// The tracking Id of the material portion.
    /// </summary>
    public Guid TrackingId { get; set; }
}
