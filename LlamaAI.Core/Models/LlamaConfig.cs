using System;

namespace LlamaAI.Core.Models
{
    public class LlamaConfig
    {
        public string ModelPath { get; set; } = string.Empty;
        public int ContextSize { get; set; } = 512;
        public int MaxTokens { get; set; } = 256;
        public float Temperature { get; set; } = 0.8f;
        public int BatchSize { get; set; } = 8;
    }
}
