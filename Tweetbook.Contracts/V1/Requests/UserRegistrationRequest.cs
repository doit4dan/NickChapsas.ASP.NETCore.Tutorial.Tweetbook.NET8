﻿using System.ComponentModel.DataAnnotations;

namespace Tweetbook.Contracts.V1.Requests
{
    public class UserRegistrationRequest
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }

        // keeping it simple, coule add first name, last name, etc. 
    }
}
