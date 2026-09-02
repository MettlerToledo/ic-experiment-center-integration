// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

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
