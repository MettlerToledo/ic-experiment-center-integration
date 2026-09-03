// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

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
