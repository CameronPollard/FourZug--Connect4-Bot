using Accessibility;
using System.Reflection.Metadata.Ecma335;

namespace FourZug.Backend.UtilityEngineAccess
{
    // The implemented interface of the component

    public class UtilityEngine : IUtilityEngine
    {
        // INTERFACE CONTRACTS

        // Makes a move onto a grid, and returns the new grid
        public char[,] MakeMove(char[,] grid, char turn, int col, int availableRow)
        {
            // Clones grid so value is used not reference
            grid = (char[,])grid.Clone();

            // Checks if inputted column is a valid move
            if (availableRow == -1)
            {
                throw new Exception("Invalid column move made on grid");
            }

            grid[col, availableRow] = turn;
            return grid;
        }

        // Returns all valid columns in the game
        public List<byte> GetValidMoves(char[] grid)
        {
            List<byte> validCols = new();
            int[] availableRows = GetAvailableRows(grid);

            for (byte col = 0; col < grid.GetLength(0); col++)
            {
                if (availableRows[col] != -1) validCols.Add(col);
            }
            return validCols;
        }

        // Gets the row a piece would fall in for each col
        public int[] GetAvailableRows(char[] grid)
        {
            int[] rowAvailability = new int[7];

            const int rowsPerCol = 6;
            for (int col = 0; col < 7; col++)
            {
                int colIndex = rowsPerCol * (col + 1) - 1;

                // If top of column full, no row available
                if (grid[colIndex] != ' ')
                {
                    rowAvailability[col] = -1;
                    continue;
                }

                while (grid[colIndex-1] == ' ')
                {
                    colIndex--;
                    // Reached bottom of col
                    if (colIndex % rowsPerCol == 0) break;
                }
                rowAvailability[col] = colIndex;
            }

            return rowAvailability;
        }
    }
}
