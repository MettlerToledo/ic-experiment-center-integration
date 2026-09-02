// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;

/// <summary>
/// Identifies a trend in the experiment
/// </summary>
public class TrendPathJson : IEquatable<TrendPathJson>
{
    /// <summary>
    /// Initializes a new instance of <see cref="TrendPathJson"/>
    /// </summary>
    public TrendPathJson(Guid stageId, Guid equipmentId, Guid occupationId, string trendName)
    {
        StageId = stageId;
        EquipmentId = equipmentId;
        OccupationId = occupationId;
        TrendName = trendName;
    }

    /// <summary>
    /// The id of the stage the trend belongs to.
    /// </summary>
    public Guid StageId { get; }

    /// <summary>
    /// The id of the equipment the trend belongs to.
    /// </summary>
    public Guid EquipmentId { get; }

    /// <summary>
    /// The id of the occupation the trend belongs to.
    /// </summary>
    public Guid OccupationId { get; }

    /// <summary>
    /// The name of the trend.
    /// </summary>
    public string TrendName { get; }

    /// <inheritdoc />
    public bool Equals(TrendPathJson? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return StageId.Equals(other.StageId)
            && EquipmentId.Equals(other.EquipmentId)
            && OccupationId.Equals(other.OccupationId)
            && string.Equals(TrendName, other.TrendName, StringComparison.Ordinal);
    }

    /// <inheritdoc />
    public override bool Equals(object? obj) => Equals(obj as TrendPathJson);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = (hash * 31) + StageId.GetHashCode();
            hash = (hash * 31) + EquipmentId.GetHashCode();
            hash = (hash * 31) + OccupationId.GetHashCode();
            hash = (hash * 31) + (TrendName is null ? 0 : StringComparer.Ordinal.GetHashCode(TrendName));
            return hash;
        }
    }

    /// <summary>Returns whether two <see cref="TrendPathJson"/> instances are equal.</summary>
    public static bool operator ==(TrendPathJson? left, TrendPathJson? right) =>
        left?.Equals(right) ?? right is null;

    /// <summary>Returns whether two <see cref="TrendPathJson"/> instances are not equal.</summary>
    public static bool operator !=(TrendPathJson? left, TrendPathJson? right) => !(left == right);
}
