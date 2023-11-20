using GirafRepositories.Interfaces;
using GirafRepositories.Persistence;

namespace GirafRepositories.WeekPlanner
{
    public class ImageRepository : Repository<byte[]>, IImageRepository
    {
        public ImageRepository(GirafDbContext context) : base(context)
        {
            
        }
        /// <summary>
        /// Reads an image from the current request's body and return it as a byte array.
        /// </summary>
        /// <param name="bodyStream">A byte-stream from the body of the request.</param>
        /// <returns>The image found in the request represented as a byte array.</returns>
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
    }
    
}