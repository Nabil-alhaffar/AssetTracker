//using AssetTracker.Models;
//using AssetTracker.Services;
//using Microsoft.AspNetCore.Mvc;
//using System.Collections.Generic;
//using System.Threading.Tasks;

//[Route("api/alerts")]
//[ApiController]
//public class AlertController : ControllerBase
//{
//    private readonly AlertService _alertService;

//    public AlertController(AlertService alertService)
//    {
//        _alertService = alertService;
//    }

//    [HttpPost("add")]
//    public IActionResult AddAlert([FromBody] StockAlert alert)
//    {
//        if (alert == null || string.IsNullOrEmpty(alert.Symbol))
//        {
//            return BadRequest("Invalid alert data.");
//        }

//        _alertService.AddAlert(alert);
//        return Ok("Alert added successfully.");
//    }

//    [HttpDelete("{id}")]
//    public async Task<IActionResult> DeleteAlert(int id)
//    {
//        var deleted = await _alertService.DeleteAlertAsync(id);
//        if (!deleted)
//            return NotFound(new { message = "Alert not found." });

//        return Ok(new { message = "Alert deleted successfully." });
//    }

//    [HttpGet("all")]
//    public async Task<IActionResult> GetAllAlerts()
//    {
//        try
//        {
//            var alerts = await _alertService.GetAllAlertsAsync();
//            if (!alerts.Any())
//                return NotFound(new { message = "No alerts found." });

//            return Ok(new { count = alerts.Count(), alerts });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"❌ Error fetching alerts: {ex.Message}");
//            return StatusCode(500, new { error = "Failed to retrieve alerts." });
//        }
//    }


//    [HttpPost("check")]
//    public async Task<IActionResult> CheckAlerts()
//    {
//        try
//        {
//            var triggeredAlerts = await _alertService.CheckAlertsAsync();

//            if (triggeredAlerts == null || !triggeredAlerts.Any())
//                return Ok(new { message = "No alerts triggered." });

//            return Ok(new
//            {
//                message = $"{triggeredAlerts.Count()} alerts triggered.",
//                alerts = triggeredAlerts
//            });
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"❌ Error checking alerts: {ex.Message}");
//            return StatusCode(500, new { error = "An error occurred while checking alerts." });
//        }
//    }
//}
