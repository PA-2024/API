using System.Globalization;
using System.Net.Mail;
using API_GesSIgn.Models;

namespace Services {

    public class SendMail
    {  
        public static void SendEmail(string email, string subject, string body)
        {
            string from = Environment.GetEnvironmentVariable("EMAIL_FROM");
            if (from == null) 
                throw new Exception("error");
            string to = email;
            MailMessage message = new MailMessage(from, to);
            message.Subject = subject;
            message.Body = body;
            SmtpClient client = new SmtpClient("server"); // A CHANGER

            client.UseDefaultCredentials = true;

            try
            {
                client.Send(message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception caught in CreateTestMessage2(): {0}",
                    ex.ToString());
            }
        }

        public static void forgetPassword(string email) {

            string subject = "Forget Password";
            string body = "Your password is 1234";

            string token = "1234";    
            SendEmail(email, subject, body);    
        }

        public static void PresenceEmail(Student student, SubjectsHour subjectsHour) {
          
            string token = "TOKEN A FAIRE";
            string body = token; 
            string subject = "Validation de la Presence";
            string email = student.Student_User.User_email;
            SendEmail(email, subject, body);
        }
                

    }
}