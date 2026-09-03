// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using AutoChem.Lab.S88Definitions.Operations;

namespace AutoChem.Lab.S88Definitions;

/// <summary>
/// A sequence of operations.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class OperationSequenceJson
{
    /// <summary>
    /// The operations in the sequence.
    /// </summary>
    public IList<OperationJson> Operations { get; } = new List<OperationJson>();
}
