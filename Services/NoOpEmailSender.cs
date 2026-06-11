using Microsoft.AspNetCore.Identity.UI.Services;

namespace planinarenje.Services;

public sealed class NoOpEmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        return Task.CompletedTask;
    }
}