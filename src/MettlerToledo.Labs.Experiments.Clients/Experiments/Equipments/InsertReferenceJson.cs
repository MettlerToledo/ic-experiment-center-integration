// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Has the information identifying an insert.
/// </summary>
public class InsertReferenceJson
{
    /// <summary>
    /// Type of the insert
    /// </summary>
    public InsertTypesJson InsertType { get; set; }
    /// <summary>
    /// Name of the insert e.g. type, Anchor, C22, 48mm/Magnetic drive shaft, C22, Length 129mm
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Actual part number
    /// </summary>
    public string? PartOrderNumber { get; set; }
}
