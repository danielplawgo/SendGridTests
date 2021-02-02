using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public SendGridController(IOptions<SendGridConfig> config)
        {
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

            return string.IsNullOrEmpty(responseContent) ? Ok() : BadRequest(responseContent);
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
