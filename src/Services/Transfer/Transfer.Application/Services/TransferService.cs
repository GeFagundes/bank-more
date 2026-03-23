using Transfer.Application.DTOs;
using Transfer.Application.Interfaces;
using Transfer.Infrastructure.Context;

namespace Transfer.Application.Services
{
    public class TransferService : ITransferService
    {
        private readonly ITransferService _service;
        private readonly TransferDbContext _context;

        public TransferService(ITransferService service, TransferDbContext context)
        {
            _service = service;
            _context = context;
        }

        public Task ExecuteTransferAsync(TransferRequest request, string originAccount)
        {
            throw new NotImplementedException();
        }
    }
}
