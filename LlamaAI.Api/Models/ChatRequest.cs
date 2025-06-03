using System.ComponentModel.DataAnnotations;

namespace LlamaAI.Api.Models
{
    public class ChatRequest
    {
        [Required]
        [MinLength(1)]
        [MaxLength(2048)]
        public string Prompt { get; set; } = string.Empty;
    }
}
