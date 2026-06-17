using TechMoveData.Entities;

namespace TechMoveServices.Interfaces
{
    public interface IClientService
    {
        Task<List<Client>> GetAllAsync ();
        Task<Client?> GetByIdAsync ( int id );
    }
}