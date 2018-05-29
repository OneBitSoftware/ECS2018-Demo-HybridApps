using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using WebApplication.Models;

namespace WebApplication.Controllers
{
    public class DatabaseController : Controller
    {
        private const string ConnectionString = "mongodb://localhost:27017";

        // GET: Database
        public async Task<ActionResult> Index()
        {
            var list = await GetSessionsAsync();

            if (list == null)
            {
                list = new List<ConferenceSession>();
            }

            return View(list);
        }

        private async Task<List<ConferenceSession>> GetSessionsAsync()
        {
            var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

            var mongoClient = new MongoClient(clientSettings);

            var database = mongoClient.GetDatabase("ECS2018");

            var collection = database.GetCollection<ConferenceSession>("ConferenceSessions");

            return await collection.Find(c => true).ToListAsync();
        }
    }
}