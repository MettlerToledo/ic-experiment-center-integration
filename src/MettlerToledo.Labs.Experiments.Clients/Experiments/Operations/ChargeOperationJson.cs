// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// An operation to charge material into a vessel.
/// </summary>
public class ChargeOperationJson : OperationWithParametersJson<ChargeParametersJson>
{
    /// <summary>
    /// A reference to the material to be charged.
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }
}
