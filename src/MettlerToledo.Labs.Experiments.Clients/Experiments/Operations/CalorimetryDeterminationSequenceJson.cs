// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The sequences of determinations that can be performed in a calorimetry operation.
/// </summary>
public enum CalorimetryDeterminationSequenceJson
{
    /// <summary>
    /// A U determination.
    /// </summary>
    U = 0,

    /// <summary>
    /// A Cpr determination.
    /// </summary>
    Cpr = 1,

    /// <summary>
    /// A U determination followed by a Cpr determination.
    /// </summary>
    UAndCpr = 2,

    /// <summary>
    /// A Cpr determination followed by a U determination.
    /// </summary>
    CprAndU = 3,

    /// <summary>
    /// A U determination followed by a Cpr determination and then another U determination.
    /// </summary>
    UAndCprAndU = 4
}
