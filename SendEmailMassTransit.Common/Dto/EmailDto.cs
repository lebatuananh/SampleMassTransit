using System;

namespace SendEmailMassTransit.Common.Dto
{
    public class EmailDto
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}