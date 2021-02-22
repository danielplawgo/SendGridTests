using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SendGridTests
{
    public class SendGridConfig
    {
        public string ApiKey { get; set; }

        public string SenderEmail { get; set; }

        public string SenderName { get; set; }

        public string TemplateId { get; set; }

        public string WebhooksKey { get; set; }
    }
}
