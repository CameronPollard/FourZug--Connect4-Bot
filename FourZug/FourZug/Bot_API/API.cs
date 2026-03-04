using FourZug.Backend.DTO;
using FourZug.Backend.BackendManager;

namespace FourZug.Bot_API
{
    // Provides a Backend interface for the frontend
    public class API : IAPI
    {
        private readonly A_BackendManager backend;

        /*
         * Does all referencing between component processors and
         * interfaces of other components
         */
        public API()
        {
            // Chooses the configuration & API<->Backend adapter
            this.backend = new BackendManager();    
        }


        /*
         * Returns the best move given a game grid/board
         * @pre:
         *      @param - grid, represents the game board
         *      @param - turn, turn of current player
         * @post:
         *      @return - Returns column of best move
         */
        public int BestMove(char[,] grid, char turn)
        {
            if (this.backend.accessAdapter == null) return -1;

            return this.backend.accessAdapter.bestMove(grid, turn);
        }


        /*
         * Returns the grid after making column move
         * @pre:
         *      @param - grid, represents the game board
         *      @param - turn, turn of current player
         *      @param - col, column user selected
         * @post:
         *      @modify - Makes move change to grid
         *      @return - Returns changed grid and winner from move: (X, O, ?)
         *      
         */
        public (char[,] grid, char winner) MakeMove(char[,] grid, char turn, int col)
        {
            if (this.backend.accessAdapter == null) return (new char[1,1], 'E');
            return this.backend.accessAdapter.makeMove(grid, turn, col);
        }


        /*
         * Returns valid columns based on a game grid/board
         * @pre:
         *      @param - grid, represents the gameboard
         * @post:
         *      @return - Returns an int list of valid column moves
         */
        public List<int> GetValidMoves(char[,] grid)
        {
            if (this.backend.accessAdapter == null) return new List<int>();

            return this.backend.accessAdapter.getValidMoves(grid);
        }
    }
}
