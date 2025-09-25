namespace FourZug.Backend.DTOs
{
    // Represents a node in the parent-pointer "tree"
    public class Node
    {
        // Represents grid AFTER the {lastColMove} move
        public char[] grid;

        // Represents who makes the next move (who's turn it is)
        public char nextMoveBy;

        // Represents the last column move that made this board
        public sbyte lastColMove;

        // Represents the piece placement eval of node
        public short placementEval = 0;

        // Tracks the next row a piece falls into for each col
        public int[] availableColRows;


        // TODO: Make node singleton
        public Node(char[] grid, char currentTurn, int[] availableColRows, sbyte lastColMove=-1)
        {
            this.grid = grid;
            this.nextMoveBy = currentTurn;
            this.lastColMove = lastColMove;
            this.availableColRows = availableColRows;
        }
    }
}