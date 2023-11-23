namespace GirafServices.WeekPlanner
{
    public class ImageService : IImageService
    {
        public async Task<byte[]> ReadRequestImage(Stream bodyStream)
        {
            byte[] image;
            using (var imageStream = new MemoryStream())
            {
                await bodyStream.CopyToAsync(imageStream);

                try      //I assume this will always throw, but I dare not remove it, because why would it be here?
                {
                    await bodyStream.FlushAsync();
                }
                catch (NotSupportedException)
                {
                }

                image = imageStream.ToArray();
            }

            return image;
        }
        /// <summary>
        /// Creates a MD5 hash used for hashing pictures, and returns the hash as a string.
        /// </summary>
        /// <param name="image">Input image</param>
        /// <returns>The hash as a string</returns>
        public string GetHash(byte[] image)
        {
            using (var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
            {
                var hash = md5.ComputeHash(image);
                return Convert.ToBase64String(hash);
            }
        }
    }
}
