// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// Defines the states of an experiment.
/// </summary>
public enum ExperimentStateJson
{
    /// <summary>
    /// The experiment is being planned and has not started yet.
    /// </summary>
    Planning = 0,

    /// <summary>
    /// The experiment is in progress. 
    /// That is at least part of the experiment has started and analysis has not been completed.
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// The experiment has been executed and conclusion have been drawn.
    /// </summary>
    Concluded = 2
}
