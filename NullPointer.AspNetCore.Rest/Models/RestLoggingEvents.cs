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
        public const int GET_NULL_ID = 1006;
        public const int GET_NOT_FOUND = 1007;
        public const int GET_ALL_RESPONSE = 1008;
        public const int GET_RESPONSE = 1009;
        public const int ADD_RESPONSE = 1010;
        public const int UPDATE_RESPONSE = 1011;
        public const int DELETE_RESPONSE = 1012;

        public const int SERVER_FAILURE = 4000;
    }
}