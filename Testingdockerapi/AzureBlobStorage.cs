using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
namespace TestingDockerApi
{ 
    public static class AzureBlobStorage
    {
        public static async Task<string> GetBlob(string fileName = null)
        {
            string line = string.Empty;
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=sunandstorageaccount;AccountKey=zcXp1Ug47/wsugiHIpKvffSsdtoGdnPt3NPuWFc9ZefsXiOdD0QZSqAHiJ+S5LXK/bsO+/rJErNC+AStWZhwWw==;EndpointSuffix=core.windows.net";
            string containerName = "sunandblobstorage";
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);
           
            //var path = @"C:\Users\sunandunni\Projects\TestingdockerapiCloudathon";
            //var fileName = "UploadTest.txt";
            //var localFile = Path.Combine(path, fileName);
            //await File.WriteAllTextAsync(localFile, "This is a test message");
            var blobClient = containerClient.GetBlobClient("aksdeployment.txt");
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        line = await streamReader.ReadLineAsync();
                        Console.WriteLine(line);
                    }
                }
            }
            //    var file = blobClient.DownloadContent();
            //Console.WriteLine("Uploading to Blob storage");
            //using FileStream uploadFileStream = File.OpenRead(localFile);
            //await blobClient.UploadAsync(uploadFileStream, true);
            //uploadFileStream.Close();
            return line;
        }
    }
}