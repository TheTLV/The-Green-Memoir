using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Command để cuốc đất
    /// </summary>
    public class PlowTileCommand : ICommand
    {
        private readonly FarmingService _farmingService;
        private readonly TilePosition _position;
        private readonly PlayerId _playerId;

        public PlowTileCommand(FarmingService farmingService, TilePosition position, PlayerId playerId)
        {
            _farmingService = farmingService;
            _position = position;
            _playerId = playerId;
        }

        public bool CanExecute()
        {
            // Có thể thêm validation ở đây
            return true;
        }

        public CommandResult Execute()
        {
            var result = _farmingService.PlowTile(_position, _playerId);

            if (result.IsSuccess)
                return CommandResult.Success();

            return CommandResult.Failed(result.ErrorMessage);
        }
    }
}

