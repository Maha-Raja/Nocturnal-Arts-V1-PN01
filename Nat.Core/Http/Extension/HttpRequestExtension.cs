using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Nat.Core.Http.Extension
{
    public class File
    {
        public File(string name, byte[] content)
        {
            Name = name;
            Content = content;
        }
        public string Name { get; set; }
        public byte[] Content { get; set; }
        public string Extension { 
            get
            {
                return Path.GetExtension(Name);
            } 
        }
    }
    public static class HttpRequestExtension
    {
        /// <summary>
        /// Read list of files as byte[] from HttpRequestMessage
        /// </summary>
        /// <param name="req"></param>
        /// <returns>List of byte[]</returns>
        public static async Task<List<File>> ReadFilesAsync(this HttpRequestMessage req)
        {
            if (req.Content.IsMimeMultipartContent())
            {
                var filesList = new List<File>();
                var provider = new MultipartMemoryStreamProvider();
                await req.Content.ReadAsMultipartAsync(provider);
                foreach (HttpContent ctnt in provider.Contents)
                {
                    //now read individual part into STREAM
                    string filename = ctnt.Headers.ContentDisposition.FileName.Replace("\"", string.Empty);
                    var stream = await ctnt.ReadAsStreamAsync();
                    stream.Seek(0, SeekOrigin.Begin);
                    byte[] buf = new byte[stream.Length];
                    stream.Read(buf, 0, buf.Length);
                    filesList.Add(new File(filename, buf));
                }
                return filesList;
            }
            else
            {
                throw new System.Exception("Content type is not multipart/form-data");
            }
        }

        /// <summary>
        /// Read single file as byte[] from HttpRequestMessage
        /// </summary>
        /// <param name="req"></param>
        /// <returns>byte[]</returns>
        public static async Task<File> ReadFileAsync(this HttpRequestMessage req)
        {
            var files = await req.ReadFilesAsync();
            return files.Count > 0 ? files[0] : null;
        }
    }
}
