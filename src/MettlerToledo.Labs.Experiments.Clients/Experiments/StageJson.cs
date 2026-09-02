// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// A stage of an experiment.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class StageJson : BaseTypeJson
{
    /// <summary>
    /// Initializes a new instance of <see cref="StageJson"/>
    /// </summary>
    public StageJson()
    {
        EquipmentCollection = new List<EquipmentJson>();
        OperationGroups = new List<OperationGroupJson>();
    }

    /// <summary>
    /// Stage Id
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// The start time in UTC of the stage.
    /// </summary>
    public DateTime? StartTimeUtc { get; set; }

    /// <summary>
    /// The completion time in UTC of the stage.
    /// </summary>
    public DateTime? CompletionTimeUtc { get; set; }

    /// <summary>
    /// The type of the stage.
    /// </summary>
    public StageTypeJson StageType { get; set; } = StageTypeJson.Unknown;

    /// <summary>
    /// The equipment used in the stage.
    /// </summary>
    public IList<EquipmentJson> EquipmentCollection { get; }

    /// <summary>
    /// The operation groups of the stage.
    /// </summary>
    public IList<OperationGroupJson> OperationGroups { get; }
}
