using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class AreaRepository : IAreaRepository
    {
        private readonly UnilevelContext _context;
        private readonly IMapper _mapper;

        public AreaRepository(UnilevelContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task AddAreaAsync(AddOrEditArea area)
        {
            Area areaNew = new Area()
            {
                AreaCode = "COD" + DateTime.Now.ToString("ddMMyyhhmmss"),
                Name = area.AreaName
            };
            var ar = _context.Areas.SingleOrDefault(a => a.AreaCode == areaNew.AreaCode);
            if (ar == null)
            {
                _context.Add(areaNew);
                await _context.SaveChangesAsync();
            } else { throw new Exception("Area code already exist, please wait a second and recreate"); }; 
        }

        public async Task DeleteAreaAsync(string areaCode)
        {
            var area = _context.Areas.Find(areaCode);
            if (area == null)
            {
                throw new Exception("area not exist");
            }
            var users = await _context.Users.Where(u => u.AreaCode == areaCode).ToListAsync();
            var distributors = await _context.Distributors.Where(d => d.AreaCore == areaCode).ToListAsync();
            if (users != null)
            {
                foreach (var user in users)
                {
                    user.AreaCode = null;
                    _context.Users.Update(user);
                    await _context.SaveChangesAsync();
                }
            }
            if (distributors != null)
            {
                foreach (var dis in distributors)
                {
                    dis.AreaCore = null;
                    _context.Distributors.Update(dis);
                    await _context.SaveChangesAsync();
                }
            }

            _context.Remove(area);
            await _context.SaveChangesAsync();
        }

        public async Task EditAreaAsync(AddOrEditArea area, string areaCode)
        {
            var areaData = _context.Areas.Find(areaCode);
            if (areaData == null) { throw new Exception("area not exist"); }
            areaData.Name = area.AreaName; 
            _context.Update(areaData);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AreaInfor>> GetAllAreaAsync()
        {
            var areas = await _context.Areas.ToListAsync();
            return _mapper.Map<List<AreaInfor>>(areas);
        }

        public async Task<AreaDetail> GetAreaAsync(string areaCode)
        {
            var area = await _context.Areas.FindAsync(areaCode);

            if (area == null) { throw new Exception("area not exist"); }

            var distributors = await _context.Distributors.Where(d => d.AreaCore == areaCode).ToListAsync();
            List<ViewDis> disLst = new List<ViewDis>();
            foreach (var distributor in distributors)
            {
                disLst.Add(new ViewDis { 
                    Id = distributor.Id,
                    Name = distributor.Name,
                    Address = distributor.Address,
                    Email= distributor.Email,
                    PhoneNumber = distributor.PhoneNumber,
                });
            }

            var users = await _context.Users.Where(u => u.AreaCode == areaCode).Include(u => u.Role).ToListAsync();
            List<UserInfo> userLst = new List<UserInfo>();
            foreach (var user in users)
            {
                userLst.Add(new UserInfo
                {
                    Id = user.Id,
                    Email = user.Email,
                    FullName = user.FullName,
                    RoleName = user.Role.Name
                });
            }

            return new AreaDetail() { AreaCode = area.AreaCode, Name = area.Name, Distributors = disLst, Users = userLst };
        }
    }
}
