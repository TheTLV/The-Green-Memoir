using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Command để tưới nước cho tile
    /// </summary>
    public class WaterTileCommand : ICommand
    {
        private readonly FarmingService _farmingService;
        private readonly TilePosition _position;
        private readonly PlayerId _playerId;

        public WaterTileCommand(FarmingService farmingService, TilePosition position, PlayerId playerId)
        {
            _farmingService = farmingService;
            _position = position;
            _playerId = playerId;
        }

        public bool CanExecute()
        {
            return _farmingService != null;
        }

        public CommandResult Execute()
        {
            var result = _farmingService.WaterTile(_position, _playerId);

            if (result.IsSuccess)
                return CommandResult.Success();

            return CommandResult.Failed(result.ErrorMessage);
        }
    }
}

