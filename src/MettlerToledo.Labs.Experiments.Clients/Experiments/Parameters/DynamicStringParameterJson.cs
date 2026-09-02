// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;

/// <summary>
/// A dynamic string parameter.
/// Limitations of the <see cref="StringValue"/> will come from the configuration in the system.
/// This will be based on the OperationType and Internal Name of the parameter.
/// </summary>
public class DynamicStringParameterJson : DynamicParameterJson
{
    /// <summary>
    /// The value of the parameter
    /// </summary>
    public string? StringValue { get; set; }
}
