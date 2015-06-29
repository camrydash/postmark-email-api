using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Mail;
using PostmarkDotNet;
using System.Net;

namespace PostMarkEmail
{
    public class MailManager
    {

        #region properties

        private String _From {get;set;}
        public String From
        {
            get
            {
                if (!String.IsNullOrEmpty(_From))
                    return _From;
                return DefaultFrom;
            }
            set
            {
                _From = value;
            }
        }
        public String To { get; set; }
        public String ReplyTo { get; set; }
        public String Cc { get; set; }
        public String Bcc { get; set; }
        public String Subject { get; set; }
        public String Body { get; set; }
        public Boolean IsBodyHtml { get; set; }

        public List<Attachment> Attachments { get; private set; }

        private Boolean? _UseApi;
        public Boolean UseApi
        {
            get
            {
                if (_UseApi.HasValue)
                    return _UseApi.Value;
                return DefaultIsApi;
            }
            set
            {
                UseApi = value;
            }
        }

        #endregion

        #region Static

        public static String DefaultFrom {get;set;}
        public static Boolean DefaultIsApi { get; set; }

        static MailManager()
        {
            var client = new SmtpClient();
            var mm = new MailMessage();
            if (mm.From != null && !String.IsNullOrEmpty(mm.From.Address))
            {
                DefaultFrom = mm.From.Address;
            }

            DefaultIsApi = String.IsNullOrEmpty(client.Host);            
            
        }

        #endregion

        public MailManager():this(null)
        {
        }

        public MailManager(MailMessage message)
        {

            IsBodyHtml = true;
            this.Attachments = new List<Attachment>();

            if (message != null)
            {
                if(message.From!=null && !String.IsNullOrEmpty(message.From.Address))
                    this.From = message.From.Address;

                this.To = message.To.ToAddressList();
                this.Cc = message.CC.ToAddressList();
                this.Bcc = message.Bcc.ToAddressList();
                this.ReplyTo = message.ReplyToList.ToAddressList();
                this.Subject = message.Subject;
                this.Body = message.Body;
                this.IsBodyHtml = message.IsBodyHtml;
                this.Attachments.AddRange(message.Attachments.ToList());
                
            }
        }
         

        public void Send()
        {
            if (String.IsNullOrEmpty(From))
                throw new ArgumentNullException("From Email Address is not provided");
            if (String.IsNullOrEmpty(To))
                throw new ArgumentNullException("To Email Address is not provided");
            if (UseApi)
                SendByApi();
            else
                SendBySmtp();
        }

        public void SendByApi()
        {
            PostmarkMessage message = new PostmarkMessage()
            {
                From = this.From
                , To = this.To 
                , Cc = this.Cc 
                , Bcc = this.Bcc 
                ,Subject = this.Subject 
                , ReplyTo = this.ReplyTo                  
            };

            if (this.IsBodyHtml)
                message.HtmlBody = this.Body;
            else
                message.TextBody = this.Body;

            if (this.Attachments != null && this.Attachments.Count > 0)
            {
                foreach (var attachment in this.Attachments)
                {
                    var bytes = new Byte[attachment.ContentStream.Length];
                    attachment.ContentStream.Read(bytes, 0, bytes.Length);
                    message.AddAttachment(bytes, attachment.ContentType.ToString(), attachment.Name);
                }
            }
            //Created just to get user name which will be used as ApiKey
            SmtpClient sc = new SmtpClient();
            PostmarkClient client = new PostmarkClient((sc.Credentials as NetworkCredential).UserName);
            var response = client.SendMessage(message);
            if (response.Status != PostmarkStatus.Success)
            {
                throw new System.Net.Mail.SmtpException(response.Message);
            }

        }

        public void SendBySmtp()
        {
            var client = new SmtpClient();
            var message = new MailMessage(this.From, this.To, this.Subject, this.Body);
            message.IsBodyHtml = this.IsBodyHtml;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
            message.CC.AddRange(this.Cc);
            message.Bcc.AddRange(this.Bcc);
            message.ReplyToList.AddRange(this.ReplyTo);
            this.Attachments.ForEach(a => message.Attachments.Add(a));          

            client.Send(message);
        }

    }

    public static class Extensions
    {

        public static String ToAddressList(this MailAddressCollection collection)
        {
            if (collection == null)
                return string.Empty;
            return String.Join(",", collection.Select(c => c.Address));
        }

        public static void AddRange(this MailAddressCollection collection, string commaSeparatedList)
        {
            if (String.IsNullOrEmpty(commaSeparatedList)) return;
             collection.AddRange(commaSeparatedList.Split(new char[] {',' }, StringSplitOptions.RemoveEmptyEntries).Select(a=>a.Trim()));
        }
        public static void AddRange(this MailAddressCollection collection, IEnumerable<String> addressList)
        {
            foreach (var address in addressList)
            {
                collection.Add(address);
            }
        }

    }
}
