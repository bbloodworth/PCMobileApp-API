using System;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;

namespace DynamicAnimation.Common
{
    public class AzureBlobManager
    {
        public static string GetBlobSasUrl(string containerName, string fileName)
        {
            string storageAccountName = "StorageAccountName".GetConfigurationValue();
            string storageAccountKey = "StorageAccountKey".GetConfigurationValue();
            int viewWindowInMinutes = "ViewWindowInMinutes".GetConfigurationNumericValue();

            //CloudStorageAccount storageAccount =
            //  CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageAccountConnectionString"]);

            StorageCredentials storageCredentials = new StorageCredentials(storageAccountName, storageAccountKey);
            CloudStorageAccount storageAccount = new CloudStorageAccount(storageCredentials, true);

            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference(containerName);

            IEnumerable<IListBlobItem> blobItems = blobContainer.ListBlobs();
            foreach (var blobItem in blobItems)
            {
                // You can iterate through a container's items
                string absUri = blobItem.StorageUri.PrimaryUri.AbsoluteUri;
                string absPath = blobItem.Uri.AbsolutePath;
            }

            CloudBlockBlob videoBlob = blobContainer.GetBlockBlobReference(fileName);

            SharedAccessBlobPolicy blobPolicy = new SharedAccessBlobPolicy
            {
                Permissions = SharedAccessBlobPermissions.Read,
                SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(viewWindowInMinutes)
            };

            string sas = videoBlob.GetSharedAccessSignature(blobPolicy);

            BlobContainerPermissions containerPermissions = new BlobContainerPermissions();
            containerPermissions.PublicAccess = BlobContainerPublicAccessType.Off;

            containerPermissions.SharedAccessPolicies.Add(
              "fiveminutepolicy", new SharedAccessBlobPolicy
              {
                  SharedAccessStartTime = DateTime.UtcNow.AddMinutes(-1),
                  SharedAccessExpiryTime = DateTime.UtcNow.AddMinutes(viewWindowInMinutes),
                  Permissions = SharedAccessBlobPermissions.Write | SharedAccessBlobPermissions.Read 
              });

            //blobContainer.GetPermissions();
            //blobContainer.SetPermissions(containerPermissions);
            //string sas = blob.GetSharedAccessSignature(new SharedAccessBlobPolicy(), "fiveminutepolicy");
            //string containerSas = blobContainer.GetSharedAccessSignature(new SharedAccessPolicy(), "fiveminutepolicy ");

            string blobUri = videoBlob.Uri.AbsoluteUri;
            return string.Format("{0}{1}", blobUri, sas);
        }
    }
}