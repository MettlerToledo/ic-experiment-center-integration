// Copyright © 2025-2026 METTLER TOLEDO. All rights reserved.

namespace MettlerToledo.Labs.Experiments.Clients.Experiments.PublicApi;

/// <summary>
/// The report file formats that can be downloaded for an experiment through the public API.
/// </summary>
public enum ReportFileType
{
    /// <summary>
    /// PDF document.
    /// </summary>
    Pdf,

    /// <summary>
    /// Word (.docx) document.
    /// </summary>
    Docx,
}
