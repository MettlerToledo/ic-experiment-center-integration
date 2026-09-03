// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for a release operation.
/// </summary>
[JsonDerivedType(typeof(ReleaseParametersCurrentJson), "ReleaseParametersCurrent")]
[JsonDerivedType(typeof(ReleaseParametersDefinedJson), "ReleaseParametersDefined")]
public class ReleaseParametersJson
{
}
