using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class MailViewModel
    {
        public string Subject { get; set; }
        public string FromEmailAddress { get; set; }
        public string Body { get; set; }
        public bool HasAttachments { get; set; }
        public IMessageAttachmentsCollectionPage Attachments { get; set; }

        public MailViewModel() { }

        public MailViewModel(Message message)
        {
            Subject = Cleanup(message.Subject);
            FromEmailAddress = message?.From?.EmailAddress?.Address;
            Body = Cleanup(message.BodyPreview);
            HasAttachments = message.HasAttachments ?? message.HasAttachments.Value;
            Attachments = message.Attachments;
        }

        #region Cleanup
        private string Cleanup(string message)
        {
            message = message.ToLower().Replace("isg", "");
            message = message.ToLower().Replace("cablecom", "");
            message = message.ToLower().Replace("cable com", "");

            return message;
        }
        #endregion
    }
}
