// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Parameters;

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
