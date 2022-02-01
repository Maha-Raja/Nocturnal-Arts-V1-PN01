using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure;
using System.IO;

namespace Nat.Core.Storage
{
   public class BlobStorage
    {

        public async Task<String> InsertBlobStorage(string containerNameConfig, Byte[] bfile, String filename, String contentType = null)
        {

            string storageConnection = CloudConfigurationManager.GetSetting("AzureWebJobsStorage");
            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageConnection);

            //create a block blob
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

            var properties = cloudBlobClient.GetServiceProperties();
            properties.DefaultServiceVersion = "2019-12-12";

            //create a container 
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(Environment.GetEnvironmentVariable(containerNameConfig));

            //create a container if it is not already exists
            if (await cloudBlobContainer.CreateIfNotExistsAsync())
            {
                await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
            }

            CloudBlockBlob blockBlob = cloudBlobContainer.GetBlockBlobReference(filename);

            if (contentType != null) 
            {
                blockBlob.Properties.ContentType = contentType;
            }

            await blockBlob.UploadFromByteArrayAsync(bfile, 0, bfile.Length);

            blockBlob.SetCacheControl("public, max-age=31536000");

            return filename;

        }

    }

    public static class BlobExtensions
    {
        //A convenience method to set the Cache-Control header.
        public static void SetCacheControl(this CloudBlob blob, string value)
        {
            blob.Properties.CacheControl = value;
            blob.SetProperties();
        }
    }
}
