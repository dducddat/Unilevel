namespace Unilevel.Helpers
{
    public class APIRespone
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public APIRespone (bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
