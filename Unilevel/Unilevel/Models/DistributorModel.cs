namespace Unilevel.Models
{
    public class ViewDis : AddDis
    {
        public string Id { get; set; }
    }

    public class AddDis
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
    }
}
