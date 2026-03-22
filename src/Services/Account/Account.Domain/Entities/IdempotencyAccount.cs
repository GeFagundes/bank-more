using System;
using System.Collections.Generic;
using System.Text;

namespace Account.Domain.Entities
{
    public class IdempotencyAccount
    {
        public string IdempotencyKey { get; private set; }
        public string RequestBody { get; private set; }
        public string ResponseBody { get; private set; }

        public IdempotencyAccount(string idempotencyKey, string requestBody, string responseBody)
        {
            IdempotencyKey = idempotencyKey;
            RequestBody = requestBody;
            ResponseBody = responseBody;
        }
    }
}
