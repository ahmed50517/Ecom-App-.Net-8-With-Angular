using Ecom.Core.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecom.Infrastructure.Repositories.Services
{
    public class ImageManagmentService : IImageManagmentService
    {
        private readonly IFileProvider _fileprovider;
        

        public ImageManagmentService(IFileProvider fileprovider)
        {
            _fileprovider = fileprovider;
        }

        public async Task<List<string>> AddImageAsync(IFormFileCollection files, string src)
        {
            var SaveImageSrc = new List<string>();
            var ImageDirectiory= Path.Combine("wwwroot","Images", src);
            if(!Directory.Exists(ImageDirectiory))
            {
                Directory.CreateDirectory(ImageDirectiory);
            }
            foreach (var item in files)
            {
                if (item.Length > 0)
                {
                    //get Image Name
                    var ImageName = item.FileName;
                    var ImageSrc = $"/Images/{src}/{ImageName}";

                    var root = Path.Combine(ImageDirectiory, ImageName);
                    using (FileStream stream = new FileStream(root, FileMode.Create))
                    {
                        await item.CopyToAsync(stream);
                    }
                    SaveImageSrc.Add(ImageSrc);
                }
                
            }
            return SaveImageSrc;
        }

        public void DeleteImageAsync(string src)
        {
            var info = _fileprovider.GetFileInfo(src);
            var root = info.PhysicalPath;
            File.Delete(root);
        }
    }
}
