// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The power level to use for the calibration step of an HF calorimetry determination operation.
/// </summary>
public enum CalibrationPowerLevelJson
{
    /// <summary>
    /// The recommended power level for the calibration for the reactor.
    /// </summary>
    RecommendedPower = 0,

    /// <summary>
    /// Fifty percent of the recommended power level for the calibration for the reactor.
    /// </summary>
    FiftyPercentPower = 1,

    /// <summary>
    /// Twenty five percent of the recommended power level for the calibration for the reactor.
    /// </summary>
    TwentyFivePercentPower = 2,
}
