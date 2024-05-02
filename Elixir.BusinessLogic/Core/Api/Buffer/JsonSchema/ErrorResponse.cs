namespace Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema
{
    internal class ErrorResponse
    {
        public string Error { get; set; }
        public string Message { get; set; }
        public int Code { get; set; }
    }
}
