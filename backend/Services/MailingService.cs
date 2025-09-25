using System;
using backend.Common;
using backend.Data;
using backend.Models;
using MailKit.Net.Smtp;
using MimeKit;

namespace backend.Services
{
    public class MailingService(AppDbContext dbContext, IConfiguration config)
    {
        public int Port { get; } = 465;

        public async Task<Result<EmailConfirmation>> SendConfirmationEmail(User user)
        {
            string senderEmail = config["Mailing:Email"]!;
            string password = config["Mailing:Password"]!;

            if (!user.IsConfirmed)
            {
                var emailConfirmation = new EmailConfirmation { UserId = user.Id };
                await dbContext.EmailConfirmations.AddAsync(emailConfirmation);
                await dbContext.SaveChangesAsync();

                string htmlTemplate = File.ReadAllText("Templates/EmailConfirmationTemplate.html");
                string confirmationLink =
                    $"https://localhost:7073/api/confirm-email?key={emailConfirmation.Key}";
                string emailBody = htmlTemplate.Replace("{confirmation_link}", confirmationLink);

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(senderEmail));
                message.To.Add(MailboxAddress.Parse(user.Email));
                message.Subject = "Email confirmation";
                message.Body = new TextPart("html") { Text = emailBody };

                var client = new SmtpClient();

                try
                {
                    client.Connect("smtp.gmail.com", Port, true);
                    client.Authenticate(senderEmail, password);
                    client.Send(message);
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return new Result<EmailConfirmation>(StatusCode.Unknown, emailConfirmation);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
                return new Result<EmailConfirmation>(StatusCode.OK, emailConfirmation);
            }
            else
            {
                var emailConfirmation = new EmailConfirmation { UserId = user.Id };
                await dbContext.EmailConfirmations.AddAsync(emailConfirmation);
                await dbContext.SaveChangesAsync();

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(senderEmail));
                message.To.Add(MailboxAddress.Parse(user.Email));
                message.Subject = "Account alredy exists";
                message.Body = new TextPart("plain")
                {
                    //Add an option to change password in case the owner wants to do so
                    Text = $"Someone tried to create an account using your email.",
                };

                var client = new SmtpClient();

                try
                {
                    client.Connect("smtp.gmail.com", Port, true);
                    client.Authenticate(senderEmail, password);
                    client.Send(message);
                }
                catch (Exception e)
                {
                    return new Result<EmailConfirmation>(StatusCode.Unknown, emailConfirmation);
                }
                finally
                {
                    client.Disconnect(true);
                    client.Dispose();
                }
                return new Result<EmailConfirmation>(StatusCode.OK, emailConfirmation);
            }
        }
    }
}
