using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Demo.PL.Helper
{
    public static class DocumentSettings
    {
        public static async Task<string> UploadFiles(IFormFile file, string foldername)
        {
            // 1. Get Located Folder Path
            string folderPath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot\\Files" , foldername);

            // 2. Get File Name and make it unique
            string fileName = $"{Guid.NewGuid()}{file.FileName}";
            
            // 3. Get File Path
            string filePath = Path.Combine(folderPath, fileName);

            // 4. Save File as Streams : [Data Per Time]
            using var fileStream = new FileStream(filePath, FileMode.Create);

            await file.CopyToAsync(fileStream);
            return fileName;
        }

        public static void DeleteFile(string fileName, string folderName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory() , "wwwroot\\Files", folderName, fileName);
            
            if(File.Exists(filePath))
                File.Delete(filePath);

        }
    
    }
}
