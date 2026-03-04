using FourZug.Backend.BotUtility;
using FourZug.Backend.DTO;

namespace FourZug.Backend.BotHeuristics;

// The implemented interface of the component

internal class BitboardEvaluator : IBotHeuristics
{
    private IBotUtility? utilEngine;


    // INTERFACE CONTRACTS

    // Calls component scripts to load their references
    public void InitComponentReferences(IBotUtility utilEngine)
    {
        this.utilEngine = utilEngine;
    }

    public void refreshPlacementEval(BitboardGame gameBoard)
    {
        short placementEval = 0;
        for (byte i = 0; i < 64; i++)
        {
            bool xPiece = (gameBoard.xBitboard & (1UL << i)) != 0;
            bool oPiece = (gameBoard.oBitboard & (1UL << i)) != 0;
            if (xPiece)
            {
                placementEval += EvalPlacement(i, true);
            }
            else if (oPiece)
            {
                placementEval += EvalPlacement(i, false);
            }
        }
        gameBoard.placementEval = placementEval;
    }

    public void updateEval(BitboardGame gameBoard, bool afterNewMove)
    {
        if (this.utilEngine == null) throw new MissingFieldException();


        int lastMoveCol = gameBoard.moveHistory.Peek();
        int bitPos = gameBoard.colHeights[lastMoveCol] - 1;
        bool isXPiece = !gameBoard.playerXTurn;

        if (afterNewMove)
        {
            gameBoard.placementEval += EvalPlacement(bitPos, isXPiece);
        }
        else // Updating the eval after undoing a move
        {
            gameBoard.placementEval -= EvalPlacement(bitPos, isXPiece);
        }
    }

    public (bool endsGame, short boardEval) evaluateBoard(BitboardGame gameBoard)
    {
        if (this.utilEngine == null) throw new Exception();

        char winner = this.utilEngine.winnerFromMove(gameBoard);
        if (winner == '?') return (false, gameBoard.placementEval);

        char lastMoveBy = (gameBoard.playerXTurn ? 'O' : 'X');
        return (true, evalWinner(lastMoveBy, winner));
    }

    // Updates the piece placement using the last placed piece location
    private sbyte EvalPlacement(int bitPos, bool isXPiece)
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

        const int topPaddingAmount = 1, rowsPerCol = 6;

        int row = bitPos % (rowsPerCol + topPaddingAmount);
        int col = bitPos / (rowsPerCol + topPaddingAmount);

        sbyte pointChange = pointTable[col, row];
        if (isXPiece) return pointChange;
        else return (sbyte)(pointChange * -1);
    }


    // PRIVATE HELPER METHODS

    private short evalWinner(char lastMoveBy, char boardWinner)
    {
        const short winGain = 1000, drawGain = -500;
        if (boardWinner == 'X') return winGain;
        if (boardWinner == 'O') return -1 * winGain;

        if (boardWinner == 'D')
        {
            if (lastMoveBy == 'X') return -1 * drawGain;
            else return drawGain;
        }

        // (If nodeWinner == '?')
        return 0;
    }        
}
