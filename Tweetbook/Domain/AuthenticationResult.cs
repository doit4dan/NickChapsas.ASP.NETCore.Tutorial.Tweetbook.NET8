﻿namespace Tweetbook.Domain
{
    public class AuthenticationResult
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }

        public string UserId { get; set; }

        public bool Success { get; set; }

        public IEnumerable<string> Errors { get; set; }        
    }
}
