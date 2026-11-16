// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using MettlerToledo.Labs.Experiments.Clients.Definitions;
using MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;
using MettlerToledo.Labs.Experiments.Schemas;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests;

public class S88DefinitionSchemaTests
{
    [Fact]
    public async Task JsonFromS88Defitions_ValidProcesses_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Processes = new List<ProcessDefinitionJson>
            {
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.None.ToString(),
                    DisplayName = "None",
                    Description = "A description for none"
                },
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.Synthesis.ToString(),
                    DisplayName = "Synthesis",
                    Description = "A description for synthesis"
                },
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.Rework.ToString(),
                    DisplayName = "Rework",
                    Description = "A description for rework"
                },
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.Reprocess.ToString(),
                    DisplayName = "Reprocess",
                    Description = "A description for reprocess"
                },
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.Purification.ToString(),
                    DisplayName = "Purification",
                    Description = "A description for purification"
                },
                new ProcessDefinitionJson
                {
                    Id = ProcessTypeJson.ParticleTuning.ToString(),
                    DisplayName = "Particle Tuning",
                    Description = "A description for particle tuning"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidProcessType_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Processes = new List<ProcessDefinitionJson>
            {
                new ProcessDefinitionJson
                {
                    Id = "InvalidProcessType",
                    DisplayName = "Invalid example",
                    Description = "A description for invalid process type"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidDisplayName_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Processes = new List<ProcessDefinitionJson>
            {
                new ProcessDefinitionJson
                {
                    Id = "InvalidProcessType",
                    DisplayName = string.Empty,
                    Description = "A description for invalid process type"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task JsonFromS88Defitions_ProcessWithoutID_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[
            {
                ""displayName"":""Particle Tuning"",
                ""description"":""A description for particle tuning""}
            ],
            ""stages"":[],
            ""operations"":[]
        }";

        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ProcessWithoutDisplayName_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[
            {
                ""id"":""ParticleTuning"",
                ""description"":""A description for particle tuning""}
            ],
            ""stages"":[],
            ""operations"":[]
        }";

        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidStages_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Stages = new List<StageDefinitionJson>
            {
                new StageDefinitionJson
                {
                    Id = StageTypeJson.Unknown.ToString(),
                    DisplayName = "Unknown",
                    Description = "A description for unknown"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.CrystallizationStage.ToString(),
                    DisplayName = "Crystallization",
                    Description = "A description for crystallization"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.DecompositionStage.ToString(),
                    DisplayName = "Decomposition",
                    Description = "A description for decomposition"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.DistillationOfProductStage.ToString(),
                    DisplayName = "Distillation of Product",
                    Description = "A description for distillation of product"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.DryingStage.ToString(),
                    DisplayName = "Drying",
                    Description = "A description for drying"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.DryingOfSolutionStage.ToString(),
                    DisplayName = "Drying of Solution",
                    Description = "A description for drying of solution"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.EquipmentConditioningStage.ToString(),
                    DisplayName = "Equipment Conditioning",
                    Description = "A description for equipment conditioning"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.EquipmentPreparationStage.ToString(),
                    DisplayName = "Equipment Preparation",
                    Description = "A description for equipment preparation"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.ExtractionStage.ToString(),
                    DisplayName = "Extraction",
                    Description = "A description for extraction"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.FiltrationStage.ToString(),
                    DisplayName = "Filtration",
                    Description = "A description for filtration"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.HomogenizationStage.ToString(),
                    DisplayName = "Homogenization",
                    Description = "A description for homogenization"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.IsolationOfSolidStage.ToString(),
                    DisplayName = "Isolation of Solid",
                    Description = "A description for isolation of solid"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.MixturePreparationStage.ToString(),
                    DisplayName = "Mixture Preparation",
                    Description = "A description for mixture preparation"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.ParticleSizeReductionStage.ToString(),
                    DisplayName = "Particle Size Reduction",
                    Description = "A description for particle size reduction"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.ReactionStage.ToString(),
                    DisplayName = "Reaction",
                    Description = "A description for reaction"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.SolventRemovalStage.ToString(),
                    DisplayName = "Solvent Removal",
                    Description = "A description for solvent removal"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.SolventSwitchStage.ToString(),
                    DisplayName = "Solvent Switch",
                    Description = "A description for solvent switch"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.TransferStage.ToString(),
                    DisplayName = "Transfer",
                    Description = "A description for transfer"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.WashingStage.ToString(),
                    DisplayName = "Washing",
                    Description = "A description for washing"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.WasteTreatmentStage.ToString(),
                    DisplayName = "Waste Treatment",
                    Description = "A description for waste treatment"
                },
                new StageDefinitionJson
                {
                    Id = StageTypeJson.ChromatographicSeparationStage.ToString(),
                    DisplayName = "Chromatographic Separation",
                    Description = "A description for chromatographic separation"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidStageType_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Stages = new List<StageDefinitionJson>
            {
                new StageDefinitionJson
                {
                    Id = "InvalidStageType",
                    DisplayName = "Invalid example",
                    Description = "A description for invalid stage type"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task JsonFromS88Defitions_StageWithoutID_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[],
            ""stages"":[
            {
                ""displayName"":""Washing"",
                ""description"":""A description for washing""}
            ],
            ""operations"":[]
        }";


        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidOperations_SchemaValidationPasses()
    {
        string[] operationIds =
        {
            "ControlPH",
            "Charge",
            "TemperatureControl",
            "HfCalDetermination",
            "OperatorMessage",
            "Pause",
            "QuickCalDetermination",
            "RecordVirtualVolume",
            "ResetIntegralQrRtc",
            "RTCalDetermination",
            "Settle",
            "Agitate",
            "TakeSample",
            "TakePicture",
            "Observation",
            "Wait",
            "Centrifuge",
            "Chromatography",
            "Concentrate",
            "Discharge",
            "Dry",
            "Filter",
            "Homogenize",
            "Hydrocyclone",
            "Inert",
            "Mill",
            "PackAndLabel",
            "PressureAdjust",
            "Recirculate",
            "Rinse",
            "Separate",
            "Sieve",
            "Transfer",
            "Wash",
            "WetMill"
        };

        var s88Definitions = new S88DefinitionsJson
        {
            Operations = operationIds.Select(id => new OperationDefinitionJson
            {
                Id = id,
                DisplayName = id,
                Description = $"A description for {id}"
            }).ToList()
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidOperationType_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "InvalidOperationType",
                    DisplayName = "Invalid example",
                    Description = "A description for invalid operation type"
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task JsonFromS88Defitions_OperationWithoutID_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[],
            ""stages"":[],
            ""operations"":[
            {
                ""displayName"":""Concentrate"",
                ""description"":""Concentrate a material""}
            ]
        }";


        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_OperationWithoutDisplayName_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[],
            ""stages"":[],
            ""operations"":[
            {
                ""id"":""Concentrate"",
                ""description"":""Concentrate a material""}
            ]
        }";


        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidBooleanParameter_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Concentrate",
                    DisplayName = "Concentrate",
                    Description = "Concentrate a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new BooleanParameterDefinitionJson
                        {
                            Id = "WithVacuum",
                            DisplayName = "With Vacuum"
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ParameterWithoutID_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[],
            ""stages"":[],
            ""operations"":[
            {
                ""parameters"":[
                {
                    ""$type"":""BooleanParameterDefinition"",
                    ""displayName"":""With Vacuum""}
                ],
                ""id"":""Concentrate"",
                ""displayName"":""Concentrate"",
                ""description"":""Concentrate a material""}
            ]
        }";


        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ParameterWithoutDisplayName_SchemaValidationFails()
    {
        var jsonDoc = @"
        {
            ""processes"":[],
            ""stages"":[],
            ""operations"":[
            {
                ""parameters"":[
                {
                    ""$type"":""BooleanParameterDefinition"",
                    ""id"":""WithVacuum""
                }
                ],
                ""id"":""Concentrate"",
                ""displayName"":""Concentrate"",
                ""description"":""Concentrate a material""}
            ]
        }";

        JsonSchema schema = await GetS88DefinitionsSchema();

        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeFalse(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidMaterialParameter_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Dry",
                    DisplayName = "Dry",
                    Description = "Dry a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new MaterialParameterDefinitionJson
                        {
                            Id = "WetContent",
                            DisplayName = "Wet Content",
                            MaterialPortionUsage = MaterialPortionUsageJson.Input
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidStringParameter_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Hydrocyclone",
                    DisplayName = "Hydrocyclone",
                    Description = "Hydrocyclone",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new StringParameterDefinitionJson
                        {
                            Id = "CycloneSetup",
                            DisplayName = "Cyclone Setup",
                            MaximumLength = 500
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidStringParameterWithAllowedValues_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Inert",
                    DisplayName = "Inert Operation",
                    Description = "Inert Operation",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new StringParameterDefinitionJson
                        {
                            Id = "InertGas",
                            DisplayName = "Inert Gas",
                            AllowedValues = ["Nitrogen", "Argon", "Other"]
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidUnitValueParameter_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Dry",
                    DisplayName = "Dry",
                    Description = "Dry a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new UnitValueParameterDefinitionJson
                        {
                            Id = "Temperature",
                            DisplayName = "Temperature",
                            Unit = "°C"
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidUnitValueParameterNoUnit_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Hydrocyclone",
                    DisplayName = "Hydrocyclone",
                    Description = "Hydrocyclone a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new UnitValueParameterDefinitionJson
                        {
                            Id = "TheoreticalTurnover",
                            DisplayName = "Theoretical Turnover",
                            Unit = null
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_ValidUnitValueParameterWithLimits_SchemaValidationPasses()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "Hydrocyclone",
                    DisplayName = "Hydrocyclone",
                    Description = "Hydrocyclone a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new UnitValueParameterDefinitionJson
                        {
                            Id = "Speed",
                            DisplayName = "Speed",
                            Unit = "min-1",
                            MinimumValue = new UnitValueJson{ Value = 0, Unit = "min-1", SignificantDigits = 1 },
                            MaximumValue = new UnitValueJson{ Value = 1_000_000, Unit = "min-1", SignificantDigits = 7 }
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        IEnumerable<string> errorMessages = SchemaTestHelpers.GetErrorMessages(results);

        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidParameterId_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "ConcentrateOperation",
                    DisplayName = "Concentrate",
                    Description = "Concentrate a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new BooleanParameterDefinitionJson
                        {
                            Id = string.Empty,
                            DisplayName = "With Vacuum"
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    [Fact]
    public async Task JsonFromS88Defitions_InvalidParameterDisplayName_SchemaValidationFails()
    {
        var s88Definitions = new S88DefinitionsJson
        {
            Operations = new List<OperationDefinitionJson>
            {
                new OperationDefinitionJson
                {
                    Id = "ConcentrateOperation",
                    DisplayName = "Concentrate",
                    Description = "Concentrate a material",
                    Parameters = new List<DynamicParameterDefinitionJson>
                    {
                        new BooleanParameterDefinitionJson
                        {
                            Id = "WithVacuum",
                            DisplayName = string.Empty
                        }
                    }
                }
            }
        };

        JsonSchema schema = await GetS88DefinitionsSchema();

        var jsonDoc = JsonSerializer.Serialize(s88Definitions, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        // Act
        EvaluationResults results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        // Assert
        results.IsValid.ShouldBeFalse();
    }

    private async Task<JsonSchema> GetS88DefinitionsSchema()
    {
        await SchemaTestHelpers.RegisterSchemas(typeof(SchemaMarkerObject).Assembly);

        return SchemaTestHelpers.GetSchema(new Uri("http://iC3ng.com/schemas/experiments/s88Definitions"));
    }
}
