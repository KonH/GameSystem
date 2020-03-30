using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Prototype.LongPollingServer.Controllers {
	[ApiController]
	[Route("[controller]")]
	public class ChatController : ControllerBase {
		readonly LongPollingChat _chat;

		public ChatController(LongPollingChat chat) {
			_chat = chat;
		}

		[HttpGet]
		public async Task<IActionResult> Subscribe() {
			var (isAny, message) = await _chat.WaitForMessage();
			if ( !isAny ) {
				return NotFound();
			}
			return Content(message);
		}

		[HttpPost]
		public IActionResult Notify(string message) {
			_chat.Write(message);
			return Ok();
		}
	}
}