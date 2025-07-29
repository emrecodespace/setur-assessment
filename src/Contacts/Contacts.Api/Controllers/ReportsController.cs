using Contacts.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Contacts.Api.Controllers;

/// <summary>
/// API endpoints for retrieving and managing report records.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ReportsController(IReportService reportService) : ControllerBase
{
    /// <summary>
    /// Retrieves a list of all reports.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Returns 200 OK with a list of reports.
    /// </returns>
    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken ct)
    {
        var result = await reportService.GetReport(ct);
        return Ok(result.Value);
    }
}

