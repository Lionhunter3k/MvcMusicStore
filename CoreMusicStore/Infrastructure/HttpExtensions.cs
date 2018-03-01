using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace CoreMusicStore.Infrastructure
{
    public static class HttpExtensions
    {
        public static string GetLocalUrl(this IUrlHelper urlHelper, string url)
        {
            if (!urlHelper.IsLocalUrl(url))
            {
                return string.Empty;
            }

            return url;
        }

        public static bool IsLocal(this HttpRequest req)
        {
            var connection = req.HttpContext.Connection;
            if (connection.RemoteIpAddress != null)
            {
                if (connection.LocalIpAddress != null)
                {
                    return connection.RemoteIpAddress.Equals(connection.LocalIpAddress);
                }
                else
                {
                    return IPAddress.IsLoopback(connection.RemoteIpAddress);
                }
            }

            // for in memory TestServer or when dealing with default connection info
            if (connection.RemoteIpAddress == null && connection.LocalIpAddress == null)
            {
                return true;
            }

            return false;
        }
        // Content-Type: multipart/form-data; boundary="----WebKitFormBoundarymx2fSWqWSd0OxQqq"
        // The spec says 70 characters is a reasonable limit.
        public static StringSegment GetBoundary(this MediaTypeHeaderValue contentType, int lengthLimit)
        {
            var boundary = HeaderUtilities.RemoveQuotes(contentType.Boundary);
            if (StringSegment.IsNullOrEmpty(boundary.Trim()))
            {
                throw new InvalidDataException("Missing content-type boundary.");
            }

            if (boundary.Length > lengthLimit)
            {
                throw new InvalidDataException(
                    $"Multipart boundary length limit {lengthLimit} exceeded.");
            }

            return boundary;
        }

        public static bool IsMultipartContentType(this string contentType)
        {
            return !string.IsNullOrEmpty(contentType)
                   && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static bool HasFormDataContentDisposition(this ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="key";
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && StringSegment.IsNullOrEmpty(contentDisposition.FileName)
                   && StringSegment.IsNullOrEmpty(contentDisposition.FileNameStar);
        }

        public static bool HasFileContentDisposition(this ContentDispositionHeaderValue contentDisposition)
        {
            // Content-Disposition: form-data; name="myfile1"; filename="Misc 002.jpg"
            return contentDisposition != null
                   && contentDisposition.DispositionType.Equals("form-data")
                   && (!StringSegment.IsNullOrEmpty(contentDisposition.FileName)
                       || !StringSegment.IsNullOrEmpty(contentDisposition.FileNameStar));
        }
    }
}
