using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;

namespace FourZug.Backend.DTO
{
    public class BitboardGame
    {
        public bool playerXTurn;
        public ulong xBitboard;
        public ulong oBitboard;

        // Bit positions of the height of each col
        public byte[] colHeights;

        // Bit position of the padding of each col
        // If the colHeight index matches this, the col is full
        public byte[] colPadPoint;

        // Indicates piece placement eval of board
        public short placementEval = 0;

        public Stack<byte> moveHistory;


        public BitboardGame(ulong _xBitboard, ulong _oBitboard, bool xTurn=true)
        {
            playerXTurn = xTurn;
            xBitboard = _xBitboard;
            oBitboard = _oBitboard;

            colHeights = [0, 7, 14, 21, 28, 35, 42];
            colPadPoint = [6, 13, 20, 27, 34, 41, 48];
            moveHistory = new Stack<byte>();
        }
    }
}
