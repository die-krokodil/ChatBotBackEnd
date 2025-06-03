using System;
using System.Threading.Tasks;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LLama;
using LLama.Abstractions;
using LLama.Common;
using LLama.Native;
using LlamaAI.Core.Models;

namespace LlamaAI.Core.Services
{
    public class LlamaService : ILlamaService
    {
        private readonly ILogger<LlamaService> _logger;
        private readonly LlamaConfig _config;
        private ChatSession? _chatSession;
        private ILLamaExecutor? _executor;

        public LlamaService(ILogger<LlamaService> logger, IOptions<LlamaConfig> config)
        {
            _logger = logger;
            _config = config.Value;
        }

        public async Task LoadModelAsync()
        {
            try
            {
                _logger.LogInformation("Starting to load model from path: {ModelPath}", _config.ModelPath);
                
                if (!File.Exists(_config.ModelPath))
                {
                    throw new FileNotFoundException($"Model file not found at path: {_config.ModelPath}");
                }

                var parameters = new ModelParams(_config.ModelPath)
                {
                    ContextSize = (uint)_config.ContextSize,
                    BatchSize = (uint)_config.BatchSize,
                };

                _logger.LogInformation("Creating model with parameters: ContextSize={ContextSize}, BatchSize={BatchSize}", 
                    parameters.ContextSize, parameters.BatchSize);

                // Load model weights
                var model = await Task.Run(() => LLamaWeights.LoadFromFile(parameters));
                _logger.LogInformation("Model weights loaded successfully");

                // Create context
                var context = await Task.Run(() => model.CreateContext(parameters));
                _logger.LogInformation("Model context created successfully");

                // Create executor and chat session
                _executor = new InteractiveExecutor(context);
                _chatSession = new ChatSession(_executor);
                
                _logger.LogInformation("AI model initialization completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading AI model: {ErrorMessage}", ex.Message);
                throw;
            }
        }

        public async Task<string> GenerateResponseAsync(string prompt)
        {
            if (_chatSession == null)
            {
                throw new InvalidOperationException("Model not loaded. Call LoadModelAsync first.");
            }

            try
            {
                var inferenceParams = new InferenceParams()
                {
                    MaxTokens = _config.MaxTokens
                };

                var sb = new StringBuilder();
                await foreach (var text in _chatSession.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams))
                {
                    sb.Append(text);
                }
                return sb.ToString().Trim();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating response");
                throw;
            }
        }
    }
}
