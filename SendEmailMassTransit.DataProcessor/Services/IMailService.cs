using System.Threading.Tasks;
using SendEmailMassTransit.Common.MessageContracts;
using SendMassTransit.Domain.Entities;

namespace SendEmailMassTransit.DataProcessor.Services
{
    public interface IMailService
    {
        Task<Email> Save(IMailChangedMessage product);

        Task Publish(Email product);
    }
}