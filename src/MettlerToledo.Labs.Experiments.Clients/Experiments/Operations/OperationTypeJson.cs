// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Enumerated operation type
/// </summary>
public enum OperationTypeJson
{
    /// <summary>Unknown.</summary>
    Unknown = 0,

    /// <summary>Centrifuge.</summary>
    Centrifuge = 1,

    /// <summary>Chromatography</summary>
    Chromatography = 2,

    /// <summary>Concentrate.</summary>
    Concentrate = 3,

    /// <summary>Discharge.</summary>
    Discharge = 4,

    /// <summary>Dry.</summary>
    Dry = 5,

    /// <summary>Filter.</summary>
    Filter = 6,

    /// <summary>Homogenize.</summary>
    Homogenize = 7,

    /// <summary>Hydrocyclone.</summary>
    Hydrocyclone = 8,

    /// <summary>Inert.</summary>
    Inert = 9,

    /// <summary>Mill.</summary>
    Mill = 10,

    /// <summary>PackAndLabel.</summary>
    PackAndLabel = 11,

    /// <summary>PressureAdjust.</summary>
    PressureAdjust = 12,

    /// <summary>Recirculate.</summary>
    Recirculate = 13,

    /// <summary>Rinse.</summary>
    Rinse = 14,

    /// <summary>Separate.</summary>
    Separate = 15,

    /// <summary>Sieve</summary>
    Sieve = 16,

    /// <summary>Transfer</summary>
    Transfer = 17,

    /// <summary>Wash</summary>
    Wash = 18,

    /// <summary>Wet Mill</summary>
    WetMill = 19,

    /// <summary>Control pH.</summary>
    ControlPH = 20,

    /// <summary>Charge</summary>
    Charge = 21,

    /// <summary>Temperature Control.</summary>
    TemperatureControl = 22,

    /// <summary>Settle.</summary>
    Settle = 23,

    /// <summary>Agitate.</summary>
    Agitate = 24,

    /// <summary>Take Sample</summary>
    TakeSample = 25,

    /// <summary>Take Picture</summary>
    TakePicture = 26,

    /// <summary>Wait</summary>
    Wait = 27,

    /// <summary>QuickCal.</summary>
    QuickCalDetermination = 28,

    /// <summary>Record Virtual Volume.</summary>
    RecordVirtualVolume = 29,

    /// <summary>Reset Integral QrRtc.</summary>
    ResetIntegralQrRtc = 30,

    /// <summary>RTCal Cp Determination.</summary>
    RTCalDetermination = 31,

    /// <summary>HFCal U and Cpr Determination.</summary>
    HfCalDetermination = 32
}
