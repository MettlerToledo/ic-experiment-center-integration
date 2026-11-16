using System.Text.Json;
using System.Text.Json.Nodes;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Evaluators;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Parameters;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Samples;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Trends;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;
using Json.More;
using Json.Schema;
using Shouldly;
using Xunit;
using MettlerToledo.Labs.Experiments.Schemas;

namespace MettlerToledo.Labs.Experiments.UnitTests;

public class ExperimentSchemaTests
{
    [Fact]
    public async Task JsonFromExperiment_ValidExperiment_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var experiment = new ExperimentJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Experiment 2024-11-13 14-38",
            Process =
            {
                TrackingId = Guid.NewGuid(),
                ProcessType = ProcessTypeJson.Synthesis
            }
        };

        var ethanol = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Ethanol",
            LotNumber = "EA33233",
            Quantity =
            {
                Design = new UnitValueJson { Value = 32.1, Unit = "mL", SignificantDigits = 3 }
            }
        };

        experiment.Materials.Add(ethanol);

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            TrackingId = Guid.NewGuid()
        };

        var stage = new StageJson()
        {
            TrackingId = Guid.NewGuid(),
            StageType = StageTypeJson.ReactionStage,
            OperationGroups = { new OperationGroupJson()
            {
                OperationSequences = { new OperationSequenceJson()
                {
                    Operations=
                    {
                        new AgitateOperationJson
                        {
                            TrackingId= Guid.NewGuid(),
                            Parameters = new StirParametersJson()
                            {
                                Speed = { Design = new UnitValueJson { Value = 100, Unit = "min-1" } }
                            }
                        },
                        new ChargeOperationJson
                        {
                            TrackingId = Guid.NewGuid(),
                            Peripherals = [new EquipmentReferenceJson {TrackingId = dosingUnit.TrackingId }],
                            Material = new MaterialPortionReferenceJson {TrackingId = ethanol.TrackingId },
                            Parameters = new ChargeByRateParametersJson
                            {
                                Rate = { Design = new UnitValueJson { Value = 1.3, Unit = "mL/min" } }
                            }
                        }
                    }
                }
                }
            }
            }
        };

        var zone = GetOrAddInstrumentZone(stage);
        zone.Peripherals.Add(dosingUnit);

        experiment.Process.Stages.Add(stage);
        var occupation = AddAlrOccupation(stage);
        occupation.StartTimeUtc = referenceTime.AddHours(-5);
        occupation.ReleaseTimeUtc = referenceTime;

        occupation.Trends.Add(new TrendJson()
        {
            Name = "Tr",
            Unit = "°C",
            StartTimeUtc = referenceTime.AddHours(-5),
            Values = {
                new (){ RelativeTime = TimeSpan.Zero, Value = 25.3 },
                new (){ RelativeTime = TimeSpan.FromSeconds(1), Value = 25.6 },
                new (){ RelativeTime = TimeSpan.FromSeconds(2), Value = 25.8 },
                new (){ RelativeTime = TimeSpan.FromSeconds(3), Value = 26.4 },
                new (){ RelativeTime = TimeSpan.FromSeconds(4), Value = 27.1 },
                new (){ RelativeTime = TimeSpan.FromSeconds(5), Value = 27.2 },
                new (){ RelativeTime = TimeSpan.FromHours(2), Value = 35.2 },
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

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromExperiment_PopulatedIds_SchemaValidationPasses()
    {
        var experiment = CreateExperimentWithOperation();
        experiment.Id = Guid.NewGuid();
        experiment.Process.Stages[0].Id = Guid.NewGuid();

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task JsonFromExperiment_NullIds_SchemaValidationPasses()
    {
        var experiment = CreateExperimentWithOperation();
        experiment.Id = null;
        experiment.Process.Stages[0].Id = null;

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task Json_ValidExperiment_SchemaValidationPasses()
    {
        /*lang=json,strict*/
        var json =
            """
            {
              "schemaVersion":"","uniqueElnId":"","name":"","suffix":"","user":"","project":"","timeZone":"","experimentStatus":"Planning","validityStatus":"Valid",
              "process":{
                "processType":"None","stages":[{
                  "startTimeUtc":null,"completionTimeUtc":null,"stageType":"Unknown","equipmentCollection":[{
                    "$type":"Alr",
                    "zones":[{
                      "$type":"AlrInstrumentZone",
                      "receptacle":{"receptacleName":"100 mL two-piece","reactorVesselNames":["100 mL two-piece"],"inserts":[]},"peripherals":[],
                      "occupations":[{
                        "startTimeUtc":null,"releaseTimeUtc":null,
                        "trends":[
                          {"name":"Tr","unit":"\u00B0C","startTimeUtc":"2025-02-25T15:25:02.0433731Z","values":[{"t":"00:00:00","v":25.3},{"t":"00:00:01","v":25.6},{"t":"00:00:02","v":25.8},{"t":"00:00:03","v":26.4},{"t":"00:00:04","v":27.1},{"t":"00:00:05","v":27.2},{"t":"2.00:00:00","v":35.2}]},
                          {"name":"Tj","unit":"\u00B0C","startTimeUtc":"2025-02-25T15:25:02.0434513Z","values":[{"t":"00:00:00.1200000","v":25.3},{"t":"00:00:01.1200000","v":25.6},{"t":"00:00:02.1200000","v":25.8},{"t":"00:00:03.1200000","v":26.4},{"t":"00:00:04.1200000","v":27.1},{"t":"00:00:05.1200000","v":27.2}]}
                        ],
                        "trackingId":"f4ec7553-4311-4308-a8b0-6dea332230cc"
                      }]
                    }],
                    "softwareVersion":"1.0.1.5","model":"EasyMax 102i","serialNumber":"C334342","assetTag":"Test EasyMax","description":"","category":"Unknown","trackingId":"f7f1af97-47fd-4a8b-854a-75580398739c"
                  }],
                  "operationGroups":[{
                    "operationSequences":[{
                      "operations":[{
                        "$type":"ControlPhOperation",
                        "peripherals":[],
                        "equipment":{"trackingId":"f7f1af97-47fd-4a8b-854a-75580398739c"},
                        "parameters":{
                          "$type":"ControlPhParametersRampByRate",
                          "dosingType":"Acid",
                          "material":{"trackingId":"ae535e47-71c0-4ed4-9407-51ad7bd94b19"},
                          "ph":{"design":null,"actual":{"value":7.5,"unit":"pH","significantDigits":2}},
                          "rate":{"design":null,"actual":{"value":0.25,"unit":"pH/min","significantDigits":2}}
                        },
                        "startTimeUtc":"2025-02-25T20:24:12.0416998Z","completionTimeUtc":"2025-02-25T20:25:02.0416998Z","completionStatus":null,"trackingId":"00000000-0000-0000-0000-000000000000"
                      }]
                    }]
                  }],
                  "trackingId":"5dcad2b6-944e-4708-89d6-f232e135f619"
                }],
                "trackingId":"00000000-0000-0000-0000-000000000000"
              },
              "materials":[{"name":"Ethanol","role":"Undefined","lotNumber":"EA33233","density":null,"quantity":{"design":null,"actual":null},"trackingId":"ae535e47-71c0-4ed4-9407-51ad7bd94b19"}],
              "customMetadata":null,"trackingId":"00000000-0000-0000-0000-000000000000"
            }
            """;

        var schema = await GetExperimentSchema();

        var testJson = JsonNode.Parse(json);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task JsonFromExperiment_ElnIdTooLong_SchemaValidationFails()
    {
        var experiment = new ExperimentJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Experiment 2024-11-13 14-38",
            UniqueElnId = "0123456789_123456789_123456789_123456789_123456789_123",
            Process =
            {
                TrackingId = Guid.NewGuid(),
                ProcessType = ProcessTypeJson.Synthesis
            }
        };

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task StirrerInsert_Valid_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var sampleOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new StirParametersJson
            {
                StirrerType = StirrerTypeJson.Overhead,
                Speed = { Actual = new UnitValueJson { Value = 125, Unit = "min-1", SignificantDigits = 3 } }
            }
        };

        var experiment = CreateExperimentWithOperation(sampleOperation);
        var instrumentZone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        instrumentZone.Receptacle.Inserts.Add(new InsertReferenceJson
        {
            InsertType = InsertTypesJson.Stirrer,
            Name = "Anchor, C22, Ø 48mm / Magnetic drive shaft, C22, Length 94mm",
            PartOrderNumber = "51161724 / 30130412"
        });

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ShakeParameters_WithSpeed_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new ShakeParametersJson
            {
                Speed = new UnitValueParameterJson
                {
                    Design = new UnitValueJson { Value = 200, Unit = "min-1", SignificantDigits = 3 }
                }
            }
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ShakeParameters_NoSpeed_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new ShakeParametersJson()
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task VortexParameters_WithSpeed_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new VortexParametersJson
            {
                Speed = new UnitValueParameterJson
                {
                    Actual = new UnitValueJson { Value = 1500, Unit = "min-1", SignificantDigits = 4 }
                }
            }
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task VortexParameters_NoSpeed_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new VortexParametersJson()
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task SwirlParameters_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new SwirlParametersJson()
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task SonicParameters_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var agitateOperation = new AgitateOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new SonicParametersJson()
        };

        var experiment = CreateExperimentWithOperation(agitateOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = EvaluateJson(schema, testJson);

        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task PeripheralJson_EcbSensor_SchemaValidationPasses()
    {
        var sensor = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Sensor,
            Identifier = "ECB-1/Sensor-1",
            Path = [new("ECB", 0), new("Sensor", 0)],
            AssignedName = "Pressure",
            SignalType = "Voltage",
            Unit = "bar",
            TrackingId = Guid.NewGuid()
        };

        var experiment = CreateExperimentWithOperation();
        var instrumentZone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        instrumentZone.Peripherals.Add(sensor);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TemperatureControlOperation_ByDuration_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersRampByDurationJson
            {
                Temperature = { Design = new UnitValueJson { Value = 35, Unit = "°C", SignificantDigits = 3 } },
                Duration = { Design = new UnitValueJson { Value = 5, Unit = "min", SignificantDigits = 1 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TemperatureControlOperation_ByRate_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersRampByRateJson
            {
                TemperatureControlMode = TemperatureControlModesJson.Reactor,
                Temperature = { Design = new UnitValueJson { Value = 35, Unit = "°C", SignificantDigits = 3 } },
                Rate = { Design = new UnitValueJson { Value = 2.5, Unit = "K/min", SignificantDigits = 2 } },
                EndValueMargin = { Design = new UnitValueJson { Value = 0.5, Unit = "K", SignificantDigits = 1 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Theory]
    [InlineData(TemperatureControlModesJson.Jacket)]
    [InlineData(TemperatureControlModesJson.Reactor)]
    public async Task TemperatureControlOperation_ByExpression_SchemaValidationPasses(TemperatureControlModesJson controlMode)
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersByExpressionJson
            {
                TemperatureControlMode = controlMode,
                TemperatureExpression = "35 °C + Time * 0.1 K/min",
                EndConditionExpression = "Time >= 10 min || Temperature >= 50 °C",
                MaximumTemperature = { Design = new UnitValueJson { Value = 60, Unit = "°C", SignificantDigits = 3 } },
                MinimumTemperature = { Design = new UnitValueJson { Value = 20, Unit = "°C", SignificantDigits = 3 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TemperatureControlOperation_NonLinear_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersNonLinearJson
            {
                Temperature = { Design = new UnitValueJson { Value = 35, Unit = "°C", SignificantDigits = 3 } },
                Duration = { Design = new UnitValueJson { Value = 5, Unit = "min", SignificantDigits = 1 } },
                Exponent = 1.8
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TemperatureControlOperation_DeltaJacketReactor_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersDeltaJson
            {
                EndJacketTemperature = { Design = new UnitValueJson { Value = 80, Unit = "°C", SignificantDigits = 3 } },
                TjTrDelta = { Design = new UnitValueJson { Value = 5, Unit = "K", SignificantDigits = 2 } },
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TemperatureControlOperation_Off_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TemperatureControlOperationJson
        {
            Parameters = new TemperatureControlParametersOffJson(),
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task SampleOperation_Complete_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var sampleOperation = new SampleOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new SingleSampleParametersJson
            {
                SampleId = "Sample 2233",
                Quantity = { Actual = new UnitValueJson { Value = 25, Unit = "μL", SignificantDigits = 3 } }
            },
            CollectedSamples = {
                new SampleJson
                {
                    Id = "Sample 2233",
                    TimeUtc = referenceTime.AddSeconds(-20),
                }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(sampleOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task SampleOperation_AutomatedComplete_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var sampler = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Sampler,
            Identifier = "EasySampler 1",
            Path = [new("EasySampler", 0)],
            TrackingId = Guid.NewGuid()
        };
        var sampleOperation = new SampleOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Peripherals = [new EquipmentReferenceJson { TrackingId = sampler.TrackingId }],
            Parameters = new SingleSampleParametersJson
            {
                SampleId = "Sample 2235",
                DilutionFactor = { Actual = new UnitValueJson { Value = 50, Unit = string.Empty } },
                Quantity = { Actual = new UnitValueJson { Value = 25, Unit = "μL", SignificantDigits = 3 } }
            },
            CollectedSamples =
            {
                new SampleJson
                {
                    Id = "Sample 2235",
                    TimeUtc = referenceTime.AddSeconds(-20),
                }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(sampleOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task SampleOperation_AutomatedSequenceComplete_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var sampler = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Sampler,
            Identifier = "EasySampler 1",
            Path = [new("EasySampler", 0)],
            TrackingId = Guid.NewGuid()
        };
        var sampleOperation = new SampleOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Peripherals = [new EquipmentReferenceJson { TrackingId = sampler.TrackingId }],
            Parameters = new SequenceSampleParametersJson
            {
                SequenceName = "4 over 10 minutes"
            },
            CollectedSamples =
            {
                new SampleJson
                {
                    Id = "Sample 4",
                    TimeUtc = referenceTime.AddMinutes(-9),
                },
                new SampleJson
                {
                    Id = "Sample 5",
                    TimeUtc = referenceTime.AddMinutes(-6.5),
                },
                new SampleJson
                {
                    Id = "Sample 6",
                    TimeUtc = referenceTime.AddMinutes(-4),
                },
                new SampleJson
                {
                    Id = "Sample 7",
                    TimeUtc = referenceTime.AddSeconds(-1.5),
                }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(sampleOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ReleaseOperation_DefinedEndValues_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var sampleOperation = new ReleaseOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new ReleaseParametersDefinedJson
            {
                Temperature = { Actual = new UnitValueJson { Value = 25, Unit = "°C", SignificantDigits = 2 } },
                StirringSpeed = { Actual = new UnitValueJson { Value = 0, Unit = "min-1", SignificantDigits = 0 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(sampleOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ReleaseOperation_KeepCurrent_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var releaseOperation = new ReleaseOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new ReleaseParametersCurrentJson(),
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(releaseOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task WaitOperation_ByDuration_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var waitOperation = new WaitOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new WaitParametersByDuration
            {
                Duration = { Design = new UnitValueJson { Value = 1, Unit = "min", SignificantDigits = 1 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(waitOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task WaitOperation_ByEndCondition_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var waitOperation = new WaitOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new WaitParametersByEndCondition
            {
                EndCondition = "Crystals are forming"
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(waitOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task WaitOperation_ByExpression_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;

        var waitOperation = new WaitOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Parameters = new WaitParametersByExpressionJson
            {
                Expression = "Tr > 85"
            },
        };

        var experiment = CreateExperimentWithOperation(waitOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task TimeMarkerOperation_Valid_SchemaValidationPasses()
    {
        var now = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new TimeMarkerOperationJson
        {
            Name = "Reaction Start",
            MarkedTime = now,
            StartTimeUtc = now,
            CompletionStatus = OperationJson.CompletionStatusEnum.Complete,
            CompletionTimeUtc = now,
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ObservationOperation_Valid_SchemaValidationPasses()
    {
        var now = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var temperatureControlJosn = new ObservationOperationJson
        {
            ObservationTime = now,
            Notes = "A change happened",
            StartTimeUtc = now,
            CompletionStatus = OperationJson.CompletionStatusEnum.Complete,
            CompletionTimeUtc = now,
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(temperatureControlJosn);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ChargeOperation_ByDuration_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var ethanol = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Ethanol",
            LotNumber = "EA33233",
            Quantity =
            {
                Design = new UnitValueJson { Value = 32.1, Unit = "mL", SignificantDigits = 3 }
            }
        };

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            SignalType = "TTL",
            TrackingId = Guid.NewGuid(),
        };

        var chargeOperationJson = new ChargeOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = dosingUnit.TrackingId }],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            Parameters = new ChargeByDurationParametersJson
            {
                Duration = { Design = new UnitValueJson { Value = 2.5, Unit = "min" } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(chargeOperationJson);
        experiment.Materials.Add(ethanol);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(dosingUnit);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ChargeOperation_ByAmountExpression_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var ethanol = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Ethanol",
            LotNumber = "EA33233",
            Quantity =
            {
                Design = new UnitValueJson { Value = 32.1, Unit = "mL", SignificantDigits = 3 }
            }
        };

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            SignalType = "TTL",
            TrackingId = Guid.NewGuid(),
        };

        var chargeOperationJson = new ChargeOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = dosingUnit.TrackingId }],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            Parameters = new ChargeByAmountExpressionParametersJson
            {
                AmountExpression = "(Tr - 25 C) * 0.1 ml/K",
                SuspendConditionExpression = "Tr >= 50 C",
                EndConditionExpression = "Tr >= 60 C",
                MaximumAmount = { Design = new UnitValueJson { Value = 30, Unit = "mL", SignificantDigits = 2 } },
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(chargeOperationJson);
        experiment.Materials.Add(ethanol);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(dosingUnit);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ChargeOperation_ByRateExpression_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };
        var ethanol = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Ethanol",
            LotNumber = "EA33233",
            Quantity =
            {
                Design = new UnitValueJson { Value = 32.1, Unit = "mL", SignificantDigits = 3 }
            }
        };

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            SignalType = "TTL",
            TrackingId = Guid.NewGuid(),
        };

        var chargeOperationJson = new ChargeOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = dosingUnit.TrackingId }],
            Material = new MaterialPortionReferenceJson { TrackingId = ethanol.TrackingId },
            Parameters = new ChargeByRateExpressionParametersJson
            {
                RateExpression = "(Tr - 25 C) * 0.1 ml/min*K",
                MaximumRate = { Design = new UnitValueJson { Value = 5, Unit = "mL/min", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(chargeOperationJson);
        experiment.Materials.Add(ethanol);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(dosingUnit);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Theory]
    [InlineData(PhDosingTypeJson.Acid)]
    [InlineData(PhDosingTypeJson.Base)]
    public async Task PhOperation_Complete_SchemaValidationPasses(PhDosingTypeJson phDosingType)
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var material = CreateMaterial();

        var pump = new PeripheralJson
        {
            Category = EquipmentCategoryJson.LiquidPump,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid()
        };
        var balance = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Balance,
            Identifier = "ECB-1/Port-1",
            Path = [new("ECB", 0), new("Balance", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlPhOperation = new ControlPhOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Peripherals = [
                new EquipmentReferenceJson { TrackingId = pump.TrackingId },
                new EquipmentReferenceJson { TrackingId = balance.TrackingId },
            ],
            Parameters = new ControlPhParametersRampByRateJson
            {
                DosingType = phDosingType,
                Material = new MaterialPortionReferenceJson { TrackingId = material.TrackingId },
                Ph = { Actual = new UnitValueJson { Value = 7.5, Unit = "pH", SignificantDigits = 2 } },
                Rate = { Actual = new UnitValueJson { Value = 0.25, Unit = "pH/min", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlPhOperation);
        experiment.Materials.Add(material);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(pump);
        zone.Peripherals.Add(balance);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Theory]
    [InlineData(PhDosingTypeJson.Acid)]
    [InlineData(PhDosingTypeJson.Base)]
    public async Task PhOperation_HoldActual_SchemaValidationPasses(PhDosingTypeJson phDosingType)
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var material = CreateMaterial();

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlPhOperation = new ControlPhOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Peripherals = [
                new EquipmentReferenceJson { TrackingId = dosingUnit.TrackingId }
            ],
            Parameters = new ControlPhParametersHoldActualJson
            {
                DosingType = phDosingType,
                Material = new MaterialPortionReferenceJson { TrackingId = material.TrackingId },
                MaxChargedQuantity = { Design = new UnitValueJson { Value = 10, Unit = "mL", SignificantDigits = 2 } },
                MixingTime = { Design = new UnitValueJson { Value = 15, Unit = "s", SignificantDigits = 2 } },
                MinPhDeviation = { Design = new UnitValueJson { Value = 0.1, Unit = "pH", SignificantDigits = 2 } },
                PhStabilityRate = { Design = new UnitValueJson { Value = 0.05, Unit = "pH/min", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlPhOperation);
        experiment.Materials.Add(material);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(dosingUnit);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task PhOperation_SwitchOff_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var material = CreateMaterial();

        var dosingUnit = new PeripheralJson
        {
            Category = EquipmentCategoryJson.VolumetricDosing,
            Identifier = "DU SP-50 1",
            Path = [new("DosingUnit", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlPhOperation = new ControlPhOperationJson
        {
            StartTimeUtc = referenceTime.AddSeconds(-50),
            CompletionTimeUtc = referenceTime,
            Peripherals = [
                new EquipmentReferenceJson { TrackingId = dosingUnit.TrackingId }
            ],
            Parameters = new ControlPhParametersSwitchOffJson(),
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlPhOperation);
        experiment.Materials.Add(material);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(dosingUnit);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task ControlBalanceOperation_OnWithStable_SchemaValidationPasses(bool waitForStable)
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var material = CreateMaterial();

        var balance = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Balance,
            Identifier = "ECB-1/Port-1",
            Path = [new("ECB", 0), new("Balance", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlBalanceOperation = new ControlBalanceOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = balance.TrackingId }],
            Parameters = new ControlBalanceOnParametersJson
            {
                Material = new MaterialPortionReferenceJson { TrackingId = material.TrackingId },
                WaitForStable = waitForStable,
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlBalanceOperation);
        experiment.Materials.Add(material);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(balance);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlBalanceOperation_Off_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var material = CreateMaterial();

        var balance = new PeripheralJson
        {
            Category = EquipmentCategoryJson.Balance,
            Identifier = "ECB-1/Port-1",
            Path = [new("ECB", 0), new("Balance", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlBalanceOperation = new ControlBalanceOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = balance.TrackingId }],
            Parameters = new ControlBalanceOffParametersJson(),
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlBalanceOperation);
        experiment.Materials.Add(material);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(balance);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlFinalControlElementOperationJson_On_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var finalControlElement = new PeripheralJson
        {
            Category = EquipmentCategoryJson.FinalControlElement,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid(),
            AssignedName = "My Final Control Element",
            SignalType = "4-20mA"
        };

        var controlFinalControlElementOperation = new ControlFinalControlElementOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = finalControlElement.TrackingId }],
            Parameters = new ControlFinalControlElementOnParametersJson
            {
                Value = { Actual = new UnitValueJson { Value = 5.1, Unit = "Hz", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlFinalControlElementOperation);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(finalControlElement);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlFinalControlElementOperationJson_OnDuration_SchemaValidationPasses()
    {
        var referenceTime = DateTime.UtcNow;
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var finalControlElement = new PeripheralJson
        {
            Category = EquipmentCategoryJson.FinalControlElement,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid(),
            AssignedName = "My Final Control Element",
            SignalType = "voltage"
        };

        var controlFinalControlElementOperation = new ControlFinalControlElementOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = finalControlElement.TrackingId }],
            Parameters = new ControlFinalControlElementOnDurationParametersJson
            {
                Value = { Actual = new UnitValueJson { Value = 30.1, Unit = "Watt", SignificantDigits = 3 } },
                Duration = { Actual = new UnitValueJson { Value = 1.5, Unit = "min", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlFinalControlElementOperation);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(finalControlElement);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlFinalControlElementOperationJson_RampByDuration_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var finalControlElement = new PeripheralJson
        {
            Category = EquipmentCategoryJson.FinalControlElement,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlFinalControlElementOperation = new ControlFinalControlElementOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = finalControlElement.TrackingId }],
            Parameters = new ControlFinalControlElementRampByDurationParametersJson
            {
                EndValue = { Actual = new UnitValueJson { Value = 350, Unit = "rpm", SignificantDigits = 3 } },
                Duration = { Actual = new UnitValueJson { Value = 1.5, Unit = "min", SignificantDigits = 2 } }
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlFinalControlElementOperation);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(finalControlElement);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlFinalControlElementOperationJson_ByExpression_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var finalControlElement = new PeripheralJson
        {
            Category = EquipmentCategoryJson.FinalControlElement,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlFinalControlElementOperation = new ControlFinalControlElementOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = finalControlElement.TrackingId }],
            Parameters = new ControlFinalControlElementByExpressionParametersJson
            {
                ControlExpression = "50 rpm/K * (Tr - 20°C)",
                EndConditionExpression = "Tr > 100°C"
            },
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlFinalControlElementOperation);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(finalControlElement);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task ControlFinalControlElementOperationJson_Off_SchemaValidationPasses()
    {
        var alr = new AlrJson() { Category = EquipmentCategoryJson.Alr, TrackingId = Guid.NewGuid() };

        var finalControlElement = new PeripheralJson
        {
            Category = EquipmentCategoryJson.FinalControlElement,
            Identifier = "ECB-1/Control-1",
            Path = [new("ECB", 0), new("ControlPort", 0)],
            TrackingId = Guid.NewGuid()
        };

        var controlFinalControlElementOperation = new ControlFinalControlElementOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Peripherals = [new EquipmentReferenceJson { TrackingId = finalControlElement.TrackingId }],
            Parameters = new ControlFinalControlElementOffParametersJson(),
            Equipment = new EquipmentReferenceJson { TrackingId = alr.TrackingId }
        };

        var experiment = CreateExperimentWithOperation(controlFinalControlElementOperation);

        var zone = GetOrAddInstrumentZone(experiment.Process.Stages[0]);
        zone.Peripherals.Add(finalControlElement);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task DynamicDischargeOperation_NullValueInStringParameter_SchemaValidationPasses()
    {
        var dryOperation = new DynamicOperationJson
        {
            TrackingId = Guid.NewGuid(),
            OperationType = OperationTypeJson.Discharge,
            Parameters = [
                new DynamicStringParameterJson
                {
                    DisplayName = "Description",
                    InternalName = "Description",
                    StringValue = null
                }
            ]

        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task DynamicDryOperation_ValidValues_SchemaValidationPasses()
    {
        var crystalsInSolvent = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Crystals in Solvent",
            Quantity =
            {
                Design = new UnitValueJson { Value = 32.1, Unit = "mL", SignificantDigits = 3 }
            }
        };

        var crystals = new MaterialPortionJson
        {
            TrackingId = Guid.NewGuid(),
            Name = "Crystals",
        };

        var dryOperation = new DynamicOperationJson
        {
            TrackingId = Guid.NewGuid(),
            OperationType = OperationTypeJson.Dry,
            Parameters = [
                new DynamicMaterialReferenceParameterJson
                {
                    DisplayName = "Wet Content",
                    InternalName = "WetContent",
                    Material = new MaterialPortionReferenceJson { TrackingId = crystalsInSolvent.TrackingId }
                },
                new DynamicMaterialReferenceParameterJson
                {
                    DisplayName = "Dry Content",
                    InternalName = "DryContent",
                    Material = new MaterialPortionReferenceJson { TrackingId = crystals.TrackingId },
                    MaterialPortionUsage = MaterialPortionUsageJson.Output
                },
                new DynamicBooleanParameterJson
                {
                    DisplayName = "With Vacuum",
                    InternalName = "WithVacuum",
                    Value = true
                },
                new DynamicUnitValueParameterJson
                {
                    DisplayName = "Pressure",
                    InternalName = "Pressure",
                    Design = new UnitValueJson { Value = 50, Unit = "mbar", SignificantDigits = 2 },
                },
                new DynamicStringParameterJson
                {
                    DisplayName = "Carrier Gas",
                    InternalName = "CarrierGas",
                    StringValue = "Nitrogen"
                }
            ]

        };

        var experiment = CreateExperimentWithOperation(dryOperation);
        experiment.Materials.Add(crystalsInSolvent);
        experiment.Materials.Add(crystals);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task DynamicInertOperation_NullDisplayNameParameter_SchemaValidationPasses()
    {
        var dryOperation = new DynamicOperationJson
        {
            TrackingId = Guid.NewGuid(),
            OperationType = OperationTypeJson.Discharge,
            Parameters = [
                new DynamicStringParameterJson
                {
                    DisplayName = null,
                    InternalName = "InertGas",
                    StringValue = "Nitrogen"
                }
            ]

        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        var errorMessages = GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task RecordVirtualVolumeOperation_NoObservedVolume_SchemaValidationPasses()
    {
        var virtualVolumeOperation = new RecordVirtualVolumeOperationJson
        {
            TrackingId = Guid.NewGuid(),
        };

        var experiment = CreateExperimentWithOperation(virtualVolumeOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task RecordVirtualVolumeOperation_WithObservedVolume_SchemaValidationPasses()
    {
        var virtualVolumeOperation = new RecordVirtualVolumeOperationJson
        {
            TrackingId = Guid.NewGuid(),
            ObservedVolume = new UnitValueJson { Value = 15, Unit = "mL", SignificantDigits = 2 }
        };

        var experiment = CreateExperimentWithOperation(virtualVolumeOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task ResetQrtcIntegralOperation_Normal_SchemaValidationPasses()
    {
        var dryOperation = new ResetQrtcIntegralOperationJson
        {
            TrackingId = Guid.NewGuid(),
        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Theory]
    [InlineData(CalorimetryDeterminationSequenceJson.U, CalibrationPowerLevelJson.RecommendedPower)]
    [InlineData(CalorimetryDeterminationSequenceJson.Cpr, CalibrationPowerLevelJson.FiftyPercentPower)]
    [InlineData(CalorimetryDeterminationSequenceJson.UAndCpr, CalibrationPowerLevelJson.TwentyFivePercentPower)]
    [InlineData(CalorimetryDeterminationSequenceJson.CprAndU, CalibrationPowerLevelJson.RecommendedPower)]
    [InlineData(CalorimetryDeterminationSequenceJson.UAndCprAndU, CalibrationPowerLevelJson.FiftyPercentPower)]
    public async Task HFCalorimetryDeterminationOperation_Standard_SchemaValidationPasses(
        CalorimetryDeterminationSequenceJson sequence, CalibrationPowerLevelJson powerLevel)
    {
        var dryOperation = new HFCalorimetryDeterminationOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Parameters = new HFCalorimetryDeterminationParametersStandardJson
            {
                DeterminationSequence = sequence,
                PowerLevel = powerLevel,
                WaitingTime = { Design = new UnitValueJson { Value = 10, Unit = "min", SignificantDigits = 2 } },
                DeltaTr = { Design = new UnitValueJson { Value = 5, Unit = "K", SignificantDigits = 2 } }
            }
        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Theory]
    [InlineData(CalibrationPowerLevelJson.RecommendedPower)]
    [InlineData(CalibrationPowerLevelJson.FiftyPercentPower)]
    [InlineData(CalibrationPowerLevelJson.TwentyFivePercentPower)]
    public async Task HFCalorimetryDeterminationOperation_Quick_SchemaValidationPasses(
        CalibrationPowerLevelJson powerLevel)
    {
        var dryOperation = new HFCalorimetryDeterminationOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Parameters = new HFCalorimetryDeterminationParametersQuickJson
            {
                PowerLevel = powerLevel,
                WaitingTime = { Design = new UnitValueJson { Value = 7.5, Unit = "min", SignificantDigits = 2 } },
            }
        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        GetOrAddInstrumentZone(experiment.Process.Stages[0]);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task RFCalorimetryDeterminationOperation_Normal_SchemaValidationPasses()
    {
        var dryOperation = new RTCalorimetryDeterminationOperationJson
        {
            TrackingId = Guid.NewGuid(),
            Parameters = new RTCalorimetryDeterminationParametersJson
            {
                WaitingTime = { Design = new UnitValueJson { Value = 7.5, Unit = "min", SignificantDigits = 2 } },
                DeltaTr = { Design = new UnitValueJson { Value = 5, Unit = "K", SignificantDigits = 2 } }
            }
        };

        var experiment = CreateExperimentWithOperation(dryOperation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task Evaluators_WithTrends_SchemaValidationPasses()
    {
        var experiment = CreateExperimentWithOperation();

        var instrument = GetOrAddReactIrInstrument(experiment.Process.Stages[0]);
        var zone = GetOrAddZone(instrument);
        var occupation = new OccupationJson
        {
            TrackingId = Guid.NewGuid(),
            Evaluators = {
                new EvaluatorConfigurationJson {
                    Name = "PeakEvaluator",
                    Trends = {
                        new TrendDefinitionJson {
                            Name = "Trend1"
                        },
                        new TrendDefinitionJson {
                            Name = "Trend2",
                        }
                    },
                    Parameters = {
                        new EvaluatorParameterJson { Name = "WaterVaporCorrection", Value = "Applied" },
                        new EvaluatorParameterJson { Name = "SubtractSpectrum", Value = "None" },
                        new EvaluatorParameterJson { Name = "BaselineCorrection", Value = "None" },
                        new EvaluatorParameterJson { Name = "Normalize", Value = "2000 cm-1" },
                        new EvaluatorParameterJson { Name = "SpectrumMath", Value = "2nd Derivative" },
                        new EvaluatorParameterJson { Name = "SmoothingWindow", Value = 0 },
                        new EvaluatorParameterJson { Name = "ScatterCorrection", Value = "None" },
                        new EvaluatorParameterJson { Name = "ConcIRT", Value = "None" },
                    }
                }
            }
        };
        zone.Occupations.Add(occupation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task IrEvaluators_WithPeaks_SchemaValidationPasses()
    {
        var experiment = CreateExperimentWithOperation();

        var instrument = GetOrAddReactIrInstrument(experiment.Process.Stages[0]);
        var zone = GetOrAddZone(instrument);
        var occupation = new OccupationJson
        {
            TrackingId = Guid.NewGuid(),
            Evaluators = {
                new EvaluatorConfigurationJson {
                    Name = "SpectraEvaluator",
                    Trends = {
                        new PeakDefinitionJson {
                            Name = "Peak1",
                            ProfileGroup = ProfileGroupJson.Area,
                            ProfileType = "AreaToSinglePointBaseline",
                            PeakStart = 1234,
                            PeakStop = 1235,
                            BaselineStart = 1233
                        },
                        new PeakDefinitionJson {
                            Name = "Peak2",
                            ProfileGroup = ProfileGroupJson.Height,
                            ProfileType = "HeightToTwoPointBaseline",
                            PeakStart = 1373,
                            BaselineStart = 1370,
                            BaselineStop = 1375
                        }
                    },
                    Parameters = {
                        new EvaluatorParameterJson { Name = "WaterVaporCorrection", Value = "Applied" },
                        new EvaluatorParameterJson { Name = "SubtractSpectrum", Value = "None" },
                        new EvaluatorParameterJson { Name = "BaselineCorrection", Value = "None" },
                        new EvaluatorParameterJson { Name = "Normalize", Value = "2000 cm-1" },
                        new EvaluatorParameterJson { Name = "SpectrumMath", Value = "2nd Derivative" },
                        new EvaluatorParameterJson { Name = "SmoothingWindow", Value = 0 },
                        new EvaluatorParameterJson { Name = "ScatterCorrection", Value = "None" },
                        new EvaluatorParameterJson { Name = "ConcIRT", Value = "None" },
                    }
                }
            }
        };
        zone.Occupations.Add(occupation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task VisionEvaluators_WithStatistics_SchemaValidationPasses()
    {
        var experiment = CreateExperimentWithOperation();

        var instrument = GetOrAddReactIrInstrument(experiment.Process.Stages[0]);
        var zone = GetOrAddZone(instrument);
        var occupation = new OccupationJson
        {
            TrackingId = Guid.NewGuid(),
            Evaluators = {
                new EvaluatorConfigurationJson {
                    Name = "ImageToChords",
                    Trends = {
                        new StatisticDefinitionJson {
                            Name = "Count 1-100 micron",
                            StatisticType = "Total Counts",
                            XYDataName = "Primary",
                            RangeStart = 1,
                            RangeEnd = 100,
                            WeightingType = "None"
                        },
                        new StatisticDefinitionJson {
                            Name = "Median 1-100 micron",
                            StatisticType = "Median",
                            XYDataName = "ImageToChords",
                            RangeStart = 1,
                            RangeEnd = 100
                        },
                        new StatisticDefinitionJson {
                            Name = "Percent above 600 micron",
                            StatisticType = "PercentAbove",
                            XYDataName = "ImageToChords",
                            Parameters = {
                                new EvaluatorParameterJson { Name = "ChordLength", Value = 600 },
                            }
                        }
                    }
                },
                new EvaluatorConfigurationJson
                {
                    Name="ImageJ",
                    Parameters = {
                        new EvaluatorParameterJson { Name = "MacroPath", Value = "C:\\temp\\macro.ijm" }
                    }
                }
            }
        };
        zone.Occupations.Add(occupation);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Theory]
    [InlineData(EquipmentCategoryJson.IR)]
    [InlineData(EquipmentCategoryJson.Raman)]
    [InlineData(EquipmentCategoryJson.Imaging)]
    [InlineData(EquipmentCategoryJson.ParticleSizeAnalyzer)]
    public async Task Instrument_SpecifiedCategory_SchemaValidationPasses(EquipmentCategoryJson category)
    {
        var experiment = CreateExperimentWithOperation();

        var instrument = GetOrAddInstrument(experiment.Process.Stages[0], category);
        GetOrAddZone(instrument);

        var schema = await GetExperimentSchema();

        var jsonDoc = JsonSerializer.Serialize(experiment, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        var results = EvaluateJson(schema, testJson);

        // Assert
        ShouldBeNoErrors(results);
    }

    [Fact]
    public async Task CombineSchema_AllSchemaParts_CombineSuccessfully()
    {
        var schema = await GetExperimentSchema();

        var bundledSchema = Should.NotThrow(() => schema.Bundle());

        var jsonSchemaDocument = bundledSchema.ToJsonDocument();
        var jsonNode = JsonNode.Parse(JsonSerializer.Serialize(bundledSchema));
        jsonNode.ShouldNotBeNull();
        var jsonNodeClone = jsonNode.DeepClone();
        var originalDefs = jsonNode["$defs"].ShouldNotBeNull();
        var newDefs = new JsonObject();

        foreach (var definition in originalDefs.AsObject())
        {
            var newKey = definition.Value.ShouldNotBeNull()["$id"].ShouldNotBeNull().GetValue<string>().Substring(37).Replace('/', '_');
            newDefs[newKey] = definition.Value.DeepClone();
        }
        jsonNodeClone["$defs"] = newDefs;

        var jsonSchemaString = JsonSerializer.Serialize(jsonNodeClone);
        GC.KeepAlive(jsonSchemaString);
    }

    private ExperimentJson CreateExperimentWithOperation(OperationJson? operationJson = null)
    {
        var experiment = new ExperimentJson();
        experiment.Process.Stages.Add(new StageJson()
        {
            TrackingId = Guid.NewGuid(),
            OperationGroups = { new OperationGroupJson()
            {
                OperationSequences = { new OperationSequenceJson()
                {
                    Operations=
                    {
                        operationJson ?? new ObservationOperationJson()
                        {
                            ObservationTime = DateTime.UtcNow,
                            Notes = "A change happened",
                            StartTimeUtc = DateTime.UtcNow,
                            CompletionStatus = OperationJson.CompletionStatusEnum.Complete,
                            CompletionTimeUtc = DateTime.UtcNow,
                            TrackingId = Guid.NewGuid()
                        }
                    }
                }
                }
            }
            }
        });

        var occupation = AddAlrOccupation(experiment.Process.Stages[0]);

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

        return experiment;
    }

    private OccupationJson AddAlrOccupation(StageJson stage)
    {
        var occupation = new OccupationJson { TrackingId = Guid.NewGuid() };
        var zone = GetOrAddInstrumentZone(stage);
        zone.Occupations.Add(occupation);

        return occupation;
    }

    private AlrJson GetOrAddAlr(StageJson stage)
    {
        var alr = stage.EquipmentCollection.OfType<AlrJson>().FirstOrDefault();

        if (alr == null)
        {
            alr = new AlrJson
            {
                Model = "EasyMax 102i",
                SerialNumber = "C334342",
                AssetTag = "Test EasyMax",
                TrackingId = Guid.NewGuid(),
                SoftwareVersion = "1.0.1.5"
            };
            stage.EquipmentCollection.Add(alr);
        }

        return alr;
    }

    private InstrumentJson GetOrAddInstrument(StageJson stage, EquipmentCategoryJson category)
    {
        var instrument = stage.EquipmentCollection.OfType<InstrumentJson>().FirstOrDefault(x => x.Category == category);
        if (instrument == null)
        {
            instrument = new InstrumentJson
            {
                Model = $"{category}",
                SerialNumber = "C334342",
                AssetTag = $"Test {category}",
                Category = category,
                TrackingId = Guid.NewGuid(),
            };
            stage.EquipmentCollection.Add(instrument);
        }
        return instrument;
    }

    private InstrumentJson GetOrAddReactIrInstrument(StageJson stage)
    {
        var instrument = stage.EquipmentCollection.OfType<InstrumentJson>().FirstOrDefault(x => x.Category == EquipmentCategoryJson.IR);
        if (instrument == null)
        {
            instrument = new InstrumentJson
            {
                Model = "ReactIR 702L",
                SerialNumber = "C334342",
                Category = EquipmentCategoryJson.IR,
            };
            stage.EquipmentCollection.Add(instrument);
        }
        return instrument;
    }

    private InstrumentZoneJson GetOrAddZone(InstrumentJson instrument)
    {
        var zone = instrument.Zones.FirstOrDefault();
        if (zone == null)
        {
            zone = new InstrumentZoneJson();
            instrument.Zones.Add(zone);
        }
        return zone;
    }

    private AlrInstrumentZoneJson GetOrAddInstrumentZone(StageJson stage)
    {
        var alr = GetOrAddAlr(stage);

        var zone = alr.Zones.OfType<AlrInstrumentZoneJson>().FirstOrDefault();
        if (zone == null)
        {
            zone = new AlrInstrumentZoneJson
            {
                Receptacle = {ReceptacleName = ExperimentJsonSerializationTests.OneHundredMlTwoPieceReactorName,
                ReactorVesselNames = { ExperimentJsonSerializationTests.OneHundredMlTwoPieceReactorName } }
            };
            alr.Zones.Add(zone);
        }
        return zone;
    }

    private MaterialPortionJson CreateMaterial() => new MaterialPortionJson
    {
        TrackingId = Guid.NewGuid(),
        Name = "Ethanol",
        LotNumber = "EA33233"
    };


    private async Task<JsonSchema> GetExperimentSchema()
    {
        await SchemaTestHelpers.RegisterSchemas(typeof(SchemaMarkerObject).Assembly);

        return SchemaTestHelpers.GetSchema(new Uri("http://iC3ng.com/schemas/experiments/experiment"));
    }

    private static EvaluationResults EvaluateJson(JsonSchema schema, JsonNode? testJson) =>
        SchemaTestHelpers.EvaluateJson(schema, testJson);

    private IEnumerable<string> GetErrorMessages(EvaluationResults results) =>
        SchemaTestHelpers.GetErrorMessages(results);

    private void ShouldBeNoErrors(EvaluationResults results)
    {
        IEnumerable<string> errorMessages = GetErrorMessages(results);

        var combinedErrorMessage = string.Join('\n', errorMessages);

        results.IsValid.ShouldBeTrue(combinedErrorMessage);
    }
}
