// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for a release operation.
/// </summary>
[JsonDerivedType(typeof(ReleaseParametersCurrentJson), "ReleaseParametersCurrent")]
[JsonDerivedType(typeof(ReleaseParametersDefinedJson), "ReleaseParametersDefined")]
public class ReleaseParametersJson
{
}
