// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.Equipments;

/// <summary>
/// Information about an Alr
/// </summary>
[JsonObjectCreationHandling(JsonObjectCreationHandling.Populate)]
public class AlrJson : InstrumentJson
{
}
