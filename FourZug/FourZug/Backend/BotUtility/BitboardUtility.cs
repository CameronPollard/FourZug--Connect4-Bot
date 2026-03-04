using FourZug.Backend.DTO;

namespace FourZug.Backend.BotUtility
{
    internal class BitboardUtility : IBotUtility
    {
        // Makes a move on the board, by toggling the piece in a slot
        public void makeMove(byte colMove, BitboardGame gameBoard)
        {
            gameBoard.moveHistory.Push(colMove);

            bool makeXMove = gameBoard.playerXTurn;

            // Toggles the piece in the slot (making move)
            updateGameBoard(colMove, makeXMove, gameBoard);

            // Update the height index of the col
            gameBoard.colHeights[colMove] += 1;
        }

        // Undoes a move on the board, by toggling the piece in a slot
        public void undoPrevMove(BitboardGame gameBoard)
        {
            if (gameBoard.moveHistory == null) return;

            bool undoXMove = !gameBoard.playerXTurn;

            byte prevMove = gameBoard.moveHistory.Pop();

            // Update the height index of the col first, so we can remove the piece
            gameBoard.colHeights[prevMove] -= 1;

            // Toggles the piece in the slot (making move)
            updateGameBoard(prevMove, undoXMove, gameBoard);

            
        }

        public (int row, int col) getCoordFromBit(byte i)
        {
            const byte bitsPerCol = 8;
            const byte topRow = 5;
            const byte startOfFinalBuffer = 54; // (6 + 2) * 7 - 2

            int row = i % bitsPerCol;
            int col = i / bitsPerCol;

            if (row > topRow || bitsPerCol >= startOfFinalBuffer)
            {
                Console.WriteLine("Error - The entered bit was part of a bitboard buffer");
            }
            return (row, col);
        }

        public byte getBitFromCoord(int row, int col)
        {
            const byte bitsPerCol = 8;
            return (byte)(col * bitsPerCol + row);
        }

        // Returns who would win if said move was made
        // Assumes this is being used after move is made
        public char winnerFromMove(BitboardGame gameBoard)
        {
            // Get the last player
            bool moveByX = !gameBoard.playerXTurn;
            ulong bitboard = moveByX ? gameBoard.xBitboard : gameBoard.oBitboard;
            byte lastMove = gameBoard.moveHistory.Peek();
            int pieceFallLoc = gameBoard.colHeights[lastMove] - 1;

            // These have to take into account bitboard paddings in the values too
            int[] bitDirecs = { 1, 8, 7, 6, -1, -8, -7, -6 };
            int[] chainAlongGradient = { 1, 1, 1, 1 };

            for (int direcIndex = 0; direcIndex < bitDirecs.Length; direcIndex++)
            {
                for (int dist = 1; dist <= 3; dist++)
                {
                    int checkedIndex = pieceFallLoc + dist * bitDirecs[direcIndex];
                    if (checkedIndex < 0 || checkedIndex > 63) break;

                    bool pieceOwned = (bitboard & (1UL << checkedIndex)) != 0;

                    if (pieceOwned)
                    {
                        // Bunches opposite directions with same gradient together
                        int gradientIndex = direcIndex % 4;
                        chainAlongGradient[gradientIndex] += 1;

                        // End game if a connect 4 by the player found
                        if (chainAlongGradient[gradientIndex] >= 4)
                        {
                            return moveByX ? 'X' : 'O';
                        }
                    }
                    else break;
                }
            }

            // This point is reached if the player's move didn't win 
            // If theres no moves left, the game is drawn, else its still in play
            return getValidMoves(gameBoard).Count() == 0 ? 'D' : '?';
        }

        // Returns the column moves that can be made on the board
        public List<byte> getValidMoves(BitboardGame gameBoard)
        {
            List<byte> validMoves = new List<byte>();
            for (byte i = 0; i < gameBoard.colHeights.Length; i++)
            {
                if (gameBoard.colHeights[i] < gameBoard.colPadPoint[i])
                {
                    validMoves.Add(i);
                }
            }
            return validMoves;
        }

        // Toggles the piece in a slot for a player's bitboard, doing or undoing a move
        private void updateGameBoard(byte col, bool changeForPlayerX, BitboardGame gameBoard)
        {
            ulong changedBitboard = 0;

            // Get the board to change
            if (changeForPlayerX) changedBitboard = gameBoard.xBitboard;
            else changedBitboard = gameBoard.oBitboard;

            // Toggle piece in (slot on board
            changedBitboard ^= (1UL << gameBoard.colHeights[col]);

            // Update the related bitboard
            if (changeForPlayerX) gameBoard.xBitboard = changedBitboard;
            else gameBoard.oBitboard = changedBitboard;

            gameBoard.playerXTurn = !gameBoard.playerXTurn;
            
        }
    }
}
