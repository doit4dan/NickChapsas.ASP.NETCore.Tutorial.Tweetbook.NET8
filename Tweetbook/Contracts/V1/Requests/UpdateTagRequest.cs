using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Tweetbook.Domain;

namespace Tweetbook.Contracts.V1.Requests
{
    public class UpdateTagRequest
    {        
        public string TagName { get; set; }        
    }
}
