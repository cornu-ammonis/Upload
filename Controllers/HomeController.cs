using Microsoft.WindowsAzure.Storage;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using Microsoft.WindowsAzure.Storage.Blob;
using Upload.Models;
using System.IO;
using Microsoft.Azure;

namespace Upload.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
            CloudBlobClient storageClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer storageContainer = storageClient.GetContainerReference(ConfigurationManager.AppSettings.Get("CloudStorageContainerReference"));
            CloudFilesModel blobsList = new CloudFilesModel(storageContainer.ListBlobs(useFlatBlobListing: true));
            return View(blobsList);
        }

        public ActionResult SetMetadata(int blocksCount, string fileName, long fileSize)
        {
            var container = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["ConfigurationSectionKey"]).CreateCloudBlobClient().GetContainerReference(ConfigurationManager.AppSettings["CloudStorageContainerReference"]);
            container.CreateIfNotExists();
            var fileToUpload = new CloudFile()
            {
                BlockCount = blocksCount,
                FileName = fileName,
                Size = fileSize,
                BlockBlob = container.GetBlockBlobReference(fileName),
                StartTime = DateTime.Now,
                IsUploadCompleted = false,
                UploadStatusMessage = string.Empty
            };
            Session.Add("CurrentFile", fileToUpload);
            return Json(true);

        }

        public ActionResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var storageClient = storageAccount.CreateCloudBlobClient();
                var storageContainer = storageClient.GetContainerReference(
                 ConfigurationManager.AppSettings.Get("CloudStorageContainerReference"));
                storageContainer.CreateIfNotExists();
               
                //code run once to make container public view
                // BlobContainerPermissions permissions = storageContainer.GetPermissions();
               // permissions.PublicAccess = BlobContainerPublicAccessType.Container;
               // storageContainer.SetPermissions(permissions);

                for (int fileNum = 0; fileNum < Request.Files.Count; fileNum++)
                {
                    string fileName = Path.GetFileName(Request.Files[fileNum].FileName);
                    if (Request.Files[fileNum] != null && Request.Files[fileNum].ContentLength > 0)
                    {
                        CloudBlockBlob azureBlockBlob = storageContainer.GetBlockBlobReference(fileName);
                        azureBlockBlob.UploadFromStream(Request.Files[fileNum].InputStream);
                    }
                }

                return RedirectToAction("Index");


            }
            return View("UploadFile");
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}