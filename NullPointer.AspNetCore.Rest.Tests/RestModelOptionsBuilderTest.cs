using NullPointer.AspNetCore.Rest.Builders;
using NullPointer.AspNetCore.Rest.Models;
using Xunit;

namespace NullPointer.AspNetCore.Rest.Tests
{
    public class RestModelOptionsBuilderTest
    {
        [Fact]
        public void CheckIfGetAllNotAllowedAfterDisabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithDisabledGetAll();
            Assert.NotEqual(RestAllowedOperations.GetAll, options.AllowedOperations & RestAllowedOperations.GetAll);
        }

        [Fact]
        public void CheckIfGetNotAllowedAfterDisabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithDisabledGet();
            Assert.NotEqual(RestAllowedOperations.Get, options.AllowedOperations & RestAllowedOperations.Get);
        }

        [Fact]
        public void CheckIfAddNotAllowedAfterDisabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithDisabledAdd();
            Assert.NotEqual(RestAllowedOperations.Add, options.AllowedOperations & RestAllowedOperations.Add);
        }

        [Fact]
        public void CheckIfUpdateNotAllowedAfterDisabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithDisabledUpdate();
            Assert.NotEqual(RestAllowedOperations.Update, options.AllowedOperations & RestAllowedOperations.Update);
        }

        [Fact]
        public void CheckIfDeleteNotAllowedAfterDisabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithDisabledDelete();
            Assert.NotEqual(RestAllowedOperations.Delete, options.AllowedOperations & RestAllowedOperations.Delete);
        }

        [Fact]
        public void CheckIfAllAllowedAfterEnabling()
        {
            RestModelOptionsBuilder options = new RestModelOptionsBuilder()
                .WithAllEnabled();
            Assert.Equal(RestAllowedOperations.All, options.AllowedOperations & RestAllowedOperations.All);
        }
    }
}