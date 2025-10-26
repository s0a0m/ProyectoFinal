using Dom = src.Models.Domain;
public interface ICommonDataService
{
    // Obtiene datos de referencia para dropdowns.
    Task<IEnumerable<Dom.Provincia>> GetAllProvinciasAsync();
    Task<Dom.Provincia?> GetProvinciaByIdAsync(int idProvincia);
}