using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGridTests.Models;

namespace SendGridTests.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendGridController : ControllerBase
    {
        private readonly SendGridConfig _config;
        private readonly ILogger<SendGridController> _logger;
        public SendGridController(IOptions<SendGridConfig> config, ILogger<SendGridController> logger)
        {
            _logger = logger;
            _config = config.Value;
        }

        [HttpPost]
        public async Task<IActionResult> Send(SendDto request)
        {
            var client = new SendGridClient(_config.ApiKey);

            var msg = MailHelper.CreateSingleEmail(new EmailAddress(_config.SenderEmail, _config.SenderName),
                new EmailAddress(request.Email, request.Name),
                "Email Subject",
                "Email body",
                "<strong>Email body</strong>");

            var response = await client.SendEmailAsync(msg);

            return await ProcessResponse(response);
        }

        private async Task<IActionResult> ProcessResponse(Response response)
        {
            var responseContent = await response.Body.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseContent) == false)
            {
                return BadRequest(responseContent);
            }

            var header = response.Headers.FirstOrDefault(h => h.Key == "X-Message-Id");

            var messageId = header.Value?.FirstOrDefault();

            _logger.LogInformation($@"MessageId: {messageId}");

            return Ok(new
            {
                MessageId = messageId
            });
        }

        [HttpPut]
        public async Task<IActionResult> SendWithTemplate(SendDto request)
        {
            var client = new SendGridClient(_config.ApiKey);

            var msg = MailHelper.CreateSingleTemplateEmail(new EmailAddress(_config.SenderEmail, _config.SenderName), 
                new EmailAddress(request.Email, request.Name),
                _config.TemplateId, 
                new { Name = request.Name});

            var response = await client.SendEmailAsync(msg);

            return await ProcessResponse(response);
        }
    }
}
