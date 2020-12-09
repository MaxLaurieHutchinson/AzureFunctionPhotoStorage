using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;
using StoragePhoto.Models;

namespace StoragePhoto
{
    public static class PhotosStorage 
    {
        [FunctionName("PhotosStorage")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
            [Blob("photos", FileAccess.ReadWrite, Connection = Literals.StorageConnectionString)] CloudBlobContainer blobContainer,
            ILogger logger)
        {

            // get request body and convert to json 
            var body = await new StreamReader(req.Body).ReadToEndAsync();
            var request = JsonConvert.DeserializeObject<PhotoUploadModel>(body);

            // create new Id to upload data to storage 
            var newId = Guid.NewGuid();
            var BlobExtentionName = Path.GetExtension(request.Name) ?? "jpg"; // jpg as a catch all // Review: what's best practice? 
            var blobName = $"{newId}.{BlobExtentionName}";  

            // Check if item exitsts & get storage area name
            // Get the file size of the photo to upload 
            await blobContainer.CreateIfNotExistsAsync();

            var CloudBlockBlob = blobContainer.GetBlockBlobReference(blobName);     // name of storage 
            var photoBytes = Convert.FromBase64String(request.Photo);               // file size

            // upload the photo to cloud storage 
            await CloudBlockBlob.UploadFromByteArrayAsync(photoBytes, 0, photoBytes.Length);

            // enter log info for analytics 
            logger?.LogInformation($"successfully Uploaded {newId}.{BlobExtentionName} file");

            // return 200 flag with uplaoded file ID 
            return new OkObjectResult(newId); 

        }
    }
}
