using NullPointer.AspNetCore.Rest.Models;

namespace NullPointer.AspNetCore.Rest.Builders
{
    public class RestModelOptionsBuilder
    {
        public RestModelOptionsBuilder(RestAllowedOperations initialAllowedOperations)
        {
            AllowedOperations = initialAllowedOperations;
        }

        public RestModelOptionsBuilder()
            : this(RestAllowedOperations.All)
        {
        }

        public RestModelOptionsBuilder WithDisabledGetAll()
        {
            AllowedOperations &= ~RestAllowedOperations.GetAll;
            return this;
        }

        public RestModelOptionsBuilder WithDisabledGet()
        {
            AllowedOperations &= ~RestAllowedOperations.Get;
            return this;
        }

        public RestModelOptionsBuilder WithDisabledAdd()
        {
            AllowedOperations &= ~RestAllowedOperations.Add;
            return this;
        }

        public RestModelOptionsBuilder WithDisabledUpdate()
        {
            AllowedOperations &= ~RestAllowedOperations.Update;
            return this;
        }

        public RestModelOptionsBuilder WithDisabledDelete()
        {
            AllowedOperations &= ~RestAllowedOperations.Delete;
            return this;
        }

        public RestModelOptionsBuilder WithAllEnabled()
        {
            AllowedOperations = RestAllowedOperations.All;
            return this;
        }

        public RestAllowedOperations AllowedOperations { get; private set; }
    }
}