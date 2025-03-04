namespace MyMassNotificationService.Domain.Entities
{
        public class EmailData
        {
            public List<string> RecipientEmails { get; set; } 
            public string Subject { get; set; }
            public string Body { get; set; } 
            public string SenderEmail { get; set; }
            public Dictionary<string, string> TemplateParameters { get; set; }

            public EmailData(List<string> recipientEmails, string subject, string body, string senderEmail = null)
            {
                RecipientEmails = recipientEmails;
                Subject = subject;
                Body = body;
                SenderEmail = senderEmail;
                TemplateParameters = new Dictionary<string, string>();
            }

            public void AddTemplateParameter(string key, string value)
            {
                if (!TemplateParameters.ContainsKey(key))
                {
                    TemplateParameters.Add(key, value);
                }
                else
                {
                    TemplateParameters[key] = value; 
                }
            }
        }
}
