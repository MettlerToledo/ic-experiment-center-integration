// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Definitions;

/// <summary>
/// A base class for process, stage, and operation definitions.
/// </summary>
public class S88DefinitionJson
{
    /// <summary>
    /// A unique identifier for the element type.
    /// Currently this should be the same as the <see cref="ProcessTypeJson"/> for processes and <see cref="StageTypeJson"/> for stages and currently valid values for operations.
    /// However, in the future users may be able to define their own types and not limited to the enum values.
    /// In the case of operations certain built-in operations will have a unique identifier that has a specific meaning in the system even in the future.
    /// It is intended to be a human readable name that can be used to identify the element type in the system.
    /// It will often be similar to the name of the element type, but should be a more stable identifier than the name, which may change over time.
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// The name of the process, stage, or operation type.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// The description of the process, stage, or operation type.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}
