// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;

/// <summary>
/// Classifies the major group of ProfileTypes e.g. Height and Area
/// </summary>
public enum ProfileGroupJson
{
    /// <summary>
    /// Profile is measuring Peak Height
    /// </summary>
    Height,

    /// <summary>
    /// Profile is measuring Peak Area
    /// </summary>
    Area,

    /// <summary>
    /// Profile is measuring Peak Center
    /// </summary>
    Center,

    /// <summary>
    /// Profile is measuring Peak Width
    /// </summary>
    Width,

    /// <summary>
    /// Profile is a Region definition
    /// </summary>
    Region
}
