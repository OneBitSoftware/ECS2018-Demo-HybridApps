using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class MailController : Controller
    {
        private IGraphService _graphService; 

        public MailController(IGraphService graphService)
        {
            _graphService = graphService ?? throw new ArgumentNullException(nameof(graphService));
        }

        // GET: Mail
        public async Task<ActionResult> Index()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var allMails = await GetAllMailViewModelMessages();
            sw.Stop();

            ViewBag.ElapsedTimeMails = sw.ElapsedMilliseconds;

            return View(allMails);
        }

        public async Task<IActionResult> ReadAllMails()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            var allMails = await GetAllMailViewModelMessages();
            sw.Stop();

            ViewBag.ElapsedTimeMails = sw.ElapsedMilliseconds;

            return View(allMails);
        }

        private async Task<List<MailViewModel>> GetAllMailViewModelMessages()
        {
            List<MailViewModel> mailViewModels = new List<MailViewModel>();
            var messages = await GetTopMessagesAsync(5, includeAttachments: true);

            foreach (var message in messages)
            {
                MailViewModel mvm = new MailViewModel(message);

                mailViewModels.Add(mvm);
            }

            return mailViewModels;
        }

        public async Task<IMailFolderMessagesCollectionPage> GetTopMessagesAsync(int numberOfMessages = 10, bool includeAttachments = false)
        {
            if (numberOfMessages < 0) throw new ArgumentNullException(nameof(numberOfMessages));

            var graphClient = _graphService.GetAuthenticatedClient();
            var request = graphClient.Me.MailFolders.Inbox.Messages.Request();
            if (includeAttachments)
            {
                // This includes the attachments to be requested along with the messages request
                request.Expand("attachments");
            }
            //This request header is added, in order to have a plain text Message Body
            request.Headers.Add(new HeaderOption("Prefer", "outlook.body-content-type='text'"));
            var messages = await request.Top(numberOfMessages).GetAsync();

            // CR: 20171611 Logging

            return messages;
        }
    }
}