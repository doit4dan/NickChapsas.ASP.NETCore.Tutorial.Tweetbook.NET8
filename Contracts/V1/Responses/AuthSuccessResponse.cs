namespace Tweetbook.Contracts.V1.Responses
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; } // for tutorial purpose, we are returning Token immediately for simplicity. 
                                          // in real world implementation you might want to verify email for example prior to sending an access token
    }
}
