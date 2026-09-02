using MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests;

public class TrendPathJsonTests
{
    private static readonly Guid StageId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid EquipmentId = Guid.Parse("22222222-2222-2222-2222-222222222222");
    private static readonly Guid OccupationId = Guid.Parse("33333333-3333-3333-3333-333333333333");
    private const string TrendName = "Temperature";

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var firstTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var secondTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);

        firstTrendPath.Equals(secondTrendPath).ShouldBeTrue();
        firstTrendPath.Equals((object)secondTrendPath).ShouldBeTrue();
        (firstTrendPath == secondTrendPath).ShouldBeTrue();
        (firstTrendPath != secondTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_SameReference_ReturnsTrue()
    {
        var trendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);

        trendPath.Equals(trendPath).ShouldBeTrue();
    }

    [Fact]
    public void Equals_Null_ReturnsFalse()
    {
        var trendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);

        trendPath.Equals(null).ShouldBeFalse();
        trendPath.Equals((object?)null).ShouldBeFalse();
        (trendPath == null).ShouldBeFalse();
        (trendPath != null).ShouldBeTrue();
    }

    [Fact]
    public void EqualityOperator_BothNull_ReturnsTrue()
    {
        TrendPathJson? leftTrendPath = null;
        TrendPathJson? rightTrendPath = null;

        (leftTrendPath == rightTrendPath).ShouldBeTrue();
        (leftTrendPath != rightTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentStageId_ReturnsFalse()
    {
        var originalTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var differentStageTrendPath = new TrendPathJson(Guid.NewGuid(), EquipmentId, OccupationId, TrendName);

        originalTrendPath.Equals(differentStageTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentEquipmentId_ReturnsFalse()
    {
        var originalTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var differentEquipmentTrendPath = new TrendPathJson(StageId, Guid.NewGuid(), OccupationId, TrendName);

        originalTrendPath.Equals(differentEquipmentTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentOccupationId_ReturnsFalse()
    {
        var originalTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var differentOccupationTrendPath = new TrendPathJson(StageId, EquipmentId, Guid.NewGuid(), TrendName);

        originalTrendPath.Equals(differentOccupationTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_DifferentTrendName_ReturnsFalse()
    {
        var temperatureTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var pressureTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, "Pressure");

        temperatureTrendPath.Equals(pressureTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void Equals_TrendNameCaseDiffers_ReturnsFalse()
    {
        var upperCaseTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, "Temperature");
        var lowerCaseTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, "temperature");

        upperCaseTrendPath.Equals(lowerCaseTrendPath).ShouldBeFalse();
    }

    [Fact]
    public void GetHashCode_SameValues_AreEqual()
    {
        var firstTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var secondTrendPath = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);

        firstTrendPath.GetHashCode().ShouldBe(secondTrendPath.GetHashCode());
    }

    [Fact]
    public void UsableAsDictionaryKey_LookupSucceeds()
    {
        var storedKey = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);
        var equivalentLookupKey = new TrendPathJson(StageId, EquipmentId, OccupationId, TrendName);

        var dictionary = new Dictionary<TrendPathJson, int> { [storedKey] = 42 };

        dictionary.ContainsKey(equivalentLookupKey).ShouldBeTrue();
        dictionary[equivalentLookupKey].ShouldBe(42);
    }

    [Fact]
    public void UsableAsDictionaryKey_DifferentKeysCoexist()
    {
        var temperatureKey = new TrendPathJson(StageId, EquipmentId, OccupationId, "Temperature");
        var pressureKey = new TrendPathJson(StageId, EquipmentId, OccupationId, "Pressure");

        var dictionary = new Dictionary<TrendPathJson, int>
        {
            [temperatureKey] = 1,
            [pressureKey] = 2,
        };

        dictionary.Count.ShouldBe(2);
        dictionary[temperatureKey].ShouldBe(1);
        dictionary[pressureKey].ShouldBe(2);
    }

    [Fact]
    public void UsableInHashSet_DeduplicatesEquivalentInstances()
    {
        var trendPathSet = new HashSet<TrendPathJson>
        {
            new(StageId, EquipmentId, OccupationId, TrendName),
            new(StageId, EquipmentId, OccupationId, TrendName),
        };

        trendPathSet.Count.ShouldBe(1);
    }
}
