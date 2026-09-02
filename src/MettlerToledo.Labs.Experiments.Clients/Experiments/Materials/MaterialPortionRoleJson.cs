// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace AutoChem.Lab.S88Definitions.Materials;

/// <summary>
/// Defines roles of a material.
/// </summary>
public enum MaterialPortionRoleJson
{
    /// <summary>
    /// Undefined.
    /// </summary>
    Undefined,

    /// <summary>
    /// Limiting reactant is the reactant that will be exhausted first, causing the reaction to stop.
    /// </summary>
    LimitingReactant,

    /// <summary>
    /// Chemical reactant - something that interacts with other reactants to produce output.
    /// </summary>
    Reactant,

    /// <summary>
    /// Something that helps reactants to interact better.
    /// </summary>
    Catalyst,

    /// <summary>
    /// Something that dissolves reagents.
    /// </summary>
    Solvent,

    /// <summary>
    /// Something that stops reaction.
    /// </summary>
    QuenchMaterial,

    /// <summary>
    /// Intermediate result.
    /// </summary>
    Intermediate,

    /// <summary>
    /// Unwanted product.
    /// </summary>
    ByProduct,

    /// <summary>
    /// Desired result.
    /// </summary>
    Product,

    /// <summary>
    /// Something that is not one of the other options or has not been defined.
    /// </summary>
    Other,
}
