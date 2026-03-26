using AppPlusPlus.Application.DTOs.Commandes;
using AppPlusPlus.Domain.Entities.CommandesInternes;

namespace AppPlusPlus.Application.Services.Commandes;

public interface ICmdService
{
    /// <summary>
    /// Returns all internal orders as CmdRowDto, with aggregated quantities and
    /// auto-corrected statuses (persisted if changed).
    /// </summary>
    Task<List<CmdRowDto>> GetCmdsWithStatusUpdateAsync();

    Task<List<Cmd>> GetCmdsWithDetailsAsync();

    Task DeleteCmdAsync(int cmdId);
}
