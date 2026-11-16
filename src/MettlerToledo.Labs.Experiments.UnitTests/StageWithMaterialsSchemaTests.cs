// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json;
using System.Text.Json.Nodes;
using Json.Schema;
using MettlerToledo.Labs.Experiments.Clients.Experiments;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Materials;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Operations;
using MettlerToledo.Labs.Experiments.Clients.Experiments.Units;
using MettlerToledo.Labs.Experiments.Schemas;
using Shouldly;
using Xunit;

namespace MettlerToledo.Labs.Experiments.UnitTests;

public class StageWithMaterialsSchemaTests
{
    [Fact]
    public async Task JsonFromStageWithMaterials_ValidData_SchemaValidationPasses()
    {
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

        var stageWithMaterials = new StageWithMaterialsJson
        {
            Stage = new StageJson
            {
                TrackingId = Guid.NewGuid(),
                StageType = StageTypeJson.ReactionStage,
                OperationGroups =
                {
                    new OperationGroupJson
                    {
                        OperationSequences =
                        {
                            new OperationSequenceJson
                            {
                                Operations =
                                {
                                    new ObservationOperationJson
                                    {
                                        TrackingId = Guid.NewGuid(),
                                        ObservationTime = DateTime.UtcNow,
                                        Notes = "A change happened",
                                        StartTimeUtc = DateTime.UtcNow,
                                        CompletionStatus = OperationJson.CompletionStatusEnum.Complete,
                                        CompletionTimeUtc = DateTime.UtcNow
                                    }
                                }
                            }
                        }
                    }
                }
            }
        };
        stageWithMaterials.Materials.Add(ethanol);

        var schema = await GetStageWithMaterialsSchema();

        var jsonDoc = JsonSerializer.Serialize(stageWithMaterials, ExperimentJsonSerializer.SerializerOptions);
        var testJson = JsonNode.Parse(jsonDoc);

        var results = SchemaTestHelpers.EvaluateJson(schema, testJson);

        var errorMessages = SchemaTestHelpers.GetErrorMessages(results);
        results.IsValid.ShouldBeTrue(string.Join('\n', errorMessages));
    }

    private static async Task<JsonSchema> GetStageWithMaterialsSchema()
    {
        await SchemaTestHelpers.RegisterSchemas(typeof(SchemaMarkerObject).Assembly);

        return SchemaTestHelpers.GetSchema(new Uri("http://iC3ng.com/schemas/experiments/stageWithMaterials"));
    }
}
