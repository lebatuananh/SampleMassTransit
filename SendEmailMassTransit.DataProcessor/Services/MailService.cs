using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SendEmailMassTransit.Common.MessageContracts;
using SendMassTransit.Domain.Entities;

namespace SendEmailMassTransit.DataProcessor.Services
{
    public class MailService : IMailService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MailService> _logger;
        private readonly EmailDbContext _emailDbContext;

        public MailService(IServiceProvider serviceProvider, ILogger<MailService> logger, EmailDbContext emailDbContext)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _emailDbContext = emailDbContext;
        }

        public async Task<Email> Save(IMailChangedMessage email)
        {
            var entity = new Email()
            {
                Body = email.EmailDto.Body,
                Id = new Guid(),
                Subject = email.EmailDto.Subject,
                To = email.EmailDto.To
            };
            await _emailDbContext.Emails.AddAsync(entity);
            await _emailDbContext.SaveChangesAsync();
            return entity;
        }

        public async Task Publish(Email email)
        {
            try
            {
                var publisher = _serviceProvider.GetService<IPublishEndpoint>();

                var mailSavedMessage = new MailDetailedMessage()
                {
                    MessageId = new Guid(),
                    Email = email,
                    CreationDate = DateTime.Now
                };

                await publisher.Publish(mailSavedMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError("MailServiceProducerError", ex);
            }
        }
    }
}