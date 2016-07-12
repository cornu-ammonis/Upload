using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Upload.Models
{
    public class CloudFilesModel
    {

        public CloudFilesModel() : this(null)
        {
            Files = new List<CloudFile>();
        }

        public CloudFilesModel(IEnumerable<IListBlobItem> list)
        {
            Files = new List<CloudFile>();
            try
            {
                if (list != null && list.Count<IListBlobItem>() > 0)
                {
                    foreach (var item in list)
                    {
                        CloudFile info = CloudFile.CreateFromIlistBlobItem(item);
                        if (info != null)
                        {
                            Files.Add(info);
                        }
                    }
                }
            }
            catch
            {
                return;
            }
        }

        public List<CloudFile> Files { get; set; }

    }
}