// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using AutoChem.Lab.S88Definitions.Materials;

namespace AutoChem.Lab.S88Definitions;

/// <summary>
/// A stage of an experiment linked with the experiment materials
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class StageWithMaterialsJson
{
    /// <summary>
    /// Initializes a new instance of <see cref="StageWithMaterialsJson"/>
    /// </summary>
    public StageWithMaterialsJson()
    {
        Stage = new StageJson();
        Materials = new List<MaterialPortionJson>();
    }

    /// <summary>
    /// The base stage data.
    /// </summary>
    public StageJson Stage { get; set; }

    /// <summary>
    /// The materials (i.e. chemicals, substances, mixtures) defined in the experiment that the stage belongs to.
    /// </summary>
    public IList<MaterialPortionJson> Materials { get; }
}
