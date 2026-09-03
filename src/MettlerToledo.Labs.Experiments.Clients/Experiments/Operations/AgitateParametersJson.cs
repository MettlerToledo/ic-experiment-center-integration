// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// The parameters for an agitate operation.
/// </summary>
[JsonDerivedType(typeof(StirParametersJson), "StirParameters")]
[JsonDerivedType(typeof(ShakeParametersJson), "ShakeParameters")]
[JsonDerivedType(typeof(VortexParametersJson), "VortexParameters")]
[JsonDerivedType(typeof(SwirlParametersJson), "SwirlParameters")]
[JsonDerivedType(typeof(SonicParametersJson), "SonicParameters")]
public class AgitateParametersJson
{
}
