using System;
using System.Collections.Generic;
using System.Text;

namespace Account.Application.DTOs
{
    public record TransactionRequest(
         string RequestId,
         string? AccountNumber,
         decimal Value,
         string Type
     );
}
