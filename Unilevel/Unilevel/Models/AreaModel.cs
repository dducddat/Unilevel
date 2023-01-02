using System.ComponentModel.DataAnnotations;

namespace Unilevel.Models
{
    public class AreaDTO
    {
        public string AreaCode { get; set; }
        public string Name { get; set; }
    }

    public class AreaNameDTO
    {
        public string AreaName { get; set; }
    }

    public class AreaDisQtyDTO : AreaDTO
    {
        public int? DistributorQty { get; set; }
    }

    public class AreaDetailDTO : AreaDTO
    {
        public List<DistributorDTO> Distributors { get; set; }
    }
}
