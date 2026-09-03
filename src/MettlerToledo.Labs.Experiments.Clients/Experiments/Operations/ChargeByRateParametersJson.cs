// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// Parameters for a charge operation that uses a rate to determine how quickly to charge the material portion.
/// </summary>
public class ChargeByRateParametersJson : ChargeParametersJson
{
    /// <summary>
    /// The rate for the charge operation.
    /// </summary>
    public UnitValueParameterJson Rate { get; set; } = new UnitValueParameterJson();
}
