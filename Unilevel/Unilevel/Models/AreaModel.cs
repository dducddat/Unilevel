using System.ComponentModel.DataAnnotations;

namespace Unilevel.Models
{
    public class AreaInfor
    {
        public string AreaCode { get; set; }
        public string Name { get; set; }
    }

    public class AddOrEditArea
    {
        public string AreaName { get; set; }
    }

    public class AreaDetail : AreaInfor
    {
        public List<ViewDis> Distributors { get; set; }
        public List<UserInfo> Users { get; set; }
    }
}
