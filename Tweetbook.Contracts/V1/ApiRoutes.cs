namespace Tweetbook.Contracts.V1
{
    public static class ApiRoutes
    {
        public const string Root = "api";
        public const string Version = "v1";
        public const string Base = Root + "/" + Version;

        public static class Posts
        {
            public const string GetAll = Base + "/posts";
            public const string Get = Base + "/posts/{postId}"; // "/posts/{postId}:Guid"
            public const string Create = Base + "/posts";
            public const string Update = Base + "/posts/{postId}";
            public const string Delete = Base + "/posts/{postId}";
        }

        public static class Identity
        {
            public const string Login = Base + "/identity/login";        // this breaks restful principals by having verbs. This identity section should be in separate identity server. 
                                                                         // doesn't really need to be a rest api, he is doing this for simplicity in tutorial
            public const string Register = Base + "/identity/register";

            public const string Refresh = Base + "/identity/refresh";
        }

        public static class Tags
        {
            public const string GetAll = Base + "/tags";
            public const string Get = Base + "/tags/{tagId}";
            public const string Create = Base + "/tags";
            public const string Update = Base + "/tags/{tagId}";
            public const string Delete = Base + "/tags/{tagId}";
        }
    }
}
