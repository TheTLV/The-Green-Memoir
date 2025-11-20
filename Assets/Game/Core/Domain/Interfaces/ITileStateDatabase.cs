using TheGreenMemoir.Unity.Data;

namespace TheGreenMemoir.Core.Domain.Interfaces
{
    /// <summary>
    /// Interface để truy cập TileStateSO database
    /// </summary>
    public interface ITileStateDatabase
    {
        TileStateSO GetTileState(string stateId);
        TileStateSO GetTileStateByType(TileStateSO.TileStateType stateType);
    }
}

