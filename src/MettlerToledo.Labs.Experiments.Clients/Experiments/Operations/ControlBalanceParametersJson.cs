// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Base class for parameters for controlling a balance
/// </summary>
[JsonDerivedType(typeof(ControlBalanceOnParametersJson), "ControlBalanceOnParameters")]
[JsonDerivedType(typeof(ControlBalanceOffParametersJson), "ControlBalanceOffParameters")]
public class ControlBalanceParametersJson
{
}
