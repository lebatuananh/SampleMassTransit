using System;

namespace SendEmailMassTransit.Common.Accepted
{
    public class MailAccepted
    {
        public Guid MessageId { get; set; }

        public bool Accepted { get; set; }
    }
}