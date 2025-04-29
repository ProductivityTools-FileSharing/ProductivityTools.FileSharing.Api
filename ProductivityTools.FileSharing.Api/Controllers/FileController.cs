using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ProductivityTools.FileSharing.Api.Model;

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

        //[HttpGet(Name = "UploadFile")]
        //public async Task<string> UploadFile()
        //{
        //    var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
        //    await UploadFromFileAsync(blobServiceClient, "d:\\expenses.bak");

        //    return "Hello";
        //}

        [HttpPost(Name = "UploadFile")]
        public ActionResult UploadFile([FromForm] FileModel fileModel)
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            string fileName = Path.GetFileName(fileModel.FormFile.FileName);
            BlobClient blobClient = blobServiceClient.GetBlobClient(fileName);
            using (var stream = fileModel.FormFile.OpenReadStream())
            {
                blobClient.Upload(stream, true);
            }
            return Ok();
        }


        [HttpGet(Name = "Listfiles")]
        public IActionResult Listfiles()
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            List<string> resutls= new List<string>();
            var blobls = blobServiceClient.GetBlobs(BlobTraits.All, BlobStates.All).ToList();
                blobls.ForEach(x =>
             {
                 if (!x.Deleted)
                 {
                     Console.WriteLine(x.Name);
                     resutls.Add(x.Name);
                 }
             });

            return Ok(resutls);
        }
    }
}
