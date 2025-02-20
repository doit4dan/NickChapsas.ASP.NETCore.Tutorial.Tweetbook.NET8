namespace Tweetbook.Contracts.V1.Responses
{
    public class GetPostResponse
    {
        public string PostId { get; set; }
        public string Name { get; set; }
        public string UserId { get; set; }
        public List<PostTagResponse> Tags { get; set; } = new List<PostTagResponse>();
    }
    public class PostTagResponse
    {
        public string TagId { get; set; }
        public string TagName { get; set; }
        public string PostId { get; set; }
    }
}