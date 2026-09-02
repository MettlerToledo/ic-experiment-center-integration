// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// The base parameters for an HF calorimetry determination operation.
/// </summary>
[JsonDerivedType(typeof(HFCalorimetryDeterminationParametersStandardJson), "HFCalorimetryDeterminationParametersStandard")]
[JsonDerivedType(typeof(HFCalorimetryDeterminationParametersQuickJson), "HFCalorimetryDeterminationParametersQuick")]
public abstract class HFCalorimetryDeterminationParametersJson
{
    /// <summary>
    /// The before, between, after, and for each step waiting time of the determination
    /// </summary>
    public UnitValueParameterJson WaitingTime { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The power level to use for the calibration step of the operation.
    /// </summary>
    public CalibrationPowerLevelJson PowerLevel { get; set; }
}
