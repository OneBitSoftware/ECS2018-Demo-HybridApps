namespace Microsoft.AspNetCore.Authentication
{
    public class AzureAdOptions
    {
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        public string Instance { get; set; }

        public string Domain { get; set; }
        
        public string TenantId { get; set; }

        public string CallbackPath { get; set; }

        public string MicrosoftGraphResourceId { get; set; }

        public string MailClientId { get; set; }
        public string MailUserName { get; set; }
        public string MailPassword { get; set; }
    }
}
