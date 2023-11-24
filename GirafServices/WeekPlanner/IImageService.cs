using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GirafServices.WeekPlanner
{
    public interface IImageService
    {
        /// <summary>
        /// Reads an image from the current request's body and return it as a byte array.
        /// </summary>
        /// <param name="bodyStream">A byte-stream from the body of the request.</param>
        /// <returns>The image found in the request represented as a byte array.</returns>
        Task<byte[]> ReadRequestImage(Stream bodyStream);
        /// <summary>
        /// Creates a MD5 hash used for hashing pictures, and returns the hash as a string.
        /// </summary>
        /// <param name="image">Input image</param>
        /// <returns>The hash as a string</returns>
        string GetHash(byte[] image);
    }
}
