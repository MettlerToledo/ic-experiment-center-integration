// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Materials;

/// <summary>
/// Defines the possible usages of a material portion in an operation.
/// </summary>
public enum MaterialPortionUsageJson
{
    /// <summary>
    /// The material portion is used as an input in the operation.
    /// </summary>
    Input = 0,

    /// <summary>
    /// The material portion is produced as an output of the operation.
    /// </summary>
    Output = 1
}
