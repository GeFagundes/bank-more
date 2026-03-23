using System;
using System.Collections.Generic;
using System.Text;

namespace Transfer.Application.DTOs
{
    public class TransferRequest
    {
        public string RequestId { get; set; } = null!;
        public string DestinationAccount { get; set; } = null!;
        public decimal Value { get; set; }
    }
}
