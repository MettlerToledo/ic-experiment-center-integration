// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Describes a part of a path, which defines the connection of a peripheral to an instrument.
/// </summary>
public class PathPartJson
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PathPartJson"/> class.
    /// </summary>
    public PathPartJson()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PathPartJson"/> class.
    /// </summary>
    public PathPartJson(string type, int index)
    {
        Type = type;
        Index = index;
    }

    /// <summary>
    /// The type of the path part, e.g. "Ecb", "ControlPort", "SensorPort", "DosingUnit"
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// The index of the path part, e.g. the index of the control port or sensor port.
    /// This index is zero-based, i.e. the first control port has index 0, the second control port has index 1, etc.
    /// </summary>
    public int Index { get; set; }
}
