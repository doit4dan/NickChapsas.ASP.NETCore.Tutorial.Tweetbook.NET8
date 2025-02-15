using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tweetbook.Domain
{
    //[Table("Post")] // How to override default table name
    public class Post
    {
        [Key] // PK annotation
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
