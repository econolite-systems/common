// SPDX-License-Identifier: MIT
// Copyright: 2023 Econolite Systems, Inc.
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Econolite.Ode.Cloud.Common.Models;

namespace Econolite.Ode.Cloud.Common.Managers
{
    public class AzureStorageManager
    {
        public static void CreateAccount()
        {
            throw new NotImplementedException();
        }

        public static void DeleteAccount()
        {
            throw new NotImplementedException();
        }

        public static BlobContainerClient CreateContainer(string connectionString, string containerName)
        {
            throw new NotImplementedException();
        }

        public static BlobContainerClient CreateContainer(BlobServiceClient client, string containerName)
        {
            var container = client.GetBlobContainerClient(containerName);
            container.CreateIfNotExists(PublicAccessType.None);
            return container;
        }

        public static void DeleteContainer()
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<BlobHierarchyItem> GetCloudBlockBlobList(string connectionString, string containerName, string blobNamePrefix)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            return container.GetBlobsByHierarchy(prefix: blobNamePrefix, delimiter: "/");
        }

        public static IEnumerable<BlobItem> GetCloudBlockBlobListFlat(string connectionString, string containerName, string blobNamePrefix)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            return container.GetBlobs(prefix: blobNamePrefix);
        }

        public static List<BlobItem> GetCloudBlockBlobList(BlobContainerClient container, string blobNamePrefix)
        {
            return container.GetBlobs(prefix: blobNamePrefix).ToList();
        }

        public static void UploadData(string connectionString, string containerName, string blobName, byte[] data, BlobUploadOptions? options = null)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            container.CreateIfNotExists(PublicAccessType.None);

            var blob = container.GetBlobClient(blobName);

            if (!blob.Exists())
            {
                blob.Upload(BinaryData.FromBytes(data), options);
            }
        }

        public static async Task UploadDataAsync(string connectionString, string containerName, string blobName, byte[] data, BlobUploadOptions? options = null)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            container.CreateIfNotExists(PublicAccessType.None);

            var blob = container.GetBlobClient(blobName);

            if (!blob.Exists())
            {
                await blob.UploadAsync(BinaryData.FromBytes(data), options);
            }
        }

        public static byte[] DownloadData(string connectionString, string containerName, string blobName)
        {
            var data = new byte[0];

            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            var blob = container.GetBlobClient(blobName);

            if (blob.Exists())
            {
                data = blob.DownloadContent().Value.Content.ToArray();
            }

            return data;
        }

        public static byte[] DownloadData(Azure.Storage.Blobs.BlobContainerClient container, string blobName)
        {
            var data = new byte[0];

            var blob = container.GetBlobClient(blobName);

            if (blob.Exists())
            {
                var download = blob.DownloadContent();
                data = download.Value.Content.ToArray();
            }

            return data;
        }

        public static BlobServiceClient GetAccount(string connectionString)
        {
            return new BlobServiceClient(connectionString);
        }

        public static async Task<List<TaggedBlobItem>> FindBlobsByTagsAsync(string connectionString, string containerName, string query)
        {
            //https://learn.microsoft.com/en-us/rest/api/storageservices/find-blobs-by-tags
            //https://learn.microsoft.com/en-us/azure/storage/blobs/storage-manage-find-blobs?tabs=azure-portal
            //All tag values are strings. The supported binary relational operators use a lexicographic sorting of the tag values.
            //To support non-string data types, including numbers and dates, you must use appropriate padding and sortable formatting.
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            List<TaggedBlobItem> blobs = new List<TaggedBlobItem>();
            await foreach (TaggedBlobItem taggedBlobItem in container.FindBlobsByTagsAsync(query))
            {
                blobs.Add(taggedBlobItem);
            }
            return blobs;
        }

        public static async Task DeleteBlobIfExistsAsync(string connectionString, string containerName, string blobName)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);
            await container.DeleteBlobIfExistsAsync(blobName);
        }

        public static async Task<AzureContainerSummary> GetContainerSummaryAsync(string connectionString, string containerName, bool includeBlobs = false)
        {
            var account = GetAccount(connectionString);
            var container = account.GetBlobContainerClient(containerName);

            long blobSize = 0;
            long blobCount = 0;
            List<BlobItem> blobs = new List<BlobItem>();
            await foreach (var blob in container.GetBlobsAsync(BlobTraits.Tags))
            {
                blobCount++;
                blobSize += blob.Properties.ContentLength.GetValueOrDefault();
                if (includeBlobs)
                {
                    blobs.Add(blob);
                }                  
            }

            var containerSummary = new AzureContainerSummary() { BlobCount = blobCount, ContainerByteSize = blobSize, Blobs = blobs};

            return containerSummary;
        }
    }
}
