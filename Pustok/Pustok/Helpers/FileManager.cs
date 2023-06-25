using Pustok.Models;

namespace Pustok.Helpers
{
    public static class FileManager
    {
        public static string Save(IFormFile file,string rootPath,string folder)
        {
            string newFileName = Guid.NewGuid().ToString() + (file.FileName.Length <= 64 ? file.FileName : file.FileName.Substring(file.FileName.Length - 64));
            string path = Path.Combine(rootPath,folder, newFileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            return newFileName;
        }

        public static void Delete(string rootPath, string folder,string fileName)
        {
            string path = Path.Combine(rootPath,folder,fileName);

            if(File.Exists(path))
            {
                File.Delete(path);
            }
        }

        public static void DeleteAll(string rootPath, string folder, List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                string path = Path.Combine(rootPath, folder, fileName);

                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
        }
    }
}
