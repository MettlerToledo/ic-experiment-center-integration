// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a standard HF calorimetry determination operation.
/// </summary>
public class HFCalorimetryDeterminationParametersStandardJson : HFCalorimetryDeterminationParametersJson
{
    /// <summary>
    /// The sequence of determinations to perform in the operation.
    /// </summary>
    public CalorimetryDeterminationSequenceJson DeterminationSequence { get; set; }

    /// <summary>
    /// The change in Tr for the cpr determination in the operation if present.
    /// </summary>
    public UnitValueParameterJson DeltaTr { get; set; } = new UnitValueParameterJson();
}
