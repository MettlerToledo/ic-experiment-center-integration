// Copyright © 2025-2025 METTLER TOLEDO. All rights reserved.

using System.Collections.Generic;
using AutoChem.Lab.S88Definitions.Samples;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// An operation for taking a sample
/// </summary>
public class SampleOperationJson : OperationWithParametersJson<SampleParametersJson>
{
    /// <summary>
    /// The samples collected during the operation.
    /// </summary>
    public List<SampleJson> CollectedSamples { get; set; } = new List<SampleJson>();
}
