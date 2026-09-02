// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions;

/// <summary>
/// Process types for an experiment.
/// </summary>
public enum ProcessTypeJson
{
    /// <summary>
    /// No process type specified
    /// </summary>
    None = 0,

    /// <summary>
    /// Synthesis process
    /// </summary>
    Synthesis = 1,

    /// <summary>
    /// Rework process
    /// </summary>
    Rework = 2,

    /// <summary>
    /// Reprocess process
    /// </summary>
    Reprocess = 3,

    /// <summary>
    /// Purification process
    /// </summary>
    Purification = 4,

    /// <summary>
    /// Particle tuning process
    /// </summary>
    ParticleTuning = 5
}
