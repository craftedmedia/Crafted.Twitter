using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.IO;

namespace Crafted.Twitter.Helpers
{
    public class ResourceHelper
    {
        /// <summary>
        /// get the resource.
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <returns>a string containing the resource</returns>
        private static string GetResource(string filename)
        {
            filename = filename.ToLower();
            string result;

            using (var stream = typeof(ResourceHelper).Assembly.GetManifestResourceStream("Crafted.Twitter.Resources." + filename))
            using (var reader = new StreamReader(stream))
            {
                result = reader.ReadToEnd();
            }


            return result;
        }

        /// <summary>
        /// Handles rendering static content files.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="path">The path.</param>
        /// <returns>a string containing the content type.</returns>
        public static string Includes(HttpContext context, string path)
        {
            //if the context is present then set the output type
            if (context != null)
            {
                SetResponseType(context, path);
            }
            //read and return the resource
            var embeddedFile = Path.GetFileName(path);
            return GetResource(embeddedFile);
        }

        public static void SetResponseType(HttpContext context, string path)
        {
            var response = context.Response;
            switch (Path.GetExtension(path))
            {
                case ".js":
                    response.ContentType = "application/javascript";
                    break;
            }
        }
    }
}
