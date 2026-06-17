using TechMoveData.Entities;
using TechMoveRepo.Interfaces;
using TechMoveServices.Interfaces;

namespace TechMoveServices.Services
{
    public class ClientService : IClientService
    {
        private readonly IRepository<Client> _repo;

        public ClientService ( IRepository<Client> repo )
        {
            _repo = repo;
        }

        public async Task<List<Client>> GetAllAsync ()
        {
            return await _repo.GetAllAsync();
        }

        public async Task<Client?> GetByIdAsync ( int id )
        {
            return await _repo.GetByIdAsync(id);
        }
    }
}