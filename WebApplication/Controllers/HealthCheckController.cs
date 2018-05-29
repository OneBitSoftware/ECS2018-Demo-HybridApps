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
    public class HealthCheckController : Controller
    {
        private const string ConnectionString = "mongodb://localhost:27017";

        // GET: HealthCheck
        public async Task<ActionResult> Index()
        {
            var result = new HeathCheckStatus();
            result.MongoDbMessage = await ConnectToMongoDbAsync();

            return View(result);
        }

        private async Task<string> ConnectToMongoDbAsync()
        {
            try
            {

                var clientSettings = MongoClientSettings.FromUrl(new MongoUrl(ConnectionString));

                var mongoClient = new MongoClient(clientSettings);

                var database = mongoClient.GetDatabase("ECS2018");

                var collections = await database.ListCollectionsAsync();

            }
            catch (Exception ex)
            {
                return ex.ToString() + " " + ConnectionString;
            }

            return "MongoDB connection successful:" + ConnectionString;
        }
    }
}