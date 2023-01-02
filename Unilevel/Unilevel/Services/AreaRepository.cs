using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class AreaRepository : IAreaRepository
    {
        public readonly UnilevelContext _context;
        public readonly IMapper _mapper;

        public AreaRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAreaAsync(AreaNameDTO name)
        {
            Area area = new Area()
            {
                AreaCode = "COD" + DateTime.Now.ToString("ddMMyyhhmmssfffffff"),
                Name = name.AreaName
            };
            var are = _context.Areas.SingleOrDefault(a => a.AreaCode == area.AreaCode);
            if (are == null)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
            } else { throw new Exception("Area code already exist"); }; 
        }

        //public async Task DeleteAreaAsync(int id)
        //{
        //    var area = _context.Areas.Find(id);
        //    if (area == null)
        //    {
        //        throw new Exception("area not exist");
        //    }
        //    var users = _context.Users.Where(u => u.AreaId == id).ToList();
        //    if (users != null)
        //    {
        //        foreach (var item in users)
        //        {
        //            item.AreaId = 9;
        //            _context.Users.Update(item);
        //            await _context.SaveChangesAsync();
        //        }
        //    }
        //    _context.Remove(area);
        //    await _context.SaveChangesAsync();
        //}

        public async Task EditAreaAsync(AreaNameDTO name, string areaCode)
        {
            var area = _context.Areas.Find(areaCode);
            if (area == null) { throw new Exception("area not exist"); }
            if (name.AreaName.Length == 0) { throw new Exception("name cannot be blank"); }
            area.Name = name.AreaName; 
            _context.Update(area);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AreaDisQtyDTO>> GetAllAreaAsync()
        {
            var areas = await _context.Areas.ToListAsync();
            List<AreaDisQtyDTO> areaDisQtyDTOs = new List<AreaDisQtyDTO>();
            foreach (var item in areas)
            {
                int distributorQty = _context.Distributors.Count(d => d.AreaCore == item.AreaCode);
                areaDisQtyDTOs.Add(new AreaDisQtyDTO()
                {
                    AreaCode = item.AreaCode,
                    Name = item.Name,
                    DistributorQty = distributorQty
                });
            }
            return areaDisQtyDTOs;
        }

        //public async Task<AreaDetailDTO> GetAreaAsync(int id)
        //{
        //    var area = await _context.Areas.FindAsync(id);

        //    if (area == null) { throw new Exception("area not exist"); }

        //    var distributors = await _context.Distributors.Where(d => d.AreaId == id).ToListAsync();

        //    var distributorLst = _mapper.Map<List<DistributorDTO>>(distributors);

        //    var users = await _context.Users.Where(u => u.AreaId == id).Include(u => u.Role).ToListAsync();

        //    var usersLst = _mapper.Map<List<UserLoginDTO>>(users);

        //    return new AreaDetailDTO() { Id = area.Id, AreaCode = area.AreaCode, Name = area.Name, Distributors = distributorLst };
        //}
    }
}
