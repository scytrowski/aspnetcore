using System.Linq;
using Microsoft.AspNetCore.Http;
using NullPointer.AspNetCore.Rest.Extensions;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    public class PathStringExtensionsTest
    {
        [Fact]
        public void CheckIfDivideIntoValidSegments()
        {
            string[] expectedSegments = new string[] { "test", "divide", "into", "segments" };
            PathString testPath = expectedSegments.Aggregate(PathString.Empty, (acc, s) => acc.SafeAdd(s));
            string[] testPathSegments = testPath.GetSegments();
            Assert.True(testPathSegments.SequenceEqual(expectedSegments));
        }
    }
}