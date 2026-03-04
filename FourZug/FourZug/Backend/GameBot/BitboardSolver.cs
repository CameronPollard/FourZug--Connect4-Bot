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

        private const byte maxDepth = 8;


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
            foreach (byte validCol in validMoves)
            {
                // Get child board
                this.utilityEngine.makeMove(validCol, gameBoard);
                this.heuristicsEngine.refreshPlacementEval(gameBoard);

                // Check for any current Win In 1s. Depth 1 can only return seen wins.
                var boardSummary = heuristicsEngine.evaluateBoard(gameBoard);
                if (boardSummary.endsGame) return (sbyte)validCol;

                // Start search
                short reward = Minimax(1, !isMaximizing, gameBoard);

                // Undo move, returning to the root
                this.utilityEngine.undoPrevMove(gameBoard);
                this.heuristicsEngine.refreshPlacementEval(gameBoard);

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
        private short Minimax(int currentDepth, bool isMaximizing, BitboardGame gameBoard)
        {
            if (utilityEngine == null || heuristicsEngine == null) throw new MissingFieldException();

            // Return value of node ends game or is a leaf
            var nodeSummary = this.heuristicsEngine.evaluateBoard(gameBoard);
            if (nodeSummary.endsGame || currentDepth == maxDepth) return nodeSummary.boardEval;

            short bestReward = isMaximizing ? short.MinValue : short.MaxValue;
            List<byte> childCols = this.utilityEngine.getValidMoves(gameBoard);

            foreach (byte childCol in childCols)
            {
                // Do move to board, creating child
                this.utilityEngine.makeMove(childCol, gameBoard);

                // Get best reward from deeper searches
                short reward = Minimax(currentDepth + 1, !isMaximizing, gameBoard);

                // Undo node move to recycle it, returning to parent
                this.utilityEngine.undoPrevMove(gameBoard);

                // If reward is better than already seen
                if (isMaximizing) bestReward = Math.Max(reward, bestReward);
                if (!isMaximizing) bestReward = Math.Min(reward, bestReward);
            }

            return bestReward;
        }
    }
}
