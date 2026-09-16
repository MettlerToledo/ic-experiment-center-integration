// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// The base properties of an operation.
/// </summary>
[JsonDerivedType(typeof(AgitateOperationJson), "AgitateOperation")]
[JsonDerivedType(typeof(TemperatureControlOperationJson), "TemperatureControlOperation")]
[JsonDerivedType(typeof(ChargeOperationJson), "ChargeOperation")]
[JsonDerivedType(typeof(SampleOperationJson), "SampleOperation")]
[JsonDerivedType(typeof(ReleaseOperationJson), "ReleaseOperation")]
[JsonDerivedType(typeof(TimeMarkerOperationJson), "TimeMarkerOperation")]
[JsonDerivedType(typeof(ControlPhOperationJson), "ControlPhOperation")]
[JsonDerivedType(typeof(ControlBalanceOperationJson), "ControlBalanceOperation")]
[JsonDerivedType(typeof(ControlFinalControlElementOperationJson), "ControlFinalControlElementOperation")]
[JsonDerivedType(typeof(ObservationOperationJson), "ObservationOperation")]
[JsonDerivedType(typeof(DynamicOperationJson), "DynamicOperation")]
[JsonDerivedType(typeof(WaitOperationJson), "WaitOperation")]
[JsonDerivedType(typeof(RecordVirtualVolumeOperationJson), "RecordVirtualVolumeOperation")]
[JsonDerivedType(typeof(ResetQrtcIntegralOperationJson), "ResetQrtcIntegralOperation")]
[JsonDerivedType(typeof(HFCalorimetryDeterminationOperationJson), "HFCalorimetryDeterminationOperation")]
[JsonDerivedType(typeof(RTCalorimetryDeterminationOperationJson), "RTCalorimetryDeterminationOperation")]
public abstract class OperationJson : BaseTypeJson
{
    /// <summary>
    /// The start time of the operation.
    /// </summary>
    public DateTime? StartTimeUtc { get; set; }

    /// <summary>
    /// The completion time of the operation.
    /// </summary>
    public DateTime? CompletionTimeUtc { get; set; }

    /// <summary>
    /// The completion status of the operation.
    /// </summary>
    public CompletionStatusEnum? CompletionStatus { get; set; }

    /// <summary>
    /// The possible completion status for the operation.
    /// </summary>
    public enum CompletionStatusEnum
    {
        /// <summary>
        /// The operation completed normally or was completed by the user
        /// </summary>
        Complete,
        /// <summary>
        /// The instrument aborted the operation. This most often is because an error or emergency condition occurred on the instrument.
        /// </summary>
        Aborted
    }

    /// <summary>
    /// The peripherals used for this operation.
    /// </summary>
    public List<EquipmentReferenceJson> Peripherals { get; set; } = new();

    /// <summary>
    /// The equipment used for this operation if any
    /// </summary>
    public EquipmentReferenceJson? Equipment { get; set; }
}
