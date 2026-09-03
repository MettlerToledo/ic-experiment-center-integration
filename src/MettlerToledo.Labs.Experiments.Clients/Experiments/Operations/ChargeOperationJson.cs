// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using AutoChem.Lab.S88Definitions.Materials;

namespace AutoChem.Lab.S88Definitions.Operations;

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
