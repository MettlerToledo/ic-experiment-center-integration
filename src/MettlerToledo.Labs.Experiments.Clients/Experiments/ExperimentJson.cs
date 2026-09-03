// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// Object, that when serialized to Json conforms to the experiment schema
/// Does not contain the trend data.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class ExperimentJson : BaseTypeJson
{
    /// <summary>
    /// Initializes a new instance of <see cref="ExperimentJson"/>
    /// </summary>
    public ExperimentJson()
    {
        Process = new ProcessJson();
        Materials = new List<MaterialPortionJson>();
    }

    /// <summary>
    /// Experiment Id
    /// </summary>
    public Guid? Id { get; set; }

    /// <summary>
    /// The version of the schema this experiment json was created with.
    /// </summary>
    public string SchemaVersion { get; set; } = string.Empty;

    /// <summary>
    /// An Id from the experiment that should be unquie within the ELN.
    /// </summary>
    public string UniqueElnId { get; set; } = string.Empty;

    /// <summary>
    /// The name of the experiment
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A suffix used to distinguish experiments created from the same template.
    /// </summary>
    public string Suffix { get; set; } = string.Empty;

    /// <summary>
    /// The user associated with the experiment.
    /// </summary>
    public string User { get; set; } = string.Empty;

    /// <summary>
    /// The name of the project this experiment is associated with.
    /// </summary>
    public string Project { get; set; } = string.Empty;

    /// <summary>
    /// The name of the time zone that experiment was executed in.
    /// </summary>
    public string TimeZone { get; set; } = string.Empty;

    /// <summary>
    /// The status of the experiment.
    /// </summary>
    public ExperimentStateJson ExperimentStatus { get; set; }

    /// <summary>
    /// Indicates if the user considers the experiment valid or not.
    /// </summary>
    public ExperimentValidityStateJson ValidityStatus { get; set; } = ExperimentValidityStateJson.Valid;

    /// <summary>
    /// The process for the experiment (stages, operation groups, and operations).
    /// </summary>
    public ProcessJson Process { get; set; }

    /// <summary>
    /// The materials (i.e. chemicals, substances, mixtures) defined in the experiment.
    /// </summary>
    public IList<MaterialPortionJson> Materials { get; }

    /// <summary>
    /// Custom content provided by the user.
    /// </summary>
    public JsonObject? CustomMetadata { get; set; }
}
