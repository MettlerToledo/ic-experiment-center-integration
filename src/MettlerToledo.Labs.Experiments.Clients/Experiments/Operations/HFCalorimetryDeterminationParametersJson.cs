// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;
using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

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
