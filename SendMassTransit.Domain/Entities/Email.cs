using System;

namespace SendMassTransit.Domain.Entities
{
    public class Email
    {
        public Guid Id { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}