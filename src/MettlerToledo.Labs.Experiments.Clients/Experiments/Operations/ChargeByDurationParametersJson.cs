// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

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
