using TheGreenMemoir.Core.Application.Services;
using TheGreenMemoir.Core.Domain.ValueObjects;

namespace TheGreenMemoir.Core.Application.Commands
{
    /// <summary>
    /// Command để trồng hạt giống bằng CropId (load từ database)
    /// </summary>
    public class PlantSeedByIdCommand : ICommand
    {
        private readonly FarmingService _farmingService;
        private readonly TilePosition _position;
        private readonly CropId _cropId;
        private readonly PlayerId _playerId;

        public PlantSeedByIdCommand(
            FarmingService farmingService, 
            TilePosition position, 
            CropId cropId, 
            PlayerId playerId)
        {
            _farmingService = farmingService;
            _position = position;
            _cropId = cropId;
            _playerId = playerId;
        }

        public bool CanExecute()
        {
            return _cropId != default;
        }

        public CommandResult Execute()
        {
            var result = _farmingService.PlantSeed(_position, _cropId, _playerId);

            if (result.IsSuccess)
                return CommandResult.Success();

            return CommandResult.Failed(result.ErrorMessage);
        }
    }
}

