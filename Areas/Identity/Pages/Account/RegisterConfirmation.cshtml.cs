// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace employeesTaskManager.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _sender;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager, IEmailSender sender)
        {
            _userManager = userManager;
            _sender = sender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public bool DisplayConfirmAccountLink { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string EmailConfirmationUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(string email, string returnUrl = null)
        {
            if (email == null)
            {
                return RedirectToPage("/Index");
            }
            returnUrl = returnUrl ?? Url.Content("~/");

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return NotFound($"Unable to load user with email '{email}'.");
            }

            Email = email;
            // Once you add a real email sender, you should remove this code that lets you confirm the account
            DisplayConfirmAccountLink = true;
            if (DisplayConfirmAccountLink)
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                EmailConfirmationUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                    protocol: Request.Scheme);
                SendConfirmationEmail(email, EmailConfirmationUrl);
            }

            return Page();
        }
        public void SendConfirmationEmail(string to, string confirmationCode)
        {
            string emailHtml = 
                $@"<!DOCTYPE html>
                <html lang=""sk"">
                <head>
                    <meta charset=""UTF-8"">
                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                    <title>Potvrdenie E-mailu</title>
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

                        .confirmation-code {{
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 10px;
                            border-radius: 5px;
                            font-size: 18px;
                            margin-top: 20px;
                            cursor: pointer;
                        }}

                        a {{
                            color: #007bff;
                            text-decoration: none;
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
                        <h1>Potvrdenie E-mailu</h1>
                        <p>Ďakujeme za registráciu na našom webe. Prosím, použite nižšie uvedený potvrdzovací kód na overenie Vášho e-mailu:</p><br/>
                        <a class=""confirmation-code"" href=""{confirmationCode}"">Potvrdiť</a>
                        <br/>
                        <br/>
                        <p>Ak ste sa na našom webe neregistrovali, prosím, tento e-mail ignorujte.</p>
                    </div>
                </body>
                </html>
                ";

            MailAddress reciver = new MailAddress(to);
            MailAddress from = new MailAddress("itiqcrew@gmail.com");

            MailMessage email = new MailMessage(from, reciver);
            email.Subject = "Email verification";
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
