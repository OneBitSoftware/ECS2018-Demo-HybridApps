using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication
{
    public interface IGraphService
    {
        GraphServiceClient GetAuthenticatedClient();
    }
}
