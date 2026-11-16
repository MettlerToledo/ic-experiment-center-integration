using System.Reflection;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using Json.Schema;

namespace MettlerToledo.Labs.Experiments.UnitTests;

internal static class SchemaTestHelpers
{
    private static readonly Dictionary<Assembly, Lazy<Task>> Registrations = new();
    private static readonly Lock RegistrationsLock = new();

    public static Task RegisterSchemas(Assembly assembly)
    {
        Lazy<Task> lazy;
        lock (RegistrationsLock)
        {
            if (!Registrations.TryGetValue(assembly, out lazy!))
            {
                lazy = new Lazy<Task>(() => RegisterSchemasCore(assembly));
                Registrations[assembly] = lazy;
            }
        }
        return lazy.Value;
    }

    private static async Task RegisterSchemasCore(Assembly assembly)
    {
        var regex = new Regex("[a-zA-Z]+Schema[.]json");
        var schemaResources = assembly.GetManifestResourceNames().Where(name => regex.IsMatch(name));

        foreach (var schemaResource in schemaResources)
        {
            await using var resourceStream = assembly.GetManifestResourceStream(schemaResource)
                                             ?? throw new Exception($"Could not find resource: {schemaResource}");

            var schema = await JsonSchema.FromStream(resourceStream);
            SchemaRegistry.Global.Register(schema);
        }
    }

    public static JsonSchema GetSchema(Uri id) => (JsonSchema)SchemaRegistry.Global.Get(id);

    public static EvaluationResults EvaluateJson(JsonSchema schema, JsonNode? testJson)
    {
        var evaluationOptions = new EvaluationOptions
        {
            OutputFormat = OutputFormat.List,
            EvaluateAs = SpecVersion.Draft202012
        };
        return schema.Evaluate(testJson, evaluationOptions);
    }

    public static IEnumerable<string> GetErrorMessages(EvaluationResults results)
    {
        var errorEvaluationResults = results.Details.Where(x => (x.Errors?.Any() ?? false) && !HasValidParent(x))
            .GroupBy(x => x.InstanceLocation)
            .OrderByDescending(x => x.Key.Count).ToList();
        return errorEvaluationResults.Select(x => $"{x.Key}\n\t{string.Join("\n\t", x.SelectMany(e => e.Errors!).Distinct())}");
    }

    private static bool HasValidParent(EvaluationResults evaluationResults) =>
        evaluationResults.Parent is not null &&
        (evaluationResults.Parent.IsValid || HasValidParent(evaluationResults.Parent));
}
