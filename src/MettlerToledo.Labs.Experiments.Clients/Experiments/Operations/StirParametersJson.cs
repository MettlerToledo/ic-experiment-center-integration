// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Parameters;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The parameters for a stir operation.
/// </summary>
public class StirParametersJson : AgitateParametersJson
{
    /// <summary>
    /// The parameter to indicate the type of stirrer.
    /// </summary>
    public StirrerTypeJson? StirrerType { get; set; }

    /// <summary>
    /// The stirring speed.
    /// </summary>
    public UnitValueParameterJson Speed { get; set; } = new UnitValueParameterJson();

    /// <summary>
    /// The duration of the stirring ramp.
    /// </summary>
    public UnitValueParameterJson Duration { get; set; } = new UnitValueParameterJson();
}
