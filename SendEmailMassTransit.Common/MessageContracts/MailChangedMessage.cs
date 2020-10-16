using System;
using SendEmailMassTransit.Common.Dto;
using SendEmailMassTransit.Common.MessageContracts;

namespace SendEmailMassTransit.API.ChangedMessage
{
    public class MailChangedMessage:IMailChangedMessage
    {
        public Guid MessageId { get; set; }
        public EmailDto EmailDto { get; set; }
        public DateTime CreationDate { get; set; }
    }
}