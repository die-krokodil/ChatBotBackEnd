using System.Threading.Tasks;

namespace LlamaAI.Core.Services
{
    public interface ILlamaService
    {
        Task<string> GenerateResponseAsync(string prompt);
        Task LoadModelAsync();
    }
}
