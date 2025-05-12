using System.IO.Compression;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Diagnostics;

namespace ChiLearn.Services
{
    //    KeyId: JRAWPAHaEh7cZdGE
    //  KeySecret: kBbaTiK4pjXOrPIy
    public class SpeechFlowService : IDisposable
    {
        private readonly HttpClient _client;
        private bool _disposed;

        public string ApiKeyId { get; }
        public string ApiKeySecret { get; }

        public SpeechFlowService(string apiKeyId, string apiKeySecret)
        {
            ApiKeyId = apiKeyId ?? throw new ArgumentNullException(nameof(apiKeyId));
            ApiKeySecret = apiKeySecret ?? throw new ArgumentNullException(nameof(apiKeySecret));

            _client = new HttpClient
            {
                Timeout = TimeSpan.FromSeconds(60)
            };
            _client.DefaultRequestHeaders.Add("keyId", ApiKeyId);
            _client.DefaultRequestHeaders.Add("keySecret", ApiKeySecret);
        }

        public async Task<string> CreateTranscriptionTaskAsync(
            string filePath,
            string language = "zh",
            CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty");

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Local file not found", filePath);

            try
            {
                var content = new MultipartFormDataContent();
                var fileBytes = await File.ReadAllBytesAsync(filePath, ct);
                var fileContent = new ByteArrayContent(fileBytes);
                content.Add(fileContent, "file", Path.GetFileName(filePath));
                content.Add(new StringContent(language), "lang");

                var response = await _client.PostAsync(
                    "https://api.speechflow.io/asr/file/v1/create",
                    content,
                    ct);

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<SpeechFlowResponse>(responseJson);

                if (result?.Code == 10000 && !string.IsNullOrEmpty(result.TaskId))
                    return result.TaskId;

                throw new Exception($"API error {result?.Code}: {result?.Message}");
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create transcription task", ex);
            }
        }

        public async Task<string> GetTranscriptionResultAsync(
            string taskId,
            int resultType = 1,
            CancellationToken ct = default,
            int maxAttempts = 10)
        {
            if (string.IsNullOrWhiteSpace(taskId))
                throw new ArgumentException("Task ID cannot be empty");

            var attempt = 0;
            var delayMs = 3000;

            while (attempt < maxAttempts && !ct.IsCancellationRequested)
            {
                attempt++;

                try
                {
                    var url = $"https://api.speechflow.io/asr/file/v1/query?taskId={taskId}&resultType={resultType}";
                    var response = await _client.GetAsync(url, ct);
                    var responseJson = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<SpeechFlowResponse>(responseJson);

                    switch (result?.Code)
                    {
                        case 11000: return result.Result;
                        case 11001:
                            await Task.Delay(delayMs, ct);
                            delayMs = Math.Min(delayMs + 1000, 10000);
                            continue;
                        default:
                            throw new Exception($"API error {result?.Code}: {result?.Message}");
                    }
                }
                catch (TaskCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (attempt == maxAttempts)
                        throw new Exception($"Max attempts reached. Last error: {ex.Message}");
                }
            }

            throw new OperationCanceledException("Operation was cancelled");
        }

        public async Task<List<Sentence>> TranscribeAndParseAsync(string filePath, CancellationToken ct = default)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            string taskId = null;

            try
            {
                taskId = await CreateTranscriptionTaskAsync(filePath, "zh", ct);
                var rawResult = await GetTranscriptionResultAsync(taskId, resultType: 1, ct);

                if (string.IsNullOrWhiteSpace(rawResult))
                {
                    Debug.WriteLine("⚠️ Получен пустой результат. Возможно, транскрипция ещё не завершена или taskId некорректный.");
                    return null;
                }

                try
                {
                    var inner = JsonConvert.DeserializeObject<TranscriptionData>(rawResult);
                    return inner?.sentences ?? new List<Sentence>();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Ошибка при разборе JSON: " + ex.Message);
                    return null;
                }


            }
            finally
            {
                try
                {
                    if (File.Exists(filePath))
                        File.Delete(filePath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Не удалось удалить файл {filePath}: {ex.Message}");
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _client?.Dispose();
            _disposed = true;
        }

        private class SpeechFlowResponse
        {
            [JsonProperty("code")]
            public int Code { get; set; }

            [JsonProperty("result")]
            public string Result { get; set; }  // ← исправлено здесь

            [JsonProperty("msg")]
            public string Message { get; set; }

            [JsonProperty("taskId")]
            public string TaskId { get; set; }
        }
    }

    public class TranscriptionData
    {
        public List<Sentence> sentences { get; set; }
        public int version { get; set; }
    }

    public class Sentence
    {
        public int bt { get; set; }
        public int et { get; set; }
        public string s { get; set; }
        public List<ReqWord> words { get; set; }
    }

    public class ReqWord
    {
        public int bt { get; set; }
        public int et { get; set; }
        public string w { get; set; }
    }

}