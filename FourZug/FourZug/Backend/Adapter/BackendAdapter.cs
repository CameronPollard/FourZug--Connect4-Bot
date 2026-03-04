using FourZug.Backend.DTO;
using System.Linq;

namespace FourZug.Backend.Adapter
{
    public class BackendAdapter : A_Adapter
    {
        // Methods to make API and backend compatible
        public override int bestMove(char[,] grid, char currentTurn)
        {
            BitboardGame gameBoard = makeBoard(grid, currentTurn);

            if (this.bot != null) return this.bot.BestMove(gameBoard);
            return -1;
        }

        public override List<int> getValidMoves(char[,] grid)
        {
            // The current turn doesn't matter for valid moves, any turn value is ok
            BitboardGame gameBoard = makeBoard(grid);

            if (this.botUtility != null)
            {
                List<byte> valid = this.botUtility.getValidMoves(gameBoard);
                List<int> intValid = new List<int>();

                foreach (byte b in valid) { intValid.Add(b); }
                return intValid;
            }
            return new List<int>();
        }

        public override (char[,], char) makeMove(char[,] grid, char currentTurn, int colMove)
        {
            BitboardGame gameBoard = makeBoard(grid, currentTurn);
            Console.WriteLine("Board made");

            if (this.botUtility != null)
            {   
                this.botUtility.makeMove((byte)colMove, gameBoard);
                char winner = this.botUtility.winnerFromMove(gameBoard);

                char[,] gridBoard = bitboardsToGrid(gameBoard.xBitboard, gameBoard.oBitboard);
                return (gridBoard, winner);
                
            }
            return (grid, 'E');
        }

        private BitboardGame makeBoard(char[,] grid, char currentTurn='X')
        {
            var res = gridToBitboards(grid);
            bool isXTurn = (currentTurn == 'X');

            // Make sure colHeights are correct
            BitboardGame gameBoard = new BitboardGame(res.xBitboard, res.oBitboard, isXTurn);

            ulong slotsTaken = gameBoard.xBitboard | gameBoard.oBitboard;
            int bit = 0;
            for (int col = 0; col < gameBoard.colHeights.Length; col++)
            {
                bit = gameBoard.colHeights[col];

                // This gets if there is a piece at the row in col
                bool pieceInRow = (slotsTaken & (1UL << bit)) != 0;

                while (bit < gameBoard.colPadPoint[col] && pieceInRow)
                {
                    gameBoard.colHeights[col] += 1;
                    bit += 1;
                    pieceInRow = (slotsTaken & (1UL << bit)) != 0;
                }
            }
            return gameBoard;
        }

        // Returns a byte array as bitboard (array of length 7)
        public override (ulong xBitboard, ulong oBitboard) gridToBitboards(char[,] grid)
        {
            // Empty columns will be on the right to for faster connect-4 checks
            ulong xBitboard = 0; // This is 64 bits, removing need for array of byte
            ulong oBitboard = 0;

            HashSet<byte> padPoints = [6, 13, 20, 27, 34, 41, 48];
            const byte cols = 7, rows = 6;
            byte bitPos = 0;

            for (int c = 0; c < cols; c++)
            {
                for (int r = 0; r < rows; r++)
                {
                    switch (grid[c, r])
                    {
                        case 'X':
                            xBitboard |= (1UL << bitPos);
                            break;
                        case 'O':
                            oBitboard |= (1UL << bitPos);
                            break;
                    }
                    bitPos += 1;
                }
                // There's no piece to be added at the padpoint
                // This is what adds the vertical top padding
                if (padPoints.Contains(bitPos)) bitPos += 1;
            }

            return (xBitboard, oBitboard);
        }

        public override char[,] bitboardsToGrid(ulong xBitboard, ulong oBitboard)
        {
            HashSet<byte> padPoints = [6, 13, 20, 27, 34, 41, 48];
            byte bitNum = 0;
            byte row = 0, col = 0;
            char[,] gridRes = new char[7, 6];


            while (xBitboard != 0 || oBitboard != 0)
            {
                if (!padPoints.Contains(bitNum))
                {
                    // Determine who owns the piece
                    bool xBitSet = (xBitboard & 1UL) != 0;
                    bool oBitSet = (oBitboard & 1UL) != 0;

                    // Copy bitboard piece
                    if (xBitSet) gridRes[row, col] = 'X';
                    else if (oBitSet) gridRes[row, col] = 'O';
                    else gridRes[row, col] = ' ';

                    // Move to next col
                    col += 1;
                }
                // At this point the rest would be padding, so stop copying
                else
                {
                    row += 1;
                    col = 0;
                    if (bitNum == padPoints.Max()) break;
                }

                // Move to next piece 
                xBitboard = (xBitboard >> 1);
                oBitboard = (oBitboard >> 1);
                bitNum += 1;
            }

            return gridRes;
        }
    }
}

