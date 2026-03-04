using FourZug.Backend.BotUtility;
using FourZug.Backend.DTO;

namespace FourZug.Backend.BotHeuristics
{
    // The interface blueprint of the component

    public interface IBotHeuristics
    {
        // Creates and saves interface references of component
        void InitComponentReferences(IBotUtility utilityEngine);

        void refreshPlacementEval(BitboardGame nodeBoard);

        (bool endsGame, short boardEval) evaluateBoard(BitboardGame nodeBoard);
    }
}
