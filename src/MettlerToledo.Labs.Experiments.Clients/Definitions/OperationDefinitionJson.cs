// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions;

/// <summary>
/// Defines a type of operation in the system.
/// </summary>
public class OperationDefinitionJson : S88DefinitionJson
{
    /// <summary>
    /// The list of dynamic parameters for the operation.
    /// </summary>
    public List<DynamicParameterDefinitionJson> Parameters { get; set; } = new List<DynamicParameterDefinitionJson>();
}
