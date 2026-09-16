// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;

/// <summary>
/// Defines the reponses for when a charge operation is ended by reaching the maximum amount 
/// when controlled by an expression that determines the amount to be charged or the rate to charge at.
/// </summary>
public enum ChargeEndedByMaxAmountResponseJson
{
    /// <summary>
    /// Continue the procedure.
    /// </summary>
    ContinueProcedure = 0,

    /// <summary>
    /// Pause the procedure.
    /// </summary>
    PauseProcedure = 1,
}
