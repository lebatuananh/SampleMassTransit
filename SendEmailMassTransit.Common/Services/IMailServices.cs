using System.Threading;
using System.Threading.Tasks;
using SendEmailMassTransit.Common.Dto;

namespace SendEmailMassTransit.Common.Services
{
    public interface IMailServices
    {
        Task Put(EmailDto request, CancellationToken cancellationToken);
    }
}