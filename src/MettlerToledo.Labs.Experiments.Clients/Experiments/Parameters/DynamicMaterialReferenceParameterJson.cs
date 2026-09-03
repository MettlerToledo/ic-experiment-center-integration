// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

/// <summary>
/// Represents a parameter that references a material used or produced in a dynamic operation.
/// </summary>
public class DynamicMaterialReferenceParameterJson : DynamicParameterJson
{
    /// <summary>
    /// A material used or resulting from a dynamic operation.
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }

    /// <summary>
    /// The usage of the material portion in the operation.
    /// </summary>
    public MaterialPortionUsageJson MaterialPortionUsage { get; set; } = MaterialPortionUsageJson.Input;
}
