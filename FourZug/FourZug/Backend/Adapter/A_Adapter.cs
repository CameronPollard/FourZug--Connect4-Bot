using FourZug.Backend.BotHeuristics;
using FourZug.Backend.BotUtility;
using FourZug.Backend.GameBot;

namespace FourZug.Backend.Adapter
{
    // Runs conversion logic to make API (2d) compatible with backend (bitboard)
    public abstract class A_Adapter
    {
        public ITreeManager? bot;
        public IBotHeuristics? botHeuristics;
        public IBotUtility? botUtility;

        public void SetupAdapter(ITreeManager treeManage, IBotHeuristics heu, IBotUtility util)
        {
            bot = treeManage;
            botHeuristics = heu;
            botUtility = util;

            // Dependency Injections
            bot.InitComponentReferences(botHeuristics, botUtility);
            botHeuristics.InitComponentReferences(botUtility);
        }

        // Methods connecting API and backend making them compatible
        public abstract int bestMove(char[,] grid, char currentTurn);
        public abstract (char[,], char) makeMove(char[,] grid, char currentTurn, int colMove);
        public abstract List<int> getValidMoves(char[,] grid);

            
        // Data adapting methods
        public abstract char[,] bitboardsToGrid(ulong xBitboard, ulong oBitboard);

        public abstract (ulong xBitboard, ulong oBitboard) gridToBitboards(char[,] grid);
    }
}

