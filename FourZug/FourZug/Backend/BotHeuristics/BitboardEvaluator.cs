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

    public void updateEvalAfterMove(BitboardGame gameBoard)
    {
        if (this.utilEngine == null) throw new MissingFieldException();

        byte lastMove = gameBoard.moveHistory.Peek();

        int row = gameBoard.colHeights[lastMove] - 1;
        byte relatedBit = this.utilEngine.getBitFromCoord(row, lastMove);
        gameBoard.placementEval += EvalPlacement(relatedBit, !gameBoard.playerXTurn);
    }

    public (bool endsGame, short boardEval) evaluateBoard(BitboardGame gameBoard)
    {
        if (this.utilEngine == null) throw new Exception();

        char winner = this.utilEngine.winnerFromMove(gameBoard);
        if (winner == '?') return (false, gameBoard.placementEval);

        char lastMoveBy = (gameBoard.playerXTurn ? 'O' : 'X');
        return (true, evalWinner(lastMoveBy, winner));
    }

    // Returns piece placement value gain of a slot
    private sbyte EvalPlacement(byte bitPos, bool isXPiece)
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
