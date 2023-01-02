//using Microsoft.EntityFrameworkCore;
//using Unilevel.Data;
//using Unilevel.Models;

//namespace Unilevel.Services
//{
//    public class DistributorRepository : IDistributorRepository
//    {
//        public readonly UnilevelContext _context;

//        public DistributorRepository(UnilevelContext context) {
//            _context = context;
//        }

//        public async Task<List<DistributorDTO>> GetAllDistributorAsync()
//        {
//            var distributors = await _context.Distributors.Where(d => d.AreaId == null).ToListAsync();
//            List<DistributorDTO> lst = new List<DistributorDTO>();
//            foreach(var item in distributors)
//            {
//                lst.Add(new DistributorDTO()
//                {
//                    Name = item.Name,
//                    Email = item.Email,
//                    Address = item.Address,
//                    PhoneNumber = item.PhoneNumber,
//                });
//            }
//            return lst; 
//        }
//    }
//}
