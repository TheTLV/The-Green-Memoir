using TheGreenMemoir.Core.Domain.Interfaces;
using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Unity.Data
{
    /// <summary>
    /// Adapter để GameDatabase implement ITileStateDatabase
    /// </summary>
    public class TileStateDatabaseAdapter : ITileStateDatabase
    {
        private readonly GameDatabase _database;

        public TileStateDatabaseAdapter(GameDatabase database)
        {
            _database = database;
        }

        public TileStateSO GetTileState(string stateId)
        {
            return _database?.GetTileState(stateId);
        }

        public TileStateSO GetTileStateByType(TileStateSO.TileStateType stateType)
        {
            // Tìm trong tileStates list
            if (_database?.tileStates != null)
            {
                foreach (var state in _database.tileStates)
                {
                    if (state != null && state.stateType == stateType)
                    {
                        return state;
                    }
                }
            }
            return null;
        }
    }
}

