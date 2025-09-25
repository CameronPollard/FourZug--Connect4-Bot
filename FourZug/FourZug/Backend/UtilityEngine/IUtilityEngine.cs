namespace FourZug.Backend.UtilityEngineAccess
{
    // The interface blueprint of the component

    public interface IUtilityEngine
    {
        // Makes a move onto a grid, and returns the new grid
        char[,] MakeMove(char[,] grid, char turn, int col, int availableRow);

        // Returns all valid columns provided a game board
        List<byte> GetValidMoves(char[,] grid);

        int[] GetAvailableRows(char[,] grid);
    }
}
