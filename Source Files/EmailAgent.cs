using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;

namespace EmailAgent
{
    public class Credential
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class Emailer
    {
        public List<Credential> credentialsIn = new List<Credential>();

        // will become void function
        public string GetCredentials()
        {
            using (StreamReader r = new StreamReader("appsettings.json")) 
            {
                string json = r.ReadToEnd();
                credentialsIn = JsonSerializer.Deserialize<List<Credential>>(json);
            }
            string credentials_string = "";
            foreach (Credential c in credentialsIn)
            {
                credentials_string += (c.Username + " ");
                //credentials_string += (c.password + " ");
            }
            return credentials_string;
        }

        public int AddAccount(string username, string password) 
        {
            Credential new_acc = new Credential();
            new_acc.Id = credentialsIn.Count+1;
            new_acc.Username = username;
            new_acc.Password = password;
            credentialsIn.Add(new_acc);

            //System.IO.File.WriteAllText("appsettings.json", string.empty);
            string jsonString = JsonSerializer.Serialize(credentialsIn, new JsonSerializerOptions() { WriteIndented = true });
            using (StreamWriter outputFile = new StreamWriter("appsettings.json"))
            {
                outputFile.WriteLine(jsonString);
            }
            return new_acc.Id;
        }

        public async Task SendEmail(int id_number, string to_address, string subject, string body) 
        {
            GetCredentials();
            MailAddress to = new MailAddress(to_address);

            MailAddress from = new MailAddress(credentialsIn[id_number].Username);
            string fromPassword = credentialsIn[id_number].Password;

            //MailAddress from = new MailAddress("claritytestacc1@gmail.com");
            //string fromPassword = "zssrjgpafbbojnhq";
            MailMessage email = new MailMessage()
            {
                From = from,
                Subject = subject,
                Body = body
            };

            email.To.Add(to);

            string host_domain = credentialsIn[id_number].Username.Split('@')[1];

            SmtpClient client = new SmtpClient()
            {
                Host = "smtp." + host_domain,
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential() 
                { 
                    UserName = from.Address,
                    Password = fromPassword
                }
            };

            int retries = 3;
            while (true)
            { 
                try
                {
                    await client.SendMailAsync(email);
                    using (StreamWriter output = File.AppendText("output.txt")) {
                        output.WriteLine("STATUS: Message Sent");
                        output.WriteLine("Sender: " + credentialsIn[id_number].Username);
                        output.WriteLine("Recipient: " + to_address);
                        output.WriteLine("Date: " + DateTime.Now.ToString("MM.dd.yyyy"));
                        output.WriteLine("Subject: " + subject);
                        output.WriteLine("Body:");
                        output.WriteLine(body);
                        output.WriteLine(" ");
                    }
                    break;
                }
                catch (SmtpException ex)
                {
                    if (--retries == 0) {
                        using (StreamWriter output = File.AppendText("output.txt"))
                        {
                            output.WriteLine("STATUS: Error Sending Message");
                            output.WriteLine("Sender: " + credentialsIn[id_number].Username);
                            output.WriteLine("Recipient: " + to_address);
                            output.WriteLine("Date: " + DateTime.Now.ToString("MM.dd.yyyy"));
                            output.WriteLine("Subject: " + subject);
                            output.WriteLine("Body:");
                            output.WriteLine(body);
                            output.WriteLine(" ");
                        }
                        throw;
                    } 
                    else Thread.Sleep(1000);
                    Console.WriteLine(ex.ToString());
                    //return ex.ToString();
                }
            }
            return;
        }
    }


}
