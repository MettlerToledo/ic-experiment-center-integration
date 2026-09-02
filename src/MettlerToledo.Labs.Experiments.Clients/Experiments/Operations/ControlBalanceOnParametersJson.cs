// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Parameters for turning the balance control on
/// </summary>
public class ControlBalanceOnParametersJson : ControlBalanceParametersJson
{
    /// <summary>
    /// A reference to the material on the balance.
    /// </summary>
    public MaterialPortionReferenceJson? Material { get; set; }

    /// <summary>
    /// Specifies to wait for the balance to become stable before starting the free balance operation.
    /// </summary>
    public bool WaitForStable { get; set; }
}
