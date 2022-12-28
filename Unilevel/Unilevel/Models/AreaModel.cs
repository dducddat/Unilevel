namespace Unilevel.Models
{
    public class AreaModel
    {
        public string? AreaCode { get; set; }
        public string? Name { get; set; }
    }

    public class AddArea
    {
        public string? AreaName { get; set; }
    }

    public class AreaList : AreaModel
    {
        public int? TotalDistributor { get; set; }
    }

    public class AreaDetail : AreaModel
    {
        public List<DistributorModel> DistributorModels { get; set; }
        public List<UserModel> UserModels { get; set; }
    }
}
