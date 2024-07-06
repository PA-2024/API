using API_GesSIgn.Sockets;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace API_GesSIgn.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QcmController : ControllerBase
    {
        private readonly IHubContext<QcmHub> _hubContext;

        public QcmController(IHubContext<QcmHub> hubContext)
        {
            _hubContext = hubContext;
        }

        [HttpPost("start/{qcmId}")]
        public async Task<IActionResult> StartQCM(int qcmId)
        {
            await _hubContext.Clients.All.SendAsync("StartQCM", qcmId);
            return Ok(new { Message = "QCM started", QcmId = qcmId });
        }

        [HttpPost("pause")]
        public async Task<IActionResult> PauseQCM()
        {
            await _hubContext.Clients.All.SendAsync("PauseQCM");
            return Ok(new { Message = "QCM paused" });
        }

        [HttpPost("resume")]
        public async Task<IActionResult> ResumeQCM()
        {
            await _hubContext.Clients.All.SendAsync("ResumeQCM");
            return Ok(new { Message = "QCM resumed" });
        }

        [HttpPost("answer")]
        public async Task<IActionResult> SubmitAnswer([FromBody] AnswerRequest answerRequest)
        {
            await _hubContext.Clients.All.SendAsync("SendAnswer", answerRequest.Answer);
            return Ok(new { Message = "Answer submitted", answerRequest.Answer });
        }

        /*
        [HttpGet("ranking")]
        public async Task<IActionResult> GetRanking()
        {
            // You may need to retrieve the ranking from your hub or directly from the database
            //var ranking = await _hubContext.Clients.All.SendAsync("GetRanking");
            return Ok(new { Message = "Ranking retrieved", Ranking = ranking });
        }
        */
    }

    public class AnswerRequest
    {
        public string ConnectionId { get; set; }
        public string Answer { get; set; }
    }
}
