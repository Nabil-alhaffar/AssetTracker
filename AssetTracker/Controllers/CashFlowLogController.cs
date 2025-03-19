using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using AssetTracker.Services;
using AssetTracker.Services.Interfaces;
using AssetTracker.Models;
namespace AssetTracker.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CashFlowLogController: ControllerBase
	{
		private readonly ICashFlowLogService _cashFlowLogService;
		public CashFlowLogController(ICashFlowLogService cashFlowLogService)
		{
			_cashFlowLogService = cashFlowLogService;
		}

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetAllUserLogs(Guid userId)
		{
			try
			{
				var userLogs = await _cashFlowLogService.GetLogsByUserIdAsync(userId);
				return Ok(userLogs);
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

        [HttpGet("All")]
        public async Task<IActionResult> GetAllLogs()
        {
            try
            {
                var logs = await _cashFlowLogService.GetAllLogsAsync();
                return Ok(logs);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet("{transactionId}")]
        public async Task<IActionResult> GetLogById(Guid transactionId)
        {
            try
            {
                var log = await _cashFlowLogService.GetLogByIdAsync(transactionId);
                return Ok(log);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        [HttpPost("Create")]
        public async Task<IActionResult> CreateLog([FromBody] CashFlowLog log)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _cashFlowLogService.AddLogAsync(log);
            return Ok("Log created successfully");


        }
        [HttpPost("Delete")]
        public async Task<IActionResult> DeleteLog([FromBody] Guid logId)
        {
            try
            {
                await _cashFlowLogService.DeleteLogAsync(logId);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }


        }


    }
}

