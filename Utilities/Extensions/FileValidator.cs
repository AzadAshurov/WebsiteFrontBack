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
            if (formFile.Length * 8 > Math.Pow(size, (int)Enum.Parse(typeof(FileSize), sizeType.ToString(), true)))
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



    }
}
