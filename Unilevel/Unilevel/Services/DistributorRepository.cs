using Microsoft.EntityFrameworkCore;
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

        public async Task AddDistributorAsync(DistributorModel distributor)
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

            _context.Add(dist);
            await _context.SaveChangesAsync();
        }

        public Task<List<DistributorModel>> GetAllDistributorAsync()
        {
            throw new NotImplementedException();
        }

        public Task RemoveDistributorAsync(string distributorId)
        {
            throw new NotImplementedException();
        }
    }
}
