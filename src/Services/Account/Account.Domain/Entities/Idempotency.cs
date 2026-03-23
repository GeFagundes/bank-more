using System;
using System.Collections.Generic;
using System.Text;

namespace Account.Domain.Entities
{
    public class Idempotency
    {
        public string IdempotencyKey { get; private set; }
        public string RequestBody { get; private set; }
        public string ResponseBody { get; private set; }

        public Idempotency(string idempotencyKey, string requestBody, string responseBody)
        {
            IdempotencyKey = idempotencyKey;
            RequestBody = requestBody;
            ResponseBody = responseBody;
        }
    }
}
