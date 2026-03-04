using FourZug.Backend.Adapter;
using FourZug.Backend.GameBot;
using FourZug.Backend.BotHeuristics;
using FourZug.Backend.BotUtility;

namespace FourZug.Backend.BackendManager
{
    // when api uses the backendManager, its using the apiToBackendAdapter
    // This class simply handles the backend config for api
    public class BackendManager : A_BackendManager
    {
        public BackendManager()
        {
            this.accessAdapter = new BackendAdapter();

            // Dependency Injections for adapter setup
            adapterBitboardConfig();
        }

        public void adapterBitboardConfig()
        {
            // Supresses warning
            if (this.accessAdapter == null) return;

            ITreeManager gameBot = new BitboardSolver();
            IBotHeuristics heu = new BitboardEvaluator();
            IBotUtility util = new BitboardUtility();

            // Do the injection on each component
            this.accessAdapter.SetupAdapter(gameBot, heu, util);
        }
    }
}