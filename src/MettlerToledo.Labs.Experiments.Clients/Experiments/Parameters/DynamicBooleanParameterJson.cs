// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

/// <summary>
/// A dynamic boolean parameter.
/// </summary>
public class DynamicBooleanParameterJson : DynamicParameterJson
{
    /// <summary>
    /// The value of the parameter.
    /// </summary>
    public bool? Value { get; set; }
}
