using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

namespace ProductivityTools.FileSharing.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FileController : ControllerBase
    {

        public FileController(IConfiguration conf)
        {
            this.Configuration = conf;
        }
        private readonly IConfiguration Configuration;

        private BlobServiceClient GetBlobServiceClient()
        {
            string connectionstring = Configuration["ConnectionString"];
            BlobServiceClient client = new(connectionstring);

            return client;
        }

        private static async Task UploadFromFileAsync(BlobContainerClient containerClient, string localFilePath)
        {
            string fileName = Path.GetFileName(localFilePath);
            BlobClient blobClient = containerClient.GetBlobClient(fileName);

            await blobClient.UploadAsync(localFilePath, true);
        }

        [HttpGet(Name = "UploadFile")]
        public async Task<string> UploadFile()
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            await UploadFromFileAsync(blobServiceClient, "d:\\expenses.bak");

            return "Hello";
        }
    }
}
