using System;
using SendEmailMassTransit.Common.Dto;

namespace SendEmailMassTransit.Common.MessageContracts
{
    public interface IMailChangedMessage
    {
        Guid MessageId { get; set; }
        EmailDto EmailDto { get; set; }
        DateTime CreationDate { get; set; }
    }
}