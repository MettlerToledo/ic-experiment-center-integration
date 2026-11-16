// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System;

namespace MettlerToledo.Labs.Experiments.Clients.Definitions.Parameters;

/// <summary>
/// Defines a string parameter in a dynamic operation.
/// </summary>
public class StringParameterDefinitionJson : DynamicParameterDefinitionJson
{
    /// <summary>
    /// The maximum length for the string value.
    /// </summary>
    public int? MaximumLength { get; set; }

    /// <summary>
    /// True if the value can be multiline.
    /// </summary>
    public bool IsMultiline { get; set; }

    /// <summary>
    /// The allowed values for the string parameter.
    /// If the array is empty, any string value is allowed. If the array contains values, only those values are allowed for the parameter.
    /// </summary>
    public string[] AllowedValues { get; set; } = Array.Empty<string>();
}
