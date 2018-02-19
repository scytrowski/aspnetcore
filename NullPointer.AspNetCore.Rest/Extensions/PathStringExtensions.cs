using System;
using Microsoft.AspNetCore.Http;

namespace NullPointer.AspNetCore.Rest.Extensions
{
    public static class PathStringExtensions
    {
        public static PathString SafeAdd(this PathString path, string newSegmentString)
        {
            if (!newSegmentString.StartsWith("/"))
                newSegmentString = $"/{newSegmentString}";
            
            return path.Add(newSegmentString);
        }

        public static string[] GetSegments(this PathString path)
        {
            if (!path.HasValue)
                return Array.Empty<string>();

            string pathStringRepr = path.Value;
            pathStringRepr = pathStringRepr.Trim('/');
            return pathStringRepr.Split('/', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}