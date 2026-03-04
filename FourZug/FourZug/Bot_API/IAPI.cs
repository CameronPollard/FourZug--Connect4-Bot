using FourZug.Backend.Adapter;

namespace FourZug.Bot_API
{
    public interface IAPI
    { 
        /*
         * Returns the best move given a game grid/board
         * @pre:
         *      @param - grid, represents the game board
         *      @param - turn, turn of current player
         * @post:
         *      @return - Returns column of best move
         */
        int BestMove(char[,] grid, char turn);


        /*
         * Returns the grid after making column move
         * @pre:
         *      @param - grid, represents the game board
         *      @param - turn, turn of current player
         *      @param - col, column user selected
         * @post:
         *      @modify - Makes move change to grid
         *      @return - Returns changed grid and game winner
         */
        (char[,] grid, char winner) MakeMove(char[,] grid, char turn, int col);


        /*
         * Returns valid columns based on a game grid/board
         * @pre:
         *      @param - grid, represents the gameboard
         * @post:
         *      @return - Returns an int list of valid column moves
         */
        List<int> GetValidMoves(char[,] grid);
    }
}
