using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Command để thu hoạch cây
    /// </summary>
    public class HarvestCropCommand : ICommand
    {
        private readonly FarmingService _farmingService;
        private readonly TilePosition _position;
        private readonly PlayerId _playerId;

        public HarvestCropCommand(FarmingService farmingService, TilePosition position, PlayerId playerId)
        {
            _farmingService = farmingService;
            _position = position;
            _playerId = playerId;
        }

        public bool CanExecute()
        {
            return true;
        }

        public CommandResult Execute()
        {
            var result = _farmingService.HarvestCrop(_position, _playerId);

            if (result.IsSuccess)
                return CommandResult.Success();

            return CommandResult.Failed(result.ErrorMessage);
        }
    }
}

