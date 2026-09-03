// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Units;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// An operation to record a virtual volume.
/// </summary>
public class RecordVirtualVolumeOperationJson : OperationJson
{
    /// <summary>
    /// The observed volume in a completed operation.
    /// </summary>
    public UnitValueJson? ObservedVolume { get; set; }
}
