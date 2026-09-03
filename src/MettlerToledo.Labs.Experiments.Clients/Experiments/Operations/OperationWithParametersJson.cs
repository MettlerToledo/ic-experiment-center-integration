// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Operations;

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
