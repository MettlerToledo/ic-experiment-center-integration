// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments;

/// <summary>
/// The process for an experiment.
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class ProcessJson : BaseTypeJson
{
    /// <summary>
    /// Initializes a new instance of <see cref="ProcessJson"/>.
    /// </summary>
    public ProcessJson()
    {
        Stages = new List<StageJson>();
    }

    /// <summary>
    /// The type of process.
    /// </summary>
    public ProcessTypeJson ProcessType { set; get; } = ProcessTypeJson.None;

    /// <summary>
    /// The stage of the process.
    /// </summary>
    public IList<StageJson> Stages { get; }
}
