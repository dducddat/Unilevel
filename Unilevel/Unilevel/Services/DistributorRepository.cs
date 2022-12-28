using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class DistributorRepository : IDistributorRepository
    {
        public readonly UnilevelContext _context;

        public DistributorRepository(UnilevelContext context) {
            _context = context;
        }

        public async Task<List<DistributorModel>> GetAllDistributorAsync()
        {
            var distributors = await _context.Distributors.Where(d => d.AreaId == null).ToListAsync();
            List<DistributorModel> lst = new List<DistributorModel>();
            foreach(var item in distributors)
            {
                lst.Add(new DistributorModel()
                {
                    Name = item.Name,
                    Email = item.Email,
                    Address = item.Address,
                    PhoneNumber = item.PhoneNumber,
                });
            }
            return lst; 
        }
    }
}
