// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// An operation with parameters.
/// </summary>
public class OperationWithParametersJson<TParameters> : OperationJson
{
    /// <summary>
    /// The parameters of the operation.
    /// </summary>
    public TParameters? Parameters { get; set; }
}
