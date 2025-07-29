using ContactReports.Api.Helpers;
using ContactReports.Application.Dtos;
using ContactReports.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ContactReports.Api.Controllers;

/// <summary>
/// API endpoints for creating report requests, listing all reports, and retrieving report details.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    /// <summary>
    /// Lists all reports in the system.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetReports(CancellationToken ct)
    {
        var result = await reportService.GetReports(ct);
        return this.ToActionResult(result);
    }
    
    /// <summary>
    /// Gets detailed information of a specific report.
    /// </summary>
    /// <param name="id">Report's unique ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns detailed statistics for the report.</returns>
    [HttpGet("{id:guid}/details")]
    public async Task<IActionResult> GetDetails(Guid id, CancellationToken ct)
    {
        var result = await reportService.GetReportDetails(id, ct);
        return this.ToActionResult(result);
    }

    /// <summary>
    /// Creates a new report request. The report will be processed asynchronously.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Returns the status of the report request.</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateReport(CancellationToken ct)
    {
        var result = await reportService.CreateReport(ct);
        return this.ToActionResult(result);
    }
    
    /// <summary>
    /// /// This endpoint is intended for background service use only.
    /// </summary>
    /// <param name="id">Report ID.</param>
    /// <param name="details">Report details (location, counts).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>200 OK if added, 404 if report not found.</returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPost("{id:guid}/details")]
    public async Task<IActionResult> CreateReport(Guid id, [FromBody] IEnumerable<ReportDetailsRequest> details, CancellationToken ct)
    {
        var result = await reportService.AddReportDetails(id, details, ct);
        return this.ToActionResult(result);
    }
}
