using System;
using SendMassTransit.Domain.Entities;

namespace SendEmailMassTransit.Common.MessageContracts
{
    public class MailDetailedMessage : IMailDetailedMessage
    {
        public Guid MessageId { get; set; }
        public Email Email { get; set; }
        public DateTime CreationDate { get; set; }
    }
}