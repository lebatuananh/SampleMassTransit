using System;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.API.ChangedMessage;
using SendEmailMassTransit.Common.Dto;
using SendEmailMassTransit.Common.MessageContracts;

namespace SendEmailMassTransit.Common.Services
{
    public class MailServices : IMailServices
    {
        private readonly ILogger<MailServices> _logger;
        private readonly IPublishEndpoint _endpoint;

        public MailServices(ILogger<MailServices> logger, IPublishEndpoint endpoint)
        {
            _logger = logger;
            _endpoint = endpoint;
        }

        public async Task Put(EmailDto request, CancellationToken cancellationToken)
        {
            try
            {
                var mailChangedRequest = new MailChangedMessage()
                {
                    CreationDate = DateTime.Now,
                    EmailDto = request,
                    MessageId = new Guid()
                };
                await _endpoint.Publish<IMailChangedMessage>(mailChangedRequest, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
    }
}