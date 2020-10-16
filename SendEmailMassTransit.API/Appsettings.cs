using SendEmailMassTransit.Common.Configs;

namespace SendEmailMassTransit.API
{
    public class Appsettings
    {
        public QueueSettings QueueSettings { get; set; }

        public Appsettings()
        {

        }

        public Appsettings( QueueSettings queueSettings)
        {
            QueueSettings = queueSettings;
        }  
    }
}