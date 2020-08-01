using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace web.Services
{
    public class DummyEmailSender : IEmailSender
    {
        private readonly ILogger<DummyEmailSender> _logger;

        public DummyEmailSender(ILogger<DummyEmailSender> logger) 
        {
            _logger = logger;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogWarning("Dummy EmailSender implementation is being used!!!");
            _logger.LogDebug($"{email}{Environment.NewLine}{subject}{Environment.NewLine}{htmlMessage}");
            return Task.CompletedTask;
        }
    }
}
