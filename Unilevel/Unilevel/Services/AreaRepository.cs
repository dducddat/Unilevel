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
                AreaCode = "COD" + DateTime.Now.ToString("ddMMyyHHmmss"),
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

        public async Task RemoveDisFromAreaAsync(string disId)
        {
            var dis = await _context.Distributors.SingleOrDefaultAsync(d => d.Id == disId);
            if (dis == null) throw new Exception("Distributor not exist");
            dis.AreaCore = null;
            _context.Update(dis);
            await _context.SaveChangesAsync();
        }

        public async Task AddDisIntoAreaAsync(string areaCode, string disId)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.AreaCode == areaCode);
            if (area == null) { throw new Exception("area not exist"); }
            var dis = await _context.Distributors.SingleOrDefaultAsync(d => d.Id == disId);
            if (dis == null) throw new Exception("Distributor not exist");
            dis.AreaCore = areaCode;
            _context.Update(dis);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ViewDis>> GetAllDisNotInAreaAsync()
        {
            var distributors = await _context.Distributors.Where(d => d.Remove == false)
                                                          .Where(d => d.AreaCore == null).ToListAsync();
            List<ViewDis> listD = new List<ViewDis>();
            foreach (var item in distributors)
            {
                listD.Add(new ViewDis
                {
                    Id = item.Id,
                    Address = item.Address,
                    Email = item.Email,
                    PhoneNumber = item.PhoneNumber,
                    Name = item.Name,
                });
            }
            return listD;
        }

        public async Task<List<UserInfo>> GetAllUsersNotInAreaAsync()
        {
            var users = await _context.Users.Where(u => u.AreaCode == null).ToListAsync();
            List<UserInfo> listUsers = new List<UserInfo>();
            foreach (var user in users)
            {
                listUsers.Add(new UserInfo
                {
                    Id = user.Id,
                    FullName = user.FullName,
                    Email = user.Email,
                });
            }
            return listUsers;
        }

        public async Task AddUserIntoAreaAsync(string areaCode, string id)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(a => a.AreaCode == areaCode);
            if (area == null) { throw new Exception("area not exist"); }
            var user = await _context.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.AreaCode = areaCode;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveUserFromAreaAsync(string id)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) { throw new Exception("user not exist"); }
            user.AreaCode = null;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
