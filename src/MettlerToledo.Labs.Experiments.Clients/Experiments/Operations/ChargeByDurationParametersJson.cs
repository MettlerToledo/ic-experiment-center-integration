// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for a charge operation that uses a duration to determine over what period of time to charge the material portion.
/// </summary>
public class ChargeByDurationParametersJson : ChargeParametersJson
{
    /// <summary>
    /// The duration for the charge operation.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
