using FourZug.Backend.DTO;
using FourZug.Backend.BotHeuristics;
using FourZug.Backend.BotUtility;

namespace FourZug.Backend.GameBot
{
    // The interface blueprint of the component

    public interface ITreeManager
    {
        // Calls component scripts to create their references
        void InitComponentReferences(IBotHeuristics heuEngine, IBotUtility utilEngine);

        // Starts the Bot and returns best move results
        sbyte BestMove(BitboardGame gameBoard);
    }
}
