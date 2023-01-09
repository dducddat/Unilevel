﻿using Microsoft.EntityFrameworkCore;
using System.Data;
using Unilevel.Data;
using Unilevel.Models;

namespace Unilevel.Services
{
    public class DistributorRepository : IDistributorRepository
    {
        public readonly UnilevelContext _context;

        public DistributorRepository(UnilevelContext context)
        {
            _context = context;
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

        public async Task AddDistributorAsync(AddDis distributor)
        {
            var distEmail = _context.Distributors.FirstOrDefault(d => d.Email == distributor.Email);
            if (distEmail != null) { throw new DuplicateNameException("email already exist"); }
            string id = DateTime.UtcNow.ToString("ddMMyyhhmmss");
            var distId = _context.Distributors.FirstOrDefault(d => d.Id == id);
            if (distId != null) { throw new Exception("distributor id already exist, please wait a second and recreate"); }

            Distributor dist = new Distributor();
            dist.Id = id;
            dist.Address = distributor.Address;
            dist.Name = distributor.Name;
            dist.Email = distributor.Email;
            dist.PhoneNumber = distributor.PhoneNumber;
            dist.Remove = false;

            _context.Add(dist);
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

        public async Task<List<ViewDis>> GetAllDistributorAsync()
        {
            var distributors = await _context.Distributors.Where(d => d.Remove == false).ToListAsync();
            List<ViewDis> listD = new List<ViewDis>();
            foreach(var item in distributors)
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

        public async Task RemoveDisFromAreaAsync(string disId)
        {
            var dis = await _context.Distributors.SingleOrDefaultAsync(d => d.Id == disId);
            if (dis == null) throw new Exception("Distributor not exist");
            dis.AreaCore = null;
            _context.Update(dis);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveDistributorAsync(string distributorId)
        {
            var dis = _context.Distributors.FirstOrDefault(d => d.Id == distributorId);
            if (dis == null) throw new Exception("Distributor not exist");
            dis.Remove = true;
            _context.Update(dis);
            await _context.SaveChangesAsync();
        }
    }
}
