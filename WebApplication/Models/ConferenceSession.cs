using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication.Models
{
    public class ConferenceSession
    {
        public ObjectId Id { get; set; }
        public string Title { get; set; }
        public string SpeakerName { get; set; }
    }
}
