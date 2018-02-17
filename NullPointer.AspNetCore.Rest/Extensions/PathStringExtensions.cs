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
    }
}