using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tweetbook.Domain
{
    [Table("PostTags")]
    public class PostTag
    {
        [Key]
        public Guid TagId { get; set; }

        [ForeignKey(nameof(TagId))]
        public Tag Tag { get; set; }

        public string TagName { get; set; }

        public Guid PostId { get; set; }

        [ForeignKey(nameof(PostId))]
        public Post Post { get; set; } // tag has one post
    }
}