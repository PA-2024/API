using System;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using API_GesSIgn.Models;

namespace Services
{
    /// <summary>
    /// Classe pour envoyer des mails
    /// </summary>
    public class SendMail
    {
        private static readonly string SmtpServer = "pro3.mail.ovh.net";
        private static readonly int SmtpPort = 587;
        private static readonly bool EnableSsl = true;
        private static readonly string FromEmail = "admin@gessign.com";
        private static readonly string SmtpPassword = Environment.GetEnvironmentVariable("MYAPP_PASSWORD_API_MAIL");

        /// <summary>
        /// Méthode pour envoyer un mail
        /// </summary>
        /// <param name="to"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        public static int SendEmail(string to, string subject, string body)
        {
            try
            {
                MailMessage message = new MailMessage(FromEmail, to)
                {
                    Subject = subject,
                    IsBodyHtml = true
                };

                var plainTextView = AlternateView.CreateAlternateViewFromString("This is the plain text part of the email", null, "text/plain");
                var htmlView = AlternateView.CreateAlternateViewFromString(body, null, "text/html");

                message.AlternateViews.Add(plainTextView);
                message.AlternateViews.Add(htmlView);

                using (SmtpClient client = new SmtpClient(SmtpServer, SmtpPort))
                {
                    client.EnableSsl = EnableSsl;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(FromEmail, SmtpPassword);

                    client.Send(message);
                }

                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception caught in SendEmail(): {ex}");
                return 42;
            }
        }

        /// <summary>
        /// envoie mail passeword
        /// </summary>
        /// <param name="email"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static int SendForgetPasswordEmail(string email, int user_id, string token)
        {
            string subject = "Reset Your Password";           
            string resetUrl = $"https://gesign.wstr.fr/reset-password/{user_id}?token={token}";
            string body = $@"
                <p>You have requested to reset your password. Please click the button below to reset your password.</p>
                <a href='{resetUrl}' style='display:inline-block;padding:10px 20px;margin:10px 0;border-radius:5px;background-color:#28a745;color:#fff;text-decoration:none;'>Reset Password</a>
                <p>If you did not request a password reset, please ignore this email.</p>";

            return SendEmail(email, subject, body);
             
        }

        /// <summary>
        /// Notification de la validation de votre mail 
        /// </summary>
        /// <param name="student"></param>
        /// <param name="subjectsHour"></param>
        /// <param name="isApproved"></param>
        /// <param name="context"></param>
        public static void SendPresenceEmail(Student student, SubjectsHour subjectsHour, bool isApproved)
        {
            string subject = "Presence Validation";
            string status = isApproved ? "approved" : "refused";
            string body = $@"
                <p>Your absence justification has been {status}.</p>
                <p>Details:</p>
                <ul>
                    <li>Student: {student.Student_User.User_email}</li>
                    <li>Subject: {subjectsHour.SubjectsHour_Subjects.Subjects_Name}</li>
                    <li>Date: {subjectsHour.SubjectsHour_DateStart}</li>
                </ul>";

            SendEmail(student.Student_User.User_email, subject, body);
        }

        
        /// <summary>
        /// Méthode pour générer un token de réinitialisation
        /// </summary>
        /// <returns></returns>
        public static string GenerateResetToken()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
