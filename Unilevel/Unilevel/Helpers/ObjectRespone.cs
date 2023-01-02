namespace Unilevel.Helpers
{
    public class ObjectRespone
    {
        public bool Success { get; set; }
        public string Message { get; set; }

        public ObjectRespone (bool success, string message)
        {
            Success = success;
            Message = message;
        }
    }
}
