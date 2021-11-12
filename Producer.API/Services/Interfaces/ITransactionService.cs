using Messages;
using System.Threading;
using System.Threading.Tasks;

namespace Producer.API.Services.Interfaces
{
    public interface ITransactionService
    {
        Task CreateTransaction(Transaction transaction, CancellationToken cancellationToken);
    }
}
