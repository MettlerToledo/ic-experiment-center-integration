// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions;

/// <summary>
/// Defines the states of an experiment.
/// </summary>
public enum ExperimentValidityStateJson
{
    /// <summary>
    /// The experiment is being planned and has not started yet.
    /// </summary>
    Invalid = 0,

    /// <summary>
    /// The experiment is in progress. 
    /// That is at least part of the experiment has started and analysis has not been completed.
    /// </summary>
    Valid = 1
}
