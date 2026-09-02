// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions;

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
