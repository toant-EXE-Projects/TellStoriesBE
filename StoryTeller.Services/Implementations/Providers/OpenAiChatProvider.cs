//using Microsoft.Extensions.Configuration;
//using StoryTeller.Data.Entities.AI;
//using StoryTeller.Services.Interfaces.AI;
//using StoryTeller.Services.Models.AI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Net.Http.Headers;
//using System.Net.Http.Json;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace StoryTeller.Services.Implementations.Providers
//{
//    public class OpenAiChatProvider : IChatProvider
//    {
//        private readonly HttpClient _httpClient;
//        private readonly string _apiKey;

//        public OpenAiChatProvider(HttpClient httpClient, IConfiguration config)
//        {
//            _httpClient = httpClient;
//            _apiKey = config["OpenAI:ApiKey"] ?? throw new ArgumentNullException("OpenAI:ApiKey");
//        }

//        private IEnumerable<object> PrepareMessages(IEnumerable<ChatMessage> history, string prompt, string? systemMessage)
//        {
//            var baseMessages = new List<object>();

//            if (!string.IsNullOrWhiteSpace(systemMessage))
//            {
//                baseMessages.Add(new { role = "system", content = systemMessage });
//            }

//            baseMessages.AddRange(history.Select(m => new { role = m.Role, content = m.Content }));
//            baseMessages.Add(new { role = "user", content = prompt });

//            return baseMessages;
//        }

//        public async Task<string> GetResponseAsync(string prompt, IEnumerable<ChatMessage> history, ChatGenerationOptions? options = null, CancellationToken ct = default)
//        {
//            var messages = PrepareMessages(history, prompt, options?.SystemInstruction);

//            var requestBody = new
//            {
//                model = "gpt-4",
//                temperature = options?.Temperature ?? 1.0,
//                top_p = options?.TopP ?? 1.0,
//                presence_penalty = options?.PresencePenalty ?? 0.0,
//                frequency_penalty = options?.FrequencyPenalty ?? 0.0,
//                messages
//            };

//            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
//            {
//                Content = JsonContent.Create(requestBody)
//            };
//            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

//            var res = await _httpClient.SendAsync(req, ct);
//            res.EnsureSuccessStatusCode();

//            using var doc = JsonDocument.Parse(await res.Content.ReadAsStringAsync(ct));
//            return doc.RootElement.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
//        }

//        public async IAsyncEnumerable<string> StreamResponseAsync(string prompt, IEnumerable<ChatMessage> history, ChatGenerationOptions? options = null, [EnumeratorCancellation] CancellationToken ct = default)
//        {
//            var messages = PrepareMessages(history, prompt, options?.SystemInstruction);

//            var requestBody = new
//            {
//                model = "gpt-4",
//                temperature = options?.Temperature ?? 1.0,
//                top_p = options?.TopP ?? 1.0,
//                presence_penalty = options?.PresencePenalty ?? 0.0,
//                frequency_penalty = options?.FrequencyPenalty ?? 0.0,
//                messages,
//                stream = true
//            };

//            var req = new HttpRequestMessage(HttpMethod.Post, "https://api.openai.com/v1/chat/completions")
//            {
//                Content = JsonContent.Create(requestBody)
//            };
//            req.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

//            var res = await _httpClient.SendAsync(req, HttpCompletionOption.ResponseHeadersRead, ct);
//            using var stream = await res.Content.ReadAsStreamAsync(ct);
//            using var reader = new StreamReader(stream);

//            while (!reader.EndOfStream)
//            {
//                var line = await reader.ReadLineAsync();
//                if (string.IsNullOrWhiteSpace(line) || !line.StartsWith("data:")) continue;

//                var payload = line[5..].Trim();
//                if (payload == "[DONE]") yield break;

//                using var json = JsonDocument.Parse(payload);
//                var content = json.RootElement.GetProperty("choices")[0].GetProperty("delta").GetProperty("content").GetString();
//                if (!string.IsNullOrWhiteSpace(content)) yield return content;
//            }
//        }

//        public Task<string> GetStoryResponseAsync(string prompt, IEnumerable<ChatMessage> history, ChatGenerationOptions? options = null, CancellationToken ct = default)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
