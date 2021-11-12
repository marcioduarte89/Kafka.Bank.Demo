using Messages;
using Microsoft.AspNetCore.Mvc;
using Producer.API.Services.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Producer.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] DTOs.Transaction transaction, CancellationToken cancellationToken)
        {
            var transactionId = Guid.NewGuid();
            await _transactionService.CreateTransaction(new Transaction()
            {
                Id = transactionId,
                UserId = transaction.UserId,
                Value = transaction.Value
            }, cancellationToken);

            return Created($"/transaction/{transactionId}", transactionId);
        }
    }
}
