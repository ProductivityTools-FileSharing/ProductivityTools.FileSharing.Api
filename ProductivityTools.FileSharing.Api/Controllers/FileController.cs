using Microsoft.AspNetCore.Mvc;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using ProductivityTools.FileSharing.Api.Model;
using Microsoft.AspNetCore.Authentication;

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
        [DisableRequestSizeLimit]
        [RequestFormLimits(MultipartBodyLengthLimit = 1073741824)]
        public ActionResult UploadFile([FromForm] FileModel fileModel)
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            string fileName = Path.GetFileName(fileModel.FormFile.FileName);
            BlobClient blobClient = blobServiceClient.GetBlobClient(fileName);
            try
            {

                using (var stream = fileModel.FormFile.OpenReadStream())
                {
                    blobClient.Upload(stream, true);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error uploading file: {ex.Message}");
                throw;
            }

            return Ok();
        }


        [HttpGet(Name = "Listfiles")]
        public IActionResult Listfiles()
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            List<AzureFile> resutls = new List<AzureFile>();
            var blobls = blobServiceClient.GetBlobs(BlobTraits.All, BlobStates.All).ToList();
            blobls.ForEach(x =>
         {
             if (!x.Deleted)
             {
                 var azureFile = new AzureFile
                 {
                     Name = x.Name,
                     Size = x.Properties.ContentLength.HasValue ? (int)x.Properties.ContentLength / 1024 / 1024 : 0,
                     Created = x.Properties.CreatedOn.HasValue ? x.Properties.CreatedOn.Value.DateTime : null
                 };
                 Console.WriteLine(x.Name);
                 resutls.Add(azureFile);
             }
         });

            return Ok(resutls);
        }

        public class F
        {
            public string FileName { get; set; }
        }
        [HttpDelete(Name = "DeleteFile")]
        public IActionResult DeleteFile(F fileName)
        {
            var blobServiceClient = GetBlobServiceClient().GetBlobContainerClient("filecontainergs");
            var r = blobServiceClient.DeleteBlobIfExists(fileName.FileName);
            return Ok(r);
        }
    }
}
