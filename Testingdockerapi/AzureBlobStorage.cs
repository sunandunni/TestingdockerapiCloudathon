using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Text;
using Testingdockerapi.Entities;
using System.Collections.Generic;

namespace TestingDockerApi
{
    public static class AzureBlobStorage
    {
        public static async Task<List<Account>> GetAccountBlob(string clientid,BlobContainerClient containerClient = null)
        {
            string line;
            List<Account> accountList = new List<Account>();
            var blobClient = containerClient.GetBlobClient("AccountList.txt");
            if (await blobClient.ExistsAsync())
            {
                var response = await blobClient.DownloadAsync();
                using (var streamReader = new StreamReader(response.Value.Content))
                {
                    while (!streamReader.EndOfStream)
                    {
                        line = await streamReader.ReadLineAsync();
                        if(!string.IsNullOrEmpty(line))
                        {
                            string[] accountsArray = line.Split(new char[] { ','},StringSplitOptions.None);
                            Account account = new Account();
                            account.id = accountsArray[0];
                            account.clientId = accountsArray[1];
                            account.name = accountsArray[2];
                            account.marketValue = Convert.ToDouble(accountsArray[3]);
                            account.isIncluded = Convert.ToBoolean(accountsArray[4]);
                            if (account.clientId.Equals(clientid))
                            {
                                accountList.Add(account);
                            }
                        }
                    }
                }
            }
            //    var file = blobClient.DownloadContent();
            //Console.WriteLine("Uploading to Blob storage");
            //using FileStream uploadFileStream = File.OpenRead(localFile);
            //await blobClient.UploadAsync(uploadFileStream, true);
            //uploadFileStream.Close();
            return accountList;
        }

        public static BlobContainerClient GetBlobContainer()
        {
            string line = string.Empty;
            var connectionString = "DefaultEndpointsProtocol=https;AccountName=otelowlsstorageaccount;AccountKey=Z6uRZ1tMAWLx3uHqBUjKLVu6lRfo1X+G5XQxph+nzLevNEH9KLIoD+WovycF2fkjVrdaCRpkwdg++AStu7PlaQ==;EndpointSuffix=core.windows.net";
            string containerName = "otelowlsblobstorage";
            var serviceClient = new BlobServiceClient(connectionString);
            var containerClient = serviceClient.GetBlobContainerClient(containerName);

            return containerClient;
        }
    }
}