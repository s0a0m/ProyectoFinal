using Dom = src.Models.Domain;
using src.Repositories.Interfaces;
using src.Models.Domain;
namespace src.Core.Services.Implementations;

public class CommonDataService : ICommonDataService
{
    private readonly IProvinciaRepository _provinciaRepo;

    public CommonDataService(IProvinciaRepository provinciaRepo)
    {
        _provinciaRepo = provinciaRepo;
    }

    public Task<IEnumerable<Dom.Provincia>> GetAllProvinciasAsync()
    {
        return _provinciaRepo.GetAllAsync();
    }

    public Task<Provincia?> GetProvinciaByIdAsync(int idProvincia)
    {
        return _provinciaRepo.GetByIdAsync(idProvincia);
    }
}