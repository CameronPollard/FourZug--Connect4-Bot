using FourZug.Backend.DTO;

namespace FourZug.Backend.BotUtility
{
    public interface IBotUtility
    {
        void makeMove(byte colMove, BitboardGame gameBoard);

        void undoPrevMove(BitboardGame gameBoard);

        (int row, int col) getCoordFromBit(byte i);

        byte getBitFromCoord(int row, int col);

        char winnerFromMove(BitboardGame gameBoard);

        List<byte> getValidMoves(BitboardGame gameBoard);
    }
}
