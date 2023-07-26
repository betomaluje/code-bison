namespace Posting
{
    public class PostStatus
    {
        public bool Success;
        public string Message;

        public PostStatus(bool success)
        {
            Success = success;
        }
    }
}