using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using employeesTaskManager.Data;
using employeesTaskManager.Models;
using Microsoft.AspNetCore.Identity;
using System.Net.Mail;
using System.Net;
using NuGet.Common;

namespace employeesTaskManager.Controllers
{
    public class passwordTokensController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<ApplicationUser> _userManager;

        public passwordTokensController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: passwordTokens

        public IActionResult ChangePassword()
        {
            return View();
        }

        // POST: passwordTokens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(string email)
        {
            ApplicationUser user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return View("MessageView", "Použivateľ z daným emailom sa nenašiel");
            var existingToken = _context.passwordToken.FirstOrDefault(x => x.userId == user.Id);
            if (existingToken != null)
            {
                var timePassed = (DateTime.Now - existingToken.CreatedDate).TotalSeconds;
                if (timePassed > 300)
                {
                    _context.passwordToken.Remove(existingToken);
                }
                else
                {
                    return View("MessageView", $"Token na tento email už bol odoslaný, nový token môžete vytvoriť o {(int)(timePassed)} sekúnd");
                }
            }
            passwordToken newToken = new passwordToken() { ID = Guid.NewGuid().ToString(), userId = user.Id, CreatedDate = DateTime.Now };
            SendConfirmationEmail(user.Email,newToken.ID);
            _context.Add(newToken);
            _context.SaveChanges();
            return View("MessageView", "email bol odoslaný");
        }

        public async Task<IActionResult> Token(string token)
        {
            if(token == null)
                Redirect(nameof(ChangePassword));
            var myToken = _context.passwordToken.FirstOrDefault(x => x.ID == token);
            if (myToken != null)
            {
                var timePassed = (DateTime.Now - myToken.CreatedDate).TotalSeconds;
                if (timePassed > 400)
                {
                    _context.passwordToken.Remove(myToken);
                    _context.SaveChanges();
                    return View("MessageView", "Čas na uplatnenie vypršal");
                }
            }
            else
            {
                return View("MessageView", "Token sa nenašiel");
            }
            return View(new PasswordResetResult() { tokenId = myToken.ID });
        }
        public async Task<IActionResult> ApplyToken(PasswordResetResult values)
        {
            if (values.tokenId == null || values.NewPassword == null)
                Redirect(nameof(ChangePassword));
            var myToken = _context.passwordToken.FirstOrDefault(x => x.ID == values.tokenId);
            if (myToken != null)
            {
                var timePassed = (DateTime.Now - myToken.CreatedDate).TotalSeconds;
                if (timePassed > 400)
                {
                    _context.passwordToken.Remove(myToken);
                    _context.SaveChanges();
                    return View("MessageView", "Čas na uplatnenie vypršal");
                }
            }
            else
            {
                return View("MessageView", "Token sa nenašiel");
            }
            ApplicationUser user = await _userManager.FindByIdAsync(myToken.userId);
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, values.NewPassword);
            _context.passwordToken.Remove(myToken);
            _context.SaveChanges();
            return View("MessageView","Heslo bolo úspešne zmenené.");
        }


        public void SendConfirmationEmail(string to, string changePasswordLink)
        {
            string passwordResetUrl = Url.Action(
                action: "Token",          
                controller: "passwordTokens",
                values: new { token = changePasswordLink },
                protocol: Request.Scheme);


            string emailHtml =
    $@"<!DOCTYPE html>
    <html lang=""sk"">
    <head>
        <meta charset=""UTF-8"">
        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
        <title>Potvrdenie Zmeny Hesla</title>
        <style>
            body {{
                font-family: 'Arial', sans-serif;
                background-color: #f4f4f4;
                margin: 0;
                padding: 0;
            }}

            .container {{
                width: 80%;
                margin: 0 auto;
                background-color: #ffffff;
                padding: 20px;
                border-radius: 10px;
                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                margin-top: 50px;
            }}

            h1 {{
                color: #333;
            }}

            p {{
                color: #555;
            }}

            .change-password-link {{
                background-color: #007bff;
                color: #ffffff;
                padding: 10px;
                border-radius: 5px;
                font-size: 18px;
                margin-top: 20px;
                cursor: pointer;
                text-decoration: none; /* Add this to remove underline */
                display: inline-block; /* Add this to make it look like a button */
            }}

            .footer {{
                margin-top: 20px;
                text-align: center;
                color: #888;
            }}
        </style>
    </head>
    <body>
        <div class=""container"">
            <h1>Potvrdenie Zmeny Hesla</h1>
            <p>Pre zmenu hesla kliknite na nasledujúci odkaz:</p><br/>
            <a class=""change-password-link"" href=""{passwordResetUrl}"">Zmeniť Heslo</a>
            <br/>
            <br/>
            <p>Ak ste nevykonali žiadnu zmenu hesla, prosím, kontaktujte nás ihneď.</p>
        </div>
    </body>
    </html>
    ";


            MailAddress reciver = new MailAddress(to);
            MailAddress from = new MailAddress("itiqcrew@gmail.com");

            MailMessage email = new MailMessage(from, reciver);
            email.Subject = "Password reset";
            email.IsBodyHtml = true;
            email.Body = emailHtml;

            SmtpClient smtp = new SmtpClient();
            smtp.Host = "smtp.gmail.com";
            smtp.Port = 587;
            smtp.Credentials = new NetworkCredential("itiqcrew@gmail.com", "kcoyjzdjrmbvhqiw");
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.EnableSsl = true;

            try
            {
                smtp.Send(email);
            }
            catch (SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
