using AppPlusPlus.Domain.Entities.Approvisionnement;

namespace AppPlusPlus.Application.Services.Approvisionnement;

public interface IApproService
{
    Task<List<Appro>> GetApprosByLocalisationsAsync(List<int> localisationIds);
    Task DeleteApproAsync(int approId);
}
