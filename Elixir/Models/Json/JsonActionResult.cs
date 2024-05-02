namespace Elixir.Models.Json
{
    public class JsonActionResult
    {
        public JsonActionResult(bool success, string message = null, bool simulateSuccess = false)
        {
            Success = success;
            Message = message;
            SimulateSuccess = simulateSuccess;
        }

        public bool Success { get; set; }
        public string Message { get; set; }
        public bool SimulateSuccess { get; set; }
    }

    public class JsonActionResult<TData> : JsonActionResult
    {
        public JsonActionResult(bool success, TData data, string message = null, bool simulateSuccess = false) : base(success, message, simulateSuccess)
        {
            Data = data;
        }

        public TData Data { get; set; }
    }
}