using AssetTracker.Models;
using AssetTracker.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[Route("api/alerts")]
[ApiController]
public class AlertController : ControllerBase
{
    private readonly AlertService _alertService;

    public AlertController(AlertService alertService)
    {
        _alertService = alertService;
    }

    [HttpPost("add")]
    public IActionResult AddAlert([FromBody] StockAlert alert)
    {
        if (alert == null || string.IsNullOrEmpty(alert.Symbol))
        {
            return BadRequest("Invalid alert data.");
        }

        _alertService.AddAlert(alert);
        return Ok("Alert added successfully.");
    }

    [HttpDelete("remove")]
    public IActionResult RemoveAlert([FromQuery] string symbol, [FromQuery] AlertType condition)
    {
        if (string.IsNullOrEmpty(symbol))
        {
            return BadRequest("Symbol is required.");
        }

        _alertService.RemoveAlert(symbol, new AlertCondition { Type = condition });
        return Ok($"Alert for {symbol} removed.");
    }

    [HttpGet("list")]
    public IActionResult GetAlerts()
    {
        var alerts = _alertService.GetAlerts();
        return Ok(alerts);
    }

    [HttpPost("check")]
    public async Task<IActionResult> CheckAlerts()
    {
        await _alertService.CheckAlertsAsync();
        return Ok("Alerts checked.");
    }
}
