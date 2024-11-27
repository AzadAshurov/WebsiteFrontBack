using WebApplication1.Utilities.Enums;

namespace WebApplication1.Utilities.Extensions
{
    public static class FileValidator
    {
        public static bool IsFileTypeValid(this IFormFile file, string type)
        {
            if (file.ContentType.Contains(type))
            {
                return true;
            }
            return false;
        }
        public static bool IsFileSizeValid(this IFormFile formFile, FileSize sizeType, int size)
        {

            long maxSizeInBytes = size * (long)Math.Pow(2, (int)sizeType - 3);
            if (formFile.Length > maxSizeInBytes)
            {
                return false;
            }
            return true;
        }

        public static async Task<string> CreateFileAsync(this IFormFile file, params string[] roots)
        {
            string fileName = string.Concat(Guid.NewGuid().ToString(), file.FileName);

            string path = string.Empty;
            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }
            path = Path.Combine(path, fileName);

            using (FileStream fileStream = new(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return fileName;
        }
        public static void DeleteFile(this string filename, params string[] roots)
        {
            string path = CreatePath(filename, roots);


            if (File.Exists(path))
            {
                File.Delete(path);
            }

        }
        private static string CreatePath(string filename, params string[] roots)
        {
            string path = string.Empty;

            for (int i = 0; i < roots.Length; i++)
            {
                path = Path.Combine(path, roots[i]);
            }

            return path = Path.Combine(path, filename);
        }


    }
}
