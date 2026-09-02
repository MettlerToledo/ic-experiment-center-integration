// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// A group of operations that run in parallel.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class OperationGroupJson
{
    /// <summary>
    /// The parallel operation sequences.
    /// </summary>
    public IList<OperationSequenceJson> OperationSequences { get; } = new List<OperationSequenceJson>();
}
