using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Domain.Entities;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Command để trồng hạt giống
    /// </summary>
    public class PlantSeedCommand : ICommand
    {
        private readonly FarmingService _farmingService;
        private readonly TilePosition _position;
        private readonly Crop _crop;
        private readonly PlayerId _playerId;

        public PlantSeedCommand(FarmingService farmingService, TilePosition position, Crop crop, PlayerId playerId)
        {
            _farmingService = farmingService;
            _position = position;
            _crop = crop;
            _playerId = playerId;
        }

        public bool CanExecute()
        {
            return _crop != null;
        }

        public CommandResult Execute()
        {
            var result = _farmingService.PlantSeed(_position, _crop, _playerId);

            if (result.IsSuccess)
                return CommandResult.Success();

            return CommandResult.Failed(result.ErrorMessage);
        }
    }
}

