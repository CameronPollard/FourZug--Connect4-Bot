using FourZug.Backend.DTO;
using FourZug.Backend.BotHeuristics;
using FourZug.Backend.BotUtility;

namespace FourZug.Backend.GameBot
{
    // The implemented interface of the component
    public class BitboardSolver : ITreeManager
    {
        private IBotHeuristics? heuristicsEngine;
        private IBotUtility? utilityEngine;

        private const byte maxDepth = 10;


        // INTERFACE CONTRACTS

        // Call component scripts to create their references
        public void InitComponentReferences(IBotHeuristics heuEngine, IBotUtility utilEngine)
        {
            heuristicsEngine = heuEngine;
            utilityEngine = utilEngine;
        }


        // Manages the Minimax searching, returning best move for grid
        public sbyte BestMove(BitboardGame gameBoard)
        {
            if (heuristicsEngine == null || utilityEngine == null) throw new MissingFieldException();
            this.heuristicsEngine.refreshPlacementEval(gameBoard);

            // Set desired points by turn and set worst possible reward to bestReward
            bool isMaximizing = gameBoard.playerXTurn;
            short bestReward = isMaximizing ? short.MinValue : short.MaxValue;

            List<byte> validMoves = this.utilityEngine.getValidMoves(gameBoard);
            if (validMoves.Count == 0) return -1;

            sbyte bestCol = -1;
            short alpha = short.MinValue, beta = short.MaxValue;

            foreach (byte validCol in validMoves)
            {
                // Get child board
                this.utilityEngine.makeMove(validCol, gameBoard);
                this.heuristicsEngine.updateEval(gameBoard, true);

                // Start search
                short reward = Minimax(1, alpha, beta, gameBoard);

                // Undo move, returning to the root
                this.heuristicsEngine.updateEval(gameBoard, false);
                this.utilityEngine.undoPrevMove(gameBoard);

                // Don't save reward if it isn't a new PB
                if (isMaximizing && reward <= bestReward) continue;
                else if (!isMaximizing && reward >= bestReward) continue;

                bestReward = reward;
                bestCol = (sbyte)validCol;
            }

            return bestCol;
        }



        // PRIVATE HELPER METHODS

        // Runs the minimax tree searching logic
        private short Minimax(int currentDepth, short alpha, short beta, BitboardGame gameBoard)
        {
            if (utilityEngine == null || heuristicsEngine == null) throw new MissingFieldException();
            bool isMaximizing = gameBoard.playerXTurn;

            // Return value of node ends game or is a leaf
            var boardSummary = this.heuristicsEngine.evaluateBoard(gameBoard);
            if (boardSummary.endsGame || currentDepth == maxDepth)
            {
                return boardSummary.boardEval;
            }

            List<byte> childCols = this.utilityEngine.getValidMoves(gameBoard);

            foreach (byte childCol in childCols)
            {
                // Get best reward from deeper searches, do AB pruning here
                if (alpha < beta)
                {
                    // Do move to board, creating child
                    this.utilityEngine.makeMove(childCol, gameBoard);
                    this.heuristicsEngine.updateEval(gameBoard, true);

                    short reward = Minimax(currentDepth + 1, alpha, beta, gameBoard);

                    if (isMaximizing) alpha = Math.Max(alpha, reward);
                    else beta = Math.Min(beta, reward);

                    // Undo node move to recycle it, returning to parent
                    this.heuristicsEngine.updateEval(gameBoard, false);
                    this.utilityEngine.undoPrevMove(gameBoard);
                }
            }

            return (isMaximizing ? alpha : beta);
        }
    }
}
