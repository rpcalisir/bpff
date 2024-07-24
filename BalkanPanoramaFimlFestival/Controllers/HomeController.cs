using BalkanPanoramaFimlFestival.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System;
using WkHtmlToPdfDotNet.Contracts;
using WkHtmlToPdfDotNet;

namespace BalkanPanoramaFimlFestival.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConverter _converter;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IConverter converter, ApplicationDbContext context)
        {
            _logger = logger;
            _converter = converter;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(string? firstName, string? lastName, string? email, string? messageContent)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(email))
            {
                // Handle invalid input
                return BadRequest("All fields are required.");
            }

            // Create a new ContactForm instance
            var contactForm = new ContactForm
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Message = messageContent ?? "No message provided."
            };

            // Save the form data to the database
            _context.ContactForms.Add(contactForm);
            await _context.SaveChangesAsync();

            // Generate the PDF content for the user
            string htmlContent = $@"
                <html>
                <head>
                    <meta charset='UTF-8'>
                    <title>PDF Document</title>
                </head>
                <body>
                    <p>Sayın {firstName} {lastName},</p>
                    <p>9. Balkan Panorama Film Festivali'ne yaptığınız başvuru başarılı bir şekilde tamamlanmıştır.</p>
                    <p>Seçilen filmler 03 Nisan 2023 tarihinde festivalin resmi web sitesinde duyurulacaktır.
                    Başvurunuzu takip etmek için  buraya  tıklayabilir ya da aşağıdaki QRKod'u okutabilirsiniz...</p>
                    <p>Başvuru yaptığınız için teşekkür ederiz.</p>
                    <p>Saygılarımızla</p>
                    <p>Balkan Panorama Film Festival Ofisi</p>
                </body>
                </html>";

            byte[] pdfBytes = GeneratePdf(htmlContent);

            // Save the PDF to a file
            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploads))
            {
                Directory.CreateDirectory(uploads);
            }

            var filePath = Path.Combine(uploads, $"{firstName}_{lastName}.pdf");
            System.IO.File.WriteAllBytes(filePath, pdfBytes);

            // Send the email with the PDF attached
            await SendEmail(firstName, lastName, email, filePath, messageContent);

            return RedirectToAction("Index");
        }

        private async Task SendEmail(string? firstName, string? lastName, string? email, string pdfFilePath, string? messageContent)
        {
            var fromAddress = new MailAddress("bearzalk@gmail.com");
            var toAddress = new MailAddress(email!); // Replace with the recipient email address
            const string fromPassword = "hhml qmtd ymjo ndyg";
            const string subject = "New Contact Form Submission";
            string body = $@"
                Dear Support Team,

                A new contact form submission has been received.

                First Name: {firstName}
                Last Name: {lastName}
                Email: {email}
                Message: {messageContent}

                Please find the attached PDF document.

                Best Regards,
                Your Application";

            var smtpClient = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587, // Use 587 for TLS
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };

            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body
            })
            {
                // Add the PDF attachment
                message.Attachments.Add(new Attachment(pdfFilePath, "application/pdf"));
                try
                {
                    await smtpClient.SendMailAsync(message);
                }
                catch (SmtpException ex)
                {
                    // Log or handle the exception
                    Console.WriteLine($"SMTP Exception: {ex.Message}");
                }
            }
        }

        private byte[] GeneratePdf(string htmlContent)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4, // or other PaperKind
                    Orientation = Orientation.Portrait,
                },
                Objects = {
                    new ObjectSettings()
                    {
                        HtmlContent = htmlContent,
                    },
                }
            };

            return _converter.Convert(doc);
        }
    }
}
