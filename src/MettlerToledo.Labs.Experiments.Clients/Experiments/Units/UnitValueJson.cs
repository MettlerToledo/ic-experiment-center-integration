// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Units;

/// <summary>
/// Defines a value with units and significant digits.
/// </summary>
public class UnitValueJson
{
    /// <summary>
    /// The numeric portion of the value and unit.
    /// </summary>
    public double Value { get; set; }

    /// <summary>
    /// The unit for the value.
    /// </summary>
    public string Unit { get; set; } = string.Empty;

    /// <summary>
    /// The number of significant digits of the UnitValue.
    /// For values less than 1 this is the precision as a power of 10 (e.g. 0 => 1.0, -1 => 0.1, -2 => 0.01, -3 0.001).
    /// </summary>
    public int? SignificantDigits { get; set; }
}
