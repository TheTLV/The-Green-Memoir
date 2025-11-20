using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Repository cho Player
    /// </summary>
    public interface IPlayerRepository
    {
        Player GetPlayer(PlayerId playerId);
        void SavePlayer(Player player);
        bool PlayerExists(PlayerId playerId);
    }
}

