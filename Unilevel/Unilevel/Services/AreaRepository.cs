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

        public async Task AddAreaAsync(AddArea areaName)
        {
            Area area = new Area() {Name = areaName.AreaName};
            _context.Add(area);
            await _context.SaveChangesAsync();
            area.AreaCode = "COD" + area.Id;
            _context.Update(area);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AreaList>> GetAllAreaAsync()
        {
            var areas = await _context.Areas.ToListAsync();

            List<AreaList> areaModels = new List<AreaList>();
            foreach (var item in areas)
            {
                int totalDist = _context.Distributors.Count(d => d.AreaId == item.Id);
                areaModels.Add(new AreaList()
                {
                    AreaCode = item.AreaCode,
                    Name = item.Name,
                    TotalDistributor = totalDist
                });
            }
            return areaModels;
        }

        public async Task<AreaDetail> GetAreaAsync(int id)
        {
            var area = await _context.Areas.FindAsync(id);

            if (area == null) throw new Exception();

            var distributors = await _context.Distributors.Where(d => d.AreaId == id).ToListAsync();

            var distributorLst = _mapper.Map<List<DistributorModel>>(distributors);

            var users = await _context.Users.Where(u => u.AreaId == id).Include(u => u.Role).ToListAsync();

            var usersLst = _mapper.Map<List<UserModel>>(users);

            return new AreaDetail() {AreaCode = area.AreaCode, Name = area.Name, DistributorModels = distributorLst, UserModels = usersLst};
        }
    }
}
