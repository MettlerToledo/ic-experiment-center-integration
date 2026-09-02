// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Wait by condition
/// </summary>
public class WaitParametersByEndCondition : WaitParametersJson
{
    /// <summary>
    /// The user specified end condition, which the use will indicate when it has been reached.
    /// </summary>
    public string? EndCondition { get; set; }
}
