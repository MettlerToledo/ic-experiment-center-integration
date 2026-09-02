// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a real time calorimetry determination operation.
/// </summary>
public class RTCalorimetryDeterminationParametersJson
{
    /// <summary>
    /// The before, between, after, and for each step waiting time of the determination
    /// </summary>
    public UnitValueParameterJson WaitingTime { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The change in Tr for the cpr determination in the operation if present.
    /// </summary>
    public UnitValueParameterJson DeltaTr { get; set; } = new UnitValueParameterJson();
}
