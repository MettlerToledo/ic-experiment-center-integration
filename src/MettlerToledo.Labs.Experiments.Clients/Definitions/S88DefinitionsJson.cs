// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions;

/// <summary>
/// The s88 definitions for a system.
/// </summary>
public class S88DefinitionsJson
{
    /// <summary>
    /// The processes defined in the system.
    /// </summary>
    public List<ProcessDefinitionJson> Processes { get; set; } = new List<ProcessDefinitionJson>();

    /// <summary>
    /// The stages defined in the system.
    /// </summary>
    public List<StageDefinitionJson> Stages { get; set; } = new List<StageDefinitionJson>();

    /// <summary>
    /// The operations defined in the system.
    /// </summary>
    public List<OperationDefinitionJson> Operations { get; set; } = new List<OperationDefinitionJson>();
}
