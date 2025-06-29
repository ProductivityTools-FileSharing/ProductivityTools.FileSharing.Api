using Microsoft.AspNetCore.Mvc;

namespace ProductivityTools.FileSharing.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DebugController : Controller
    {

        public DebugController()
        {
        }

        [HttpGet]
        [Route("Date")]
        public string Date()
        {
            return DateTime.Now.ToString();
        }

        [HttpGet]
        [Route("AppName")]
        public string AppName()
        {
            return "PTFileSharing";
        }

        [HttpGet]
        [Route("Hello")]
        public string Hello(string name)
        {
            return string.Concat($"Hello {name.ToString()} Current date:{DateTime.Now}");
        }

        [HttpGet]
        [Route("ServerName")]
        public string ServerName()
        {
            string server = "NoSQLServer";
            return server;
        }
    }

}
