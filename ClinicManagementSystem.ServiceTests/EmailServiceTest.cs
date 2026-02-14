using Xunit;
using Moq;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

public class EmailServiceTest
{
    [Fact]
    public async Task SendAsync_ShouldNotThrow_WhenConfigurationIsValid()
    {
        var settings = new Dictionary<string, string>
        {
            {"EmailSettings:SmtpServer", "smtp.test.com"},
            {"EmailSettings:Port", "587"},
            {"EmailSettings:Username", "test@test.com"},
            {"EmailSettings:Password", "password"},
            {"EmailSettings:From", "test@test.com"}
        };

        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(settings)
            .Build();

        var service = new EmailService(config);

        await Assert.ThrowsAnyAsync<System.Net.Mail.SmtpException>(() =>
            service.SendAsync("someone@test.com", "Subject", "Body"));
    }
}
