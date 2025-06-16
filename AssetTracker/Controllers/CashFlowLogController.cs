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


        /// <summary>
        /// Retrieves all cash flow logs for a specific user.
        /// </summary>
        /// <param name="userId">User's unique identifier</param>
        /// <returns>List of logs or error</returns>
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


        /// <summary>
        /// Retrieves all cash flow logs in the system.
        /// </summary>
        /// <returns>All logs or error</returns>
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

        /// <summary>
        /// Retrieves a specific cash flow log by transaction ID.
        /// </summary>
        /// <param name="transactionId">Transaction log ID</param>
        /// <returns>Log entry or error</returns>
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

        /// <summary>
        /// Creates a new cash flow log.
        /// </summary>
        /// <param name="log">CashFlowLog object from request body</param>
        /// <returns>Success or error</returns>
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

        /// <summary>
        /// Deletes an existing cash flow log by ID.
        /// </summary>
        /// <param name="logId">Log ID to delete</param>
        /// <returns>Success or error</returns>
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

