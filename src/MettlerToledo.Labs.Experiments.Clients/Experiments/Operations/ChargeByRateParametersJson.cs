// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

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
