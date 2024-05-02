using System;
using Elixir.BusinessLogic.Core.Api.Buffer.JsonSchema;

namespace Elixir.BusinessLogic.Core.Api.Buffer
{
    public class BufferApiException : Exception
    {
        public BufferApiException(string message, int code) : base(message)
        {
            ErrorCode = code;
        }

        internal BufferApiException(ErrorResponse response) : base(response?.Error ?? response?.Message)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response), "Unable to parse error result from API.");

            ErrorCode = response.Code;
        }

        public int ErrorCode { get; set; }
    }
}
