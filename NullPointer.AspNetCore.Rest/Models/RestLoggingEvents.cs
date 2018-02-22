namespace NullPointer.AspNetCore.Rest.Models
{
    public static class RestLoggingEvents
    {
        public const int DISABLED_METHOD_REQUEST = 1000;
        public const int GET_ALL_REQUEST = 1001;
        public const int GET_REQUEST = 1002;
        public const int ADD_REQUEST = 1003;
        public const int UPDATE_REQUEST = 1004;
        public const int DELETE_REQUEST = 1005;

        public const int SERVER_FAILURE = 4000;
    }
}