using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LlamaAI.Core.Services;
using LlamaAI.Api.Models;

namespace LlamaAI.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly ILlamaService _llamaService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(ILlamaService llamaService, ILogger<ChatController> logger)
        {
            _llamaService = llamaService;
            _logger = logger;
        }

        /// <summary>
        /// Generates a response based on the provided prompt
        /// </summary>
        /// <param name="request">Chat request containing the prompt</param>
        /// <returns>Generated response</returns>
        [HttpPost("generate")]
        [ProducesResponseType(typeof(ChatResponse), 200)]
        [ProducesResponseType(typeof(ChatResponse), 400)]
        [ProducesResponseType(typeof(ChatResponse), 500)]
        public async Task<IActionResult> GenerateResponse([FromBody] ChatRequest request)
        {
            try
            {
                var response = await _llamaService.GenerateResponseAsync(request.Prompt);
                return Ok(new ChatResponse
                {
                    Response = response,
                    Success = true
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Model not loaded error");
                return BadRequest(new ChatResponse
                {
                    Success = false,
                    Error = "AI model is not ready. Please try again later."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating response");
                return StatusCode(500, new ChatResponse
                {
                    Success = false,
                    Error = "An error occurred while processing your request."
                });
            }
        }
    }
}
