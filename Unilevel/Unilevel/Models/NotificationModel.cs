namespace Unilevel.Models
{
    public class AddNotification
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
    }

    public class ListNotification
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string CreateDate { get; set; }

        public string CreateByUser { get; set; }

        public bool View { get; set; }
    }

    public class NewNotification
    {
        public int Id { get; set; }
        public string AvatarUserCreated { get; set; }
        public string Title { get; set; }
        public string NameUserCreated { get; set; }
    }

    public class NotificationDetail
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string CreateDate { get; set; }
        public string AvatarUserCreated { get; set; }
        public string NameUserCreated { get; set; }
    }
}
