// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

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
