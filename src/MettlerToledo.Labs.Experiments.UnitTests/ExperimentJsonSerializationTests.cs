// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Diagnostics;
using System.Text.Json;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Samples;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests;

public class ExperimentJsonSerializationTests
{
    private readonly ITestOutputHelper _testOuputHelper;
    internal static readonly string OneHundredMlTwoPieceReactorName = "100 mL two-piece";

    public ExperimentJsonSerializationTests(ITestOutputHelper outputHelper)
    {
        _testOuputHelper = outputHelper;
    }

    [Fact]
    public void Deserialize_JsonFromExperiment_SameValues()
    {
        var referenceTime = DateTime.UtcNow;
        int lengthInMinutes = 2;
        var startTime = referenceTime.AddMinutes(-lengthInMinutes);
        var experiment = new ExperimentJson()
        {
            Name = "Test",
            ExperimentStatus = ExperimentStateJson.Concluded,
            Process = { Stages = { new StageJson { StartTimeUtc = startTime, CompletionTimeUtc = referenceTime } } }
        };
        var stirParameters = new StirParametersJson()
        {
            StirrerType = StirrerTypeJson.Overhead,
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1", SignificantDigits = 3 } }
        };
        var temperatureParameters = new TemperatureControlParametersRampByDurationJson
        {
            TemperatureControlMode = TemperatureControlModesJson.Reactor,
            Temperature = { Design = new UnitValueJson { Value = 35.0, Unit = "°C", SignificantDigits = 3 } },
            Duration = { Design = new UnitValueJson { Value = 80, Unit = "s", SignificantDigits = 2 } }
        };

        AddAgitateOperation(experiment, stirParameters, startTime = startTime.AddSeconds(5), startTime.AddSeconds(15));
        AddTemperatureControlOperation(experiment, temperatureParameters, startTime = startTime.AddSeconds(30), startTime.AddSeconds(90));
        var occupation = AddAlrOccupation(experiment);
        occupation.StartTimeUtc = startTime;
        occupation.ReleaseTimeUtc = referenceTime;

        var valueCount = 60 * lengthInMinutes;

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = startTime,
            Values = GetTrendValues(valueCount).ToList()
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = startTime,
            Values = GetTrendValues(valueCount).ToList()
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "R",
            Unit = "min⁻¹",
            StartTimeUtc = startTime,
            Values = (new TrendValueJson[] {
                new() { RelativeTime = TimeSpan.Zero, Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(1), Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(2), Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(3), Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(4), Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(5), Value = 0 },
                new() { RelativeTime = TimeSpan.FromSeconds(6), Value = 10 },
                new() { RelativeTime = TimeSpan.FromSeconds(7), Value = 20 },
                new() { RelativeTime = TimeSpan.FromSeconds(8), Value = 30 },
                new() { RelativeTime = TimeSpan.FromSeconds(9), Value = 40 },
                new() { RelativeTime = TimeSpan.FromSeconds(10), Value = 50 },
                new() { RelativeTime = TimeSpan.FromSeconds(11), Value = 60 },
                new() { RelativeTime = TimeSpan.FromSeconds(12), Value = 70 },
                new() { RelativeTime = TimeSpan.FromSeconds(13), Value = 80 },
                new() { RelativeTime = TimeSpan.FromSeconds(14), Value = 90 },
                new() { RelativeTime = TimeSpan.FromSeconds(15), Value = 100 },
                new() { RelativeTime = TimeSpan.FromSeconds(16), Value = 100 },
                new() { RelativeTime = TimeSpan.FromSeconds(17), Value = 100 },
                new() { RelativeTime = TimeSpan.FromSeconds(18), Value = 100 },
                new() { RelativeTime = TimeSpan.FromSeconds(19), Value = 100 }}).Concat(GetTrendValues(valueCount - 20, TimeSpan.FromSeconds(20))).ToList()
        });

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);

        // Act
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        // Assert
        readExperiment.ShouldNotBeNull();
        readExperiment.Name.ShouldBe(experiment.Name);
        var readAlr = readExperiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();
        var zone = readAlr.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
        zone.Receptacle.ReceptacleName.ShouldBe(OneHundredMlTwoPieceReactorName);
        var readOccupation = GetFirstOccupation(readExperiment);
        readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
        readOccupation.Trends[0].Values.Count.ShouldBe(occupation.Trends[0].Values.Count);
        readOccupation.Trends[1].Values.Count.ShouldBe(occupation.Trends[1].Values.Count);
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
        readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
    }

    [Fact]
    public void Serialize_ExperimentAndStageIds_RoundTripPreservesIds()
    {
        var experimentId = Guid.NewGuid();
        var stageId = Guid.NewGuid();
        var experiment = new ExperimentJson
        {
            Id = experimentId,
            Name = "Test",
            Process = { Stages = { new StageJson { Id = stageId } } }
        };

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);

        // Act
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        // Assert
        readExperiment.ShouldNotBeNull();
        readExperiment.Id.ShouldBe(experimentId);
        readExperiment.Process.Stages[0].Id.ShouldBe(stageId);
    }

    [Fact]
    public void Serialize_NullExperimentAndStageIds_RoundTripPreservesNull()
    {
        var experiment = new ExperimentJson
        {
            Id = null,
            Name = "Test",
            Process = { Stages = { new StageJson { Id = null } } }
        };

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);

        // Act
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        // Assert
        readExperiment.ShouldNotBeNull();
        readExperiment.Id.ShouldBeNull();
        readExperiment.Process.Stages[0].Id.ShouldBeNull();
    }

    [Fact]
    public void Deserialize_ShakeParameters_SameValues()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var shakeParameters = new ShakeParametersJson
        {
            Speed = new UnitValueParameterJson
            {
                Design = new UnitValueJson { Value = 250, Unit = "min-1", SignificantDigits = 3 }
            }
        };

        AddAgitateOperation(experiment, shakeParameters);

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        readExperiment.ShouldNotBeNull();
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<ShakeParametersJson>();
        readParameters.Speed.ShouldNotBeNull().Design.ShouldNotBeNull().Value.ShouldBe(shakeParameters.Speed.Design.Value);
    }

    [Fact]
    public void Deserialize_VortexParametersWithoutSpeed_SpeedIsNull()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var vortexParameters = new VortexParametersJson();

        AddAgitateOperation(experiment, vortexParameters);

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        readExperiment.ShouldNotBeNull();
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<VortexParametersJson>();
        readParameters.Speed.ShouldBeNull();
    }

    [Fact]
    public void Deserialize_SwirlParameters_SameType()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        AddAgitateOperation(experiment, new SwirlParametersJson());

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        readExperiment.ShouldNotBeNull();
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        readOperation.Parameters.ShouldBeOfType<SwirlParametersJson>();
    }

    [Fact]
    public void Deserialize_SonicParameters_SameType()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        AddAgitateOperation(experiment, new SonicParametersJson());

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        readExperiment.ShouldNotBeNull();
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        readOperation.Parameters.ShouldBeOfType<SonicParametersJson>();
    }

    [Fact]
    public void Deserialize_DeltaTemperatureOperation_SameValues()
    {
        var referenceTime = DateTime.UtcNow;
        int lengthInMinutes = 2;
        var startTime = referenceTime.AddMinutes(-lengthInMinutes);
        var experiment = new ExperimentJson()
        {
            Name = "Test",
            ExperimentStatus = ExperimentStateJson.Concluded,
            Process = { Stages = { new StageJson { StartTimeUtc = startTime, CompletionTimeUtc = referenceTime } } }
        };
        var temperatureParameters = new TemperatureControlParametersDeltaJson
        {
            EndJacketTemperature = { Design = new UnitValueJson { Value = 35.0, Unit = "°C", SignificantDigits = 3 } },
            TjTrDelta = { Design = new UnitValueJson { Value = 4.0, Unit = "K", SignificantDigits = 2 } }
        };

        AddTemperatureControlOperation(experiment, temperatureParameters, startTime = startTime.AddSeconds(30), startTime.AddSeconds(90));
        var occupation = AddAlrOccupation(experiment);
        occupation.StartTimeUtc = startTime;
        occupation.ReleaseTimeUtc = referenceTime;

        var json = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);

        // Act
        var readExperiment = JsonSerializer.Deserialize<ExperimentJson>(json, ExperimentJsonSerializer.SerializerOptions);

        // Assert
        readExperiment.ShouldNotBeNull();
        readExperiment.Name.ShouldBe(experiment.Name);

        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].
            ShouldBeOfType<TemperatureControlOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<TemperatureControlParametersDeltaJson>();
        readParameters.EndJacketTemperature.Design.ShouldNotBeNull().Value.ShouldBe(temperatureParameters.EndJacketTemperature.Design.Value);
        readParameters.TjTrDelta.Design.ShouldNotBeNull().Value.ShouldBe(temperatureParameters.TjTrDelta.Design.Value);
    }

    [Fact]
    public async Task Deserialize_JsonFromExperimentLargeTrendSet_SameValues()
    {
        var referenceTime = DateTime.UtcNow;
        int lengthInHours = 5;
        var startTime = referenceTime.AddHours(-lengthInHours);
        var experiment = new ExperimentJson()
        {
            Name = "Test",
            ExperimentStatus = ExperimentStateJson.Concluded,
            Process = { Stages = { new StageJson { StartTimeUtc = startTime, CompletionTimeUtc = referenceTime } } }
        };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };
        var temperatureParameters = new TemperatureControlParametersRampByDurationJson()
        {
            Temperature = { Actual = new UnitValueJson { Value = 40.0, Unit = "°C", SignificantDigits = 3 } },
            Duration = { Actual = new UnitValueJson { Value = 1.0, Unit = "min", SignificantDigits = 2 } },
            EndValueMargin = { Actual = new UnitValueJson { Value = 1.0, Unit = "K", SignificantDigits = 2 } }
        };
        var sampleParameters = new SingleSampleParametersJson()
        {
            SampleId = "Sample 1",
            Preparation = "Diluted",
            Quantity = { Actual = new UnitValueJson { Value = 1.5, Unit = "mL", SignificantDigits = 2 } }
        };

        AddAgitateOperation(experiment, stirParameters, startTime.AddMinutes(5), startTime.AddMinutes(6));
        AddTemperatureControlOperation(experiment, temperatureParameters, startTime.AddMinutes(7), startTime.AddMinutes(8));
        AddSampleOperation(experiment, sampleParameters, startTime.AddMinutes(10), startTime.AddMinutes(10));

        var occupation = AddAlrOccupation(experiment);
        var alrZone = GetInstrumentZone(experiment, 0);

        (string Name, string Unit)[] trendNameAndUnits = [
            (Name: "Tr", Unit: "°C"), (Name: "Tj", Unit: "°C"), (Name: "Tc", Unit: "°C"), (Name: "TSet", Unit: "°C"), (Name: "TEnd", Unit: "°C"),
            (Name: "Tr-Tj", Unit: "K"),
            (Name: "R",  Unit: "min⁻¹"), (Name: "RSet",  Unit: "min⁻¹"), (Name: "REnd",  Unit: "min⁻¹"),
            (Name: "pH",  Unit: "pH"),
            (Name: "V_DU1",  Unit: "mL"), (Name: "Vset_DU1",  Unit: "mL"),
            (Name: "qc",  Unit: "W")
        ];
        for (int index = 0; index < trendNameAndUnits.Length; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = trendNameAndUnits[index].Name,
                Unit = trendNameAndUnits[index].Unit,
                StartTimeUtc = startTime,
            });
        }

        var valueCount = 60 * 60 * lengthInHours;
        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.Serialize(stream, experiment, CreateTrendData(valueCount));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            ExperimentJson? readExperiment;
            using (var stream = File.OpenRead(tempFileName))
            {
                readExperiment = await JsonSerializer.DeserializeAsync<ExperimentJson>(stream, ExperimentJsonSerializer.SerializerOptions, TestContext.Current.CancellationToken);
            }

            // Assert
            readExperiment.ShouldNotBeNull();
            readExperiment.Name.ShouldBe(experiment.Name);
            var alrJson = readExperiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();
            var alrZoneJson = alrJson.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
            alrZoneJson.Receptacle.Inserts.Count.ShouldBe(alrZone.Receptacle.Inserts.Count);
            var readOccupation = GetFirstOccupation(readExperiment);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(valueCount);
            readOccupation.Trends[1].Values.Count.ShouldBe(valueCount);
            var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
            var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
            readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task Deserialize_JsonFromStageLargeTrendSet_SameValues()
    {
        var referenceTime = DateTime.UtcNow;
        int lengthInHours = 5;
        var startTime = referenceTime.AddHours(-lengthInHours);
        var experiment = new ExperimentJson()
        {
            Name = "Test",
            ExperimentStatus = ExperimentStateJson.Concluded,
            Process = { Stages = { new StageJson { StartTimeUtc = startTime, CompletionTimeUtc = referenceTime } } }
        };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };
        var temperatureParameters = new TemperatureControlParametersRampByDurationJson()
        {
            Temperature = { Actual = new UnitValueJson { Value = 40.0, Unit = "°C", SignificantDigits = 3 } },
            Duration = { Actual = new UnitValueJson { Value = 1.0, Unit = "min", SignificantDigits = 2 } },
            EndValueMargin = { Actual = new UnitValueJson { Value = 1.0, Unit = "K", SignificantDigits = 2 } }
        };
        var sampleParameters = new SingleSampleParametersJson()
        {
            SampleId = "Sample 1",
            Preparation = "Diluted",
            Quantity = { Actual = new UnitValueJson { Value = 1.5, Unit = "mL", SignificantDigits = 2 } }
        };

        AddAgitateOperation(experiment, stirParameters, startTime.AddMinutes(5), startTime.AddMinutes(6));
        AddTemperatureControlOperation(experiment, temperatureParameters, startTime.AddMinutes(7), startTime.AddMinutes(8));
        AddSampleOperation(experiment, sampleParameters, startTime.AddMinutes(10), startTime.AddMinutes(10));

        var occupation = AddAlrOccupation(experiment);
        var alrZone = GetInstrumentZone(experiment, 0);

        (string Name, string Unit)[] trendNameAndUnits = [
            (Name: "Tr", Unit: "°C"), (Name: "Tj", Unit: "°C"), (Name: "Tc", Unit: "°C"), (Name: "TSet", Unit: "°C"), (Name: "TEnd", Unit: "°C"),
            (Name: "Tr-Tj", Unit: "K"),
            (Name: "R",  Unit: "min⁻¹"), (Name: "RSet",  Unit: "min⁻¹"), (Name: "REnd",  Unit: "min⁻¹"),
            (Name: "pH",  Unit: "pH"),
            (Name: "V_DU1",  Unit: "mL"), (Name: "Vset_DU1",  Unit: "mL"),
            (Name: "qc",  Unit: "W")
        ];
        for (int index = 0; index < trendNameAndUnits.Length; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = trendNameAndUnits[index].Name,
                Unit = trendNameAndUnits[index].Unit,
                StartTimeUtc = startTime,
            });
        }

        var valueCount = 60 * 60 * lengthInHours;
        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            var stageWithMaterials = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
            foreach (var material in experiment.Materials)
            {
                stageWithMaterials.Materials.Add(material);
            }
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.SerializeStage(stream, stageWithMaterials, CreateTrendData(valueCount));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            StageWithMaterialsJson? readStage;
            using (var stream = File.OpenRead(tempFileName))
            {
                readStage = await JsonSerializer.DeserializeAsync<StageWithMaterialsJson>(stream, ExperimentJsonSerializer.SerializerOptions, TestContext.Current.CancellationToken);
            }

            // Assert
            readStage.ShouldNotBeNull();
            var readOccupation = GetFirstOccupation(readStage);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(valueCount);
            readOccupation.Trends[1].Values.Count.ShouldBe(valueCount);
            var readOperation = readStage.Stage.OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
            var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
            readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task DeserializeWithoutTrendValues_ExperimentWithTrends_NoTrendValues()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.Zero, Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5), Value = 27.2 },
            new (){ RelativeTime = TimeSpan.FromDays(2), Value = 35.2 },
        }
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.FromSeconds(0.12), Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1.12), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2.12), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3.12), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4.12), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5.12), Value = 27.2 },
        }
        });

        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, experiment, ExperimentJsonSerializer.SerializerOptions);
        stream.Seek(0, SeekOrigin.Begin);

        // Act
        var readExperiment = await ExperimentJsonSerializer.DeserializeWithoutTrendValues(stream);

        // Assert
        readExperiment.ShouldNotBeNull();
        readExperiment.Name.ShouldBe(experiment.Name);
        var readOccupation = GetFirstOccupation(readExperiment);
        readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
        readOccupation.Trends[0].Values.Count.ShouldBe(0);
        readOccupation.Trends[1].Values.Count.ShouldBe(0);
        var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
        readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
    }

    [Fact]
    public async Task DeserializeWithoutTrendValues_StageWithTrends_NoTrendValues()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.Zero, Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5), Value = 27.2 },
            new (){ RelativeTime = TimeSpan.FromDays(2), Value = 35.2 },
        }
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.FromSeconds(0.12), Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1.12), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2.12), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3.12), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4.12), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5.12), Value = 27.2 },
        }
        });

        var stream = new MemoryStream();
        var stageWithMaterials = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
        JsonSerializer.Serialize(stream, stageWithMaterials, ExperimentJsonSerializer.SerializerOptions);
        stream.Seek(0, SeekOrigin.Begin);

        // 
        var readStage = await ExperimentJsonSerializer.DeserializeStageWithoutTrendValues(stream);

        // Assert
        readStage.ShouldNotBeNull();
        var readOccupation = GetFirstOccupation(readStage);
        readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
        readOccupation.Trends[0].Values.Count.ShouldBe(0);
        readOccupation.Trends[1].Values.Count.ShouldBe(0);
        var readOperation = readStage.Stage.OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
        var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
        readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
    }

    [Fact]
    public async Task ReadTrendsExperiment_ExperimentWithTrends_Trend()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.Zero, Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5), Value = 27.2 },
            new (){ RelativeTime = TimeSpan.FromDays(2), Value = 35.2 },
        }
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.FromSeconds(0.12), Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1.12), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2.12), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3.12), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4.12), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5.12), Value = 27.2 },
        }
        });

        var stream = new MemoryStream();
        JsonSerializer.Serialize(stream, experiment, ExperimentJsonSerializer.SerializerOptions);
        stream.Seek(0, SeekOrigin.Begin);

        // Act
        List<TrendJson> readTrends = [];
        await ExperimentJsonSerializer.ReadTrends(stream, (_, trend) => { readTrends.Add(trend); return Task.CompletedTask; });

        // Assert
        readTrends.Count.ShouldBe(occupation.Trends.Count);
        var trend = occupation.Trends[0];
        var readTrend = readTrends[0];
        readTrend.ShouldNotBeNull();
        readTrend.Name.ShouldBe(trend.Name);
        readTrend.Values.Count.ShouldBe(trend.Values.Count);
        readTrend.Values[0].RelativeTime.ShouldBe(trend.Values[0].RelativeTime);
        readTrend.Values[0].Value.ShouldBe(trend.Values[0].Value);
        readTrend.Values[1].RelativeTime.ShouldBe(trend.Values[1].RelativeTime);
        readTrend.Values[1].Value.ShouldBe(trend.Values[1].Value);
    }

    [Fact]
    public async Task ReadTrendsStage_StageWithTrends_Trend()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.Zero, Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5), Value = 27.2 },
            new (){ RelativeTime = TimeSpan.FromDays(2), Value = 35.2 },
        }
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            Values = {
            new (){ RelativeTime = TimeSpan.FromSeconds(0.12), Value = 25.3 },
            new (){ RelativeTime = TimeSpan.FromSeconds(1.12), Value = 25.6 },
            new (){ RelativeTime = TimeSpan.FromSeconds(2.12), Value = 25.8 },
            new (){ RelativeTime = TimeSpan.FromSeconds(3.12), Value = 26.4 },
            new (){ RelativeTime = TimeSpan.FromSeconds(4.12), Value = 27.1 },
            new (){ RelativeTime = TimeSpan.FromSeconds(5.12), Value = 27.2 },
        }
        });

        var stream = new MemoryStream();
        var stageWithMaterials = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
        JsonSerializer.Serialize(stream, stageWithMaterials, ExperimentJsonSerializer.SerializerOptions);
        stream.Seek(0, SeekOrigin.Begin);

        // Act
        List<TrendJson> readTrends = [];
        await ExperimentJsonSerializer.ReadTrends(stream, (_, trend) => { readTrends.Add(trend); return Task.CompletedTask; });

        // Assert
        readTrends.Count.ShouldBe(occupation.Trends.Count);
        var trend = occupation.Trends[0];
        var readTrend = readTrends[0];
        readTrend.ShouldNotBeNull();
        readTrend.Name.ShouldBe(trend.Name);
        readTrend.Values.Count.ShouldBe(trend.Values.Count);
        readTrend.Values[0].RelativeTime.ShouldBe(trend.Values[0].RelativeTime);
        readTrend.Values[0].Value.ShouldBe(trend.Values[0].Value);
        readTrend.Values[1].RelativeTime.ShouldBe(trend.Values[1].RelativeTime);
        readTrend.Values[1].Value.ShouldBe(trend.Values[1].Value);
    }

    [Fact]
    public async Task Serialize_TrendsValuesProvidedSeparate_JsonStored()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);
        var alr = experiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });

        var valueCount = 200;

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        // Act
        try
        {
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.Serialize(stream, experiment, CreateTrendData(valueCount));
            }

            // Assert
            ExperimentJson? readExperiment;
            using (var stream = File.OpenRead(tempFileName))
            {
                readExperiment = JsonSerializer.Deserialize<ExperimentJson>(stream, ExperimentJsonSerializer.SerializerOptions);
            }
            readExperiment.ShouldNotBeNull();
            readExperiment.Name.ShouldBe(experiment.Name);
            var readAlr = readExperiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();
            readAlr.SoftwareVersion.ShouldBe(alr.SoftwareVersion);
            var zone = readAlr.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
            zone.Receptacle.ReceptacleName.ShouldBe(OneHundredMlTwoPieceReactorName); var readOccupation = GetFirstOccupation(readExperiment);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(valueCount);
            readOccupation.Trends[1].Values.Count.ShouldBe(valueCount);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task SerializeStage_TrendsValuesProvidedSeparate_JsonStored()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);
        var alr = experiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });

        var valueCount = 200;

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        // Act
        try
        {
            using (var stream = File.OpenWrite(tempFileName))
            {
                var stageJson = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
                await ExperimentJsonSerializer.SerializeStage(stream, stageJson, CreateTrendData(valueCount));
            }

            // Assert
            StageWithMaterialsJson? readStage;
            using (var stream = File.OpenRead(tempFileName))
            {
                readStage = JsonSerializer.Deserialize<StageWithMaterialsJson>(stream, ExperimentJsonSerializer.SerializerOptions);
            }
            readStage.ShouldNotBeNull();
            var readAlr = readStage.Stage.EquipmentCollection[0].ShouldBeOfType<AlrJson>();
            readAlr.SoftwareVersion.ShouldBe(alr.SoftwareVersion);
            var zone = readAlr.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
            zone.Receptacle.ReceptacleName.ShouldBe(OneHundredMlTwoPieceReactorName); var readOccupation = GetFirstOccupation(readStage);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(valueCount);
            readOccupation.Trends[1].Values.Count.ShouldBe(valueCount);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task Serialize_TrendsValuesProvidedSeparateWithNonNumbers_OnlyNumbersWritten()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);
        var alr = experiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        // Act
        try
        {
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.Serialize(stream, experiment, GetNonNumberTrendValues);
            }

            // Assert
            ExperimentJson? readExperiment;
            using (var stream = File.OpenRead(tempFileName))
            {
                readExperiment = JsonSerializer.Deserialize<ExperimentJson>(stream, ExperimentJsonSerializer.SerializerOptions);
            }
            readExperiment.ShouldNotBeNull();
            readExperiment.Name.ShouldBe(experiment.Name);
            var readAlr = readExperiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();
            readAlr.SoftwareVersion.ShouldBe(alr.SoftwareVersion);
            var zone = readAlr.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
            zone.Receptacle.ReceptacleName.ShouldBe(OneHundredMlTwoPieceReactorName); var readOccupation = GetFirstOccupation(readExperiment);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(1);
            readOccupation.Trends[1].Values.Count.ShouldBe(1);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task SerializeStage_TrendsValuesProvidedSeparateWithNonNumbers_OnlyNumbersWritten()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);
        var alr = experiment.Process.Stages[0].EquipmentCollection[0].ShouldBeOfType<AlrJson>();

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });
        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tj",
            Unit = "°C",
            StartTimeUtc = DateTime.UtcNow.AddHours(-5),
        });

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        // Act
        try
        {
            using (var stream = File.OpenWrite(tempFileName))
            {
                var stageJson = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
                await ExperimentJsonSerializer.SerializeStage(stream, stageJson, GetNonNumberTrendValues);
            }

            // Assert
            StageWithMaterialsJson? readStage;
            using (var stream = File.OpenRead(tempFileName))
            {
                readStage = JsonSerializer.Deserialize<StageWithMaterialsJson>(stream, ExperimentJsonSerializer.SerializerOptions);
            }
            readStage.ShouldNotBeNull();
            var readAlr = readStage.Stage.EquipmentCollection[0].ShouldBeOfType<AlrJson>();
            readAlr.SoftwareVersion.ShouldBe(alr.SoftwareVersion);
            var zone = readAlr.Zones[0].ShouldBeOfType<AlrInstrumentZoneJson>();
            zone.Receptacle.ReceptacleName.ShouldBe(OneHundredMlTwoPieceReactorName); var readOccupation = GetFirstOccupation(readStage);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(1);
            readOccupation.Trends[1].Values.Count.ShouldBe(1);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task DeserializeWithoutTrendValues_LargeTrendSet_NoTrendValues()
    {
        var experiment = new ExperimentJson() { Name = "TestExp" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        for (int index = 0; index < 20; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = $"Temp{index + 1}",
                Unit = "°C",
                StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            });
        }

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.Serialize(stream, experiment, CreateTrendData(60 * 60 * 24 * 5));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            ExperimentJson? readExperiment;
            using (var stream = File.OpenRead(tempFileName))
            {
                readExperiment = await ExperimentJsonSerializer.DeserializeWithoutTrendValues(stream);
            }

            // Assert
            readExperiment.ShouldNotBeNull();
            readExperiment.Name.ShouldBe(experiment.Name);
            var readOccupation = GetFirstOccupation(readExperiment);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(0);
            readOccupation.Trends[1].Values.Count.ShouldBe(0);
            var readOperation = readExperiment.Process.Stages[0].OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
            var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
            readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task DeserializeStageWithoutTrendValues_LargeTrendSet_NoTrendValues()
    {
        var experiment = new ExperimentJson() { Name = "TestExp" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        for (int index = 0; index < 20; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = $"Temp{index + 1}",
                Unit = "°C",
                StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            });
        }

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");

        try
        {
            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenWrite(tempFileName))
            {
                var stageJson = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
                await ExperimentJsonSerializer.SerializeStage(stream, stageJson, CreateTrendData(60 * 60 * 24 * 5));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            StageWithMaterialsJson? readStage;
            using (var stream = File.OpenRead(tempFileName))
            {
                readStage = await ExperimentJsonSerializer.DeserializeStageWithoutTrendValues(stream);
            }

            // Assert
            readStage.ShouldNotBeNull();
            var readOccupation = GetFirstOccupation(readStage);
            readOccupation.Trends.Count.ShouldBe(occupation.Trends.Count);
            readOccupation.Trends[0].Values.Count.ShouldBe(0);
            readOccupation.Trends[1].Values.Count.ShouldBe(0);
            var readOperation = readStage.Stage.OperationGroups[0].OperationSequences[0].Operations[0].ShouldBeOfType<AgitateOperationJson>();
            var readParameters = readOperation.Parameters.ShouldBeOfType<StirParametersJson>();
            readParameters.Speed.Design.ShouldNotBeNull().Value.ShouldBe(stirParameters.Speed.Design.Value);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task ReadTrends_LargeTrendSet_TrendWithValues()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        for (int index = 0; index < 20; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = $"Temp{index + 1}",
                Unit = "°C",
                StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            });
        }

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");
        var valueCount = 60 * 60 * 24 * 5;

        try
        {
            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenWrite(tempFileName))
            {
                await ExperimentJsonSerializer.Serialize(stream, experiment, CreateTrendData(valueCount));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            TrendJson? readTrend = null;
            using (var stream = File.OpenRead(tempFileName))
            {
                await ExperimentJsonSerializer.ReadTrends(stream,
                    (_, trend) =>
                    {
                        if (trend.Name == occupation.Trends[5].Name)
                        {
                            readTrend = trend;
                        }
                        return Task.CompletedTask;
                    });
            }

            // Assert
            readTrend.ShouldNotBeNull();
            readTrend.Name.ShouldBe(occupation.Trends[5].Name);
            readTrend.Values.Count.ShouldBe(valueCount);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    [Fact]
    public async Task ReadTrends_StageLargeTrendSet_TrendWithValues()
    {
        var experiment = new ExperimentJson() { Name = "Test" };
        var stirParameters = new StirParametersJson()
        {
            Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
        };

        AddAgitateOperation(experiment, stirParameters);
        var occupation = AddAlrOccupation(experiment);

        for (int index = 0; index < 20; index++)
        {
            occupation.Trends.Add(new TrendJson()
            {
                Name = $"Temp{index + 1}",
                Unit = "°C",
                StartTimeUtc = DateTime.UtcNow.AddHours(-5),
            });
        }

        var tempFileName = Path.Combine(Path.GetTempPath(), $"TrendTest_{Guid.NewGuid()}.json");
        var valueCount = 60 * 60 * 24 * 5;

        try
        {
            var stopwatch = Stopwatch.StartNew();
            using (var stream = File.OpenWrite(tempFileName))
            {
                var stageJson = new StageWithMaterialsJson() { Stage = experiment.Process.Stages[0] };
                await ExperimentJsonSerializer.SerializeStage(stream, stageJson, CreateTrendData(valueCount));
            }
            _testOuputHelper.WriteLine($"Time to serialize large trends {stopwatch.Elapsed}");

            // Act
            TrendJson? readTrend = null;
            using (var stream = File.OpenRead(tempFileName))
            {
                await ExperimentJsonSerializer.ReadTrends(stream,
                    (_, trend) =>
                    {
                        if (trend.Name == occupation.Trends[5].Name)
                        {
                            readTrend = trend;
                        }
                        return Task.CompletedTask;
                    });
            }

            // Assert
            readTrend.ShouldNotBeNull();
            readTrend.Name.ShouldBe(occupation.Trends[5].Name);
            readTrend.Values.Count.ShouldBe(valueCount);
        }
        finally
        {
            File.Delete(tempFileName);
        }
    }

    private AlrInstrumentZoneJson GetInstrumentZone(ExperimentJson experiment, int zoneIndex)
    {
        var alrInstrument = (InstrumentJson)experiment.Process.Stages[0].EquipmentCollection[0];
        return (AlrInstrumentZoneJson)alrInstrument.Zones[zoneIndex];
    }

    private Func<TrendPathJson, Task<IEnumerable<TrendValueJson>>> CreateTrendData(int numberOfValue)
    {
        return _ =>
        {
            return Task.FromResult(GetTrendValues(numberOfValue));
        };
    }

    private Task<IEnumerable<TrendValueJson>> GetNonNumberTrendValues(TrendPathJson _)
    {
        return Task.FromResult<IEnumerable<TrendValueJson>>(
            [
                new() { RelativeTime = TimeSpan.FromSeconds(0), Value = 5.0 },
                new() { RelativeTime = TimeSpan.FromSeconds(1), Value = double.NegativeInfinity },
                new() { RelativeTime = TimeSpan.FromSeconds(2), Value = double.PositiveInfinity },
                new() { RelativeTime = TimeSpan.FromSeconds(3), Value = double.NaN }
            ]);
    }

    private IEnumerable<TrendValueJson> GetTrendValues(int numberOfValue, TimeSpan offset = default)
    {
        var amplitude = Random.Shared.NextDouble() * 50 + 1;
        foreach (var index in Enumerable.Range(0, numberOfValue))
        {
            yield return new TrendValueJson { RelativeTime = offset + TimeSpan.FromSeconds(index), Value = amplitude * Math.Sin(index / 20.0) };
        }
    }

    private OccupationJson AddAlrOccupation(ExperimentJson experiment)
    {
        var occupation = new OccupationJson { TrackingId = Guid.NewGuid() };
        StageJson stage = GetOrAddStage(experiment);
        stage.EquipmentCollection.Add(new AlrJson
        {
            Model = "EasyMax 102i",
            SerialNumber = "C334342",
            AssetTag = "Test EasyMax",
            TrackingId = Guid.NewGuid(),
            SoftwareVersion = "1.0.2.1",
            Zones =
            {
                new AlrInstrumentZoneJson
                {
                    Receptacle =
                    {
                        ReceptacleName = OneHundredMlTwoPieceReactorName,
                        ReactorVesselNames = { OneHundredMlTwoPieceReactorName },
                        Inserts = { new InsertReferenceJson { InsertType = InsertTypesJson.Stirrer, Name = "Anchor, glass, Ø 48mm, Length 261mm", PartOrderNumber = "51161727" } }
                    },
                    Occupations =
                    {
                        occupation
                    },
                }
            }
        });

        return occupation;
    }

    private static void AddAgitateOperation(ExperimentJson experiment, AgitateParametersJson stirParameters,
        DateTime? startTime = null, DateTime? completionTime = null)
    {
        StageJson stage = GetOrAddStage(experiment);
        AddOperationInNewGroup(stage, new AgitateOperationJson
        {
            TrackingId = Guid.NewGuid(),
            StartTimeUtc = startTime,
            CompletionTimeUtc = completionTime,
            CompletionStatus = completionTime.HasValue ? OperationJson.CompletionStatusEnum.Complete : null,
            Parameters = stirParameters
        });
    }

    private void AddTemperatureControlOperation(ExperimentJson experiment, TemperatureControlParametersJson temperatureParameters,
        DateTime? startTime = null, DateTime? completionTime = null)
    {
        StageJson stage = GetOrAddStage(experiment);
        AddOperationInNewGroup(stage, new TemperatureControlOperationJson
        {
            TrackingId = Guid.NewGuid(),
            StartTimeUtc = startTime,
            CompletionTimeUtc = completionTime,
            CompletionStatus = completionTime.HasValue ? OperationJson.CompletionStatusEnum.Complete : null,
            Parameters = temperatureParameters
        });
    }

    private void AddSampleOperation(ExperimentJson experiment, SingleSampleParametersJson sampleParameters,
        DateTime? startTime = null, DateTime? completionTime = null)
    {
        StageJson stage = GetOrAddStage(experiment);

        var sampleOperationJson = new SampleOperationJson
        {
            TrackingId = Guid.NewGuid(),
            StartTimeUtc = startTime,
            CompletionTimeUtc = completionTime,
            CompletionStatus = completionTime.HasValue ? OperationJson.CompletionStatusEnum.Complete : null,
            Parameters = sampleParameters
        };

        if (startTime.HasValue)
        {
            sampleOperationJson.CollectedSamples.Add(new SampleJson { Id = sampleParameters.SampleId, TimeUtc = startTime.Value });
        }

        AddOperationInNewGroup(stage, sampleOperationJson);
    }

    private static void AddOperationInNewGroup(StageJson stage, OperationJson operation)
    {
        stage.OperationGroups.Add(new OperationGroupJson()
        {
            OperationSequences = { new OperationSequenceJson()
            {
                Operations= { operation }
            }
            }
        });
    }

    private static StageJson GetOrAddStage(ExperimentJson experiment)
    {
        var process = experiment.Process;
        if (process.Stages.Count == 0)
        {
            process.Stages.Add(new StageJson());
        }

        var stage = process.Stages[0];
        return stage;
    }

    private OccupationJson GetFirstOccupation(ExperimentJson readExperiment)
    {
        var instruments = readExperiment.Process.Stages.SelectMany(x => x.EquipmentCollection.OfType<InstrumentJson>());
        return instruments.SelectMany(x => x.Zones).SelectMany(x => x.Occupations).First();
    }
    private OccupationJson GetFirstOccupation(StageWithMaterialsJson readStage)
    {
        var instruments = readStage.Stage.EquipmentCollection.OfType<InstrumentJson>();
        return instruments.SelectMany(x => x.Zones).SelectMany(x => x.Occupations).First();
    }
}
