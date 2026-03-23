using System;
using System.Collections.Generic;
using System.Text;
using Transfer.Application.DTOs;

namespace Transfer.Application.Interfaces
{
    public interface ITransferService
    {
        Task ExecuteTransferAsync(TransferRequest request, string originAccount);
    }
}
