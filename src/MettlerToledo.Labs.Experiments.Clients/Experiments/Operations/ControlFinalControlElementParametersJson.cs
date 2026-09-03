// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

using System.Text.Json.Serialization;

namespace AutoChem.Lab.S88Definitions.Operations;

/// <summary>
/// The base class for final parameters for <see cref="ControlFinalControlElementOperationJson"/>.
/// </summary>
[JsonDerivedType(typeof(ControlFinalControlElementOnParametersJson), "ControlFinalControlElementOnParameters")]
[JsonDerivedType(typeof(ControlFinalControlElementOnDurationParametersJson), "ControlFinalControlElementOnDurationParameters")]
[JsonDerivedType(typeof(ControlFinalControlElementRampByDurationParametersJson), "ControlFinalControlElementRampByDurationParameters")]
[JsonDerivedType(typeof(ControlFinalControlElementByExpressionParametersJson), "ControlFinalControlElementByExpressionParameters")]
[JsonDerivedType(typeof(ControlFinalControlElementOffParametersJson), "ControlFinalControlElementOffParameters")]
public class ControlFinalControlElementParametersJson
{
}
