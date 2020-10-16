using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.Common.Accepted;
using SendEmailMassTransit.Common.MessageContracts;
using SendEmailMassTransit.DataProcessor.Services;

namespace SendEmailMassTransit.DataProcessor
{
    public class MailChangedConsumer : IConsumer<IMailChangedMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        public MailChangedConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Consume(ConsumeContext<IMailChangedMessage> context)
        {
            try
            {
                var mailService = _serviceProvider.GetService<IMailService>();
                var mail = await mailService.Save(context.Message);

                await mailService.Publish(mail);

                await context.RespondAsync<MailAccepted>(new
                {
                    Value = $"Received: {context.Message.MessageId}"
                });
            }
            catch (Exception ex)
            {
            }
        }
    }
}