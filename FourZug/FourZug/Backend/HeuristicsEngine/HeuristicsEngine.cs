using FourZug.Backend.DTOs;
using FourZug.Backend.UtilityEngineAccess;
using System.Reflection.Metadata.Ecma335;

namespace FourZug.Backend.HeuristicsEngineAccess
{
    // The implemented interface of the component

    internal class HeuristicsEngine : IHeuristicsEngine
    {
        private IUtilityEngine? utilEngine;


        // INTERFACE CONTRACTS

        // Calls component scripts to load their references
        public void InitComponentReferences(IUtilityEngine utilEngine)
        {
            this.utilEngine = utilEngine;
        }

        // Returns if game ends and the evaluation of a node
        public (bool endsGame, short nodeEval) NodeSummary(Node node)
        {
            // If next move is by X, then last was by O. Same for O to X
            char nodeLastMoveBy = node.nextMoveBy == 'X' ? 'O' : 'X';

            char nodeWinner = BoardWinner(node.grid, nodeLastMoveBy, node.lastColMove);

            short nodeEval = EvalNodeUsingWinner(node, nodeWinner, nodeLastMoveBy);

            if (nodeWinner != '?') return (true, nodeEval);
            else return (false, nodeEval);
        }

        // Return the game winner
        // TODO: Add in empty overflow slots to prevent false positive wins
        public char GetWinner(char[] grid, char lastMoveBy, byte lastColMove)
        {
            if (utilEngine == null) throw new MissingFieldException();


            // Get the row the last piece fell into
            const byte rowsPerCol = 6;
            int lastPieceI = rowsPerCol * (lastColMove+1) - 1;

            // Get where the piece fell into
            while (grid[lastPieceI] != ' ')
            {
                lastPieceI--;
            }

            // Determines the scale changes in index to check for connect 4s
            // In order: Vertical, Diagonal (NE / SW), Horizontal, Diagonal (SE / NW)
            int[] indexChangeScales = { 1, 7, 6, 5 };

            // Run through each direction, checking for connect 4
            foreach (int direc in indexChangeScales)
            {
                int connectedPieces = 1;
                
                // Check positive direc
                for (int dist = 1; dist <= 3; dist++)
                {
                    int checkedIndex = lastPieceI + dist * direc;

                    if (checkedIndex <= 41 && isValidIndex(lastPieceI, checkedIndex))
                    {
                        if (grid[checkedIndex] == lastMoveBy) connectedPieces++;
                        else break;
                    }
                }

                // Check opposite direc
                for (int dist = -1; dist >= -3; dist--)
                {
                    int checkedIndex = lastPieceI + dist * direc;

                    if (checkedIndex >= 0 && isValidIndex)
                    {
                        if (grid[checkedIndex] == lastMoveBy) connectedPieces++;
                        else break;
                    }
                }

                if (connectedPieces >= 4) return lastMoveBy;
            }

            // If no player has won and no move left, game is a draw
            if (utilEngine.GetValidMoves(grid).Count == 0) return 'D';

            // If no one has won and it isnt a draw, the game must still be in play
            else return '?';
        }

        // Returns placement eval of entire baord
        public short EvalPiecePlacements(char[,] grid)
        {
            short pointBalance = 0;
            for (int col = 0; col < grid.GetLength(0); col++)
            {
                for (int row = 0; row < grid.GetLength(1); row++)
                {
                    char containedPiece = grid[col, row];
                    if (containedPiece == ' ') continue;
                    pointBalance += EvalPlacement(col, row, containedPiece);
                }
            }

            return pointBalance;
        }

        // Returns piece placement value gain of a slot
        public sbyte EvalPlacement(int col, int row, char containedPiece)
        {
            // Represents the points gained from positions taken
            // Viewing from side would correlate visually to game board and help understand array access
            sbyte[,] pointTable = new sbyte[7, 6]
            {
                { 3, 4, 5, 5, 4, 3},
                { 4, 6, 8, 8, 6, 4 },
                { 5, 8, 11, 11, 8, 5 },
                { 7, 10, 13, 13, 10, 7 },
                { 5, 8, 11, 11, 8, 5 },
                { 4, 6, 8, 8, 6, 4 },
                { 3, 4, 5, 5, 4, 3}
            };

            if (containedPiece == 'X') return pointTable[col, row];
            else return (sbyte)(pointTable[col, row] * -1);

        }


        // PRIVATE HELPER METHODS

        // Checks an ID of a grid is valid in a connect 4 chain
        private bool isValidIndex(int newPieceI, int pointedI)
        {
            // Handles invalid index
            if (pointedI < 0 || pointedI > 41) return false;

            int posCol = newPieceI / 6, posRow = newPieceI % 6;
            int pointedCol = pointedI / 6, pointedRow = pointedI % 6;
            int colDist = Math.Abs(posCol - pointedCol), rowDist = Math.Abs(posRow - pointedRow);

            // Checks for diagonal 1 to 1 gradient
            if (colDist != 0 && rowDist != 0)
            {
                if (colDist != rowDist) return false;
            }

            return true;
        }

        private short EvalNodeUsingWinner(Node node, char nodeWinner, char nodeLastMoveBy)
        {
            const short winGain = 1000, drawGain = -500;
            if (nodeWinner == 'X') return winGain;
            if (nodeWinner == 'O') return -1 * winGain;

            if (nodeWinner == 'D')
            {
                if (nodeLastMoveBy == 'X') return -1 * drawGain;
                else return drawGain;
            }

            // (If nodeWinner == '?')
            else return node.placementEval;
        }        
    }
}
