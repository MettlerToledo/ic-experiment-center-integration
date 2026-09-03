// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Base class for parameters for controlling a balance
/// </summary>
[JsonDerivedType(typeof(ControlBalanceOnParametersJson), "ControlBalanceOnParameters")]
[JsonDerivedType(typeof(ControlBalanceOffParametersJson), "ControlBalanceOffParameters")]
public class ControlBalanceParametersJson
{
}
