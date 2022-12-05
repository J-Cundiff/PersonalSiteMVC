using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using PersonalSiteMVC.Models;
using System.Diagnostics;

namespace PersonalSiteMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IConfiguration _config;

        public HomeController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
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

        public IActionResult Portfolio()
        {
            return View();
        }

        public IActionResult Resume()
        {
            return View();
        }

        public IActionResult About()
        {
            return View();
        }

        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactViewModel cvm)
        {
            if (!ModelState.IsValid)
            {
                //Send the user back to the form. We can pass the object to the View
                //so the form will contain the original information they provided.

                return View(cvm);
            }

            //Create the format for the message content we will receive from the form
            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />Message: {cvm.Message}";

            //Create a MimeMessage Object to assist with storing and transporting the Email information
            //from the contact form
            var mm = new MimeMessage();

            //We can access the creditials for our email user from the appsettings.json file:
            mm.From.Add(new MailboxAddress("Sender", _config.GetValue<string>("Credentials:Email:User")));

            //The recipient of this email willbe our personal email address, also stored in the appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _config.GetValue<string>("Credentials:Email:Recipient")));

            //The subject of the message will be the one provided by the user, which is stored in the cvm object
            mm.Subject = cvm.Subject;

            //The body of the message will be the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message as "Urgent" so it will be flagged in our email client
            mm.Priority = MessagePriority.Urgent;

            //We can also add the users provided email address to the list of ReplyTo Address
            //so our replies can be sent directly to them instead of our email user
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create the SmtpClient used to send the email.
            //Once all of the code inside of the using directive scope has been executed it
            //will close any open connections and dispose of the object for us.
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using the credentials in our appsettings.json
                client.Connect(_config.GetValue<string>("Credentials:Email:Client"));

                //Log in to the mail server using the credentials for our email user
                client.Authenticate(

                    //Username
                    _config.GetValue<string>("Credentials:Email:User"),

                    //Password
                    _config.GetValue<string>("Credentials:Email:Password")

                    );

                //It's possible the mail server may be down when the user attempts to contact us
                //or we may have issues in our code. So let's wrap or code to send the message 
                //in a try/catch block

                try
                {
                    //Try to send the email
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If theres an issue we can store an error message in a ViewBag variable 
                    // to be displayed in the View
                    ViewBag.ErrorMessage = $"There was an error processing your request." +
                        $"Please try again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with their form information intact
                    return View(cvm);

                }



            }



            return View("EmailConfirmation", cvm);
        }

        
    }
}