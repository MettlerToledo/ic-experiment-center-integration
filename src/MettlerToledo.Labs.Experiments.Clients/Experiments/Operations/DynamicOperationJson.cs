// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// A user definable operation that can be used for manual operations or user defined operations for controling
/// non-Mettler Toledo equipment. Note the control of external equipment is a future use case and not currently supported.
/// </summary>
public class DynamicOperationJson : OperationJson
{
    /// <summary>
    /// The type of the operations.
    /// </summary>
    public OperationTypeJson? OperationType { get; set; }

    /// <summary>
    /// The parameters for the operation.
    /// </summary>
    public List<DynamicParameterJson> Parameters { get; set; } = new();
}
