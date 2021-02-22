using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using StrongGrid;

namespace SendGridTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendGridWebhooksController : ControllerBase
    {
        private readonly SendGridConfig _config;
        private readonly ILogger<SendGridWebhooksController> _logger;
        public SendGridWebhooksController(IOptions<SendGridConfig> config,
            ILogger<SendGridWebhooksController> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        [HttpPost]
        [Route("InboundEmail")]
        public async Task<IActionResult> ReceiveInboundEmail()
        {
            try
            {
                var signature = Request.Headers[WebhookParser.SIGNATURE_HEADER_NAME];
                var timestamp = Request.Headers[WebhookParser.TIMESTAMP_HEADER_NAME];

                var parser = new WebhookParser();
                var events = await parser.ParseSignedEventsWebhookAsync(Request.Body, _config.WebhooksKey, signature, timestamp);

                _logger.LogInformation(JsonConvert.SerializeObject(events));

                return Ok();
            }
            catch (SecurityException e)
            {
                return BadRequest();
            }
        }
    }
}
