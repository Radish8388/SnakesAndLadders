using System;
using System.Collections.Generic;
//using System.Drawing;
using System.Text;
using System.Windows.Controls;

namespace SnakesAndLadders
{
    class Token
    {
        public int Square { get; set; } = 0;
        public Image Img = new Image();
        public int Row { get; set; } = 10;
        public int Col { get; set; } = 0;
        public double Size { get; set; } = 0;

        private int[] boardPosition = { 100,
                 90, 91, 92, 93, 94, 95, 96, 97, 98, 99,
                 89, 88, 87, 86, 85, 84, 83, 82, 81, 80,
                 70, 71, 72, 73, 74, 75, 76, 77, 78, 79,
                 69, 68, 67, 66, 65, 64, 63, 62, 61, 60,
                 50, 51, 52, 53, 54, 55, 56, 57, 58, 59,
                 49, 48, 47, 46, 45, 44, 43, 42, 41, 40,
                 30, 31, 32, 33, 34, 35, 36, 37, 38, 39,
                 29, 28, 27, 26, 25, 24, 23, 22, 21, 20,
                 10, 11, 12, 13, 14, 15, 16, 17, 18, 19,
                  9,  8,  7,  6,  5,  4,  3,  2,  1,  0,
                  1,  2,  3,  4,  5,  6
                 };

        public void MoveTo(int square)
        {
            if (square > 100)
                Square = 100 - (square - 100);
            else
                Square = square;
            Row = boardPosition[square] / 10;
            Col = boardPosition[square] % 10;
        }

        public int GetRow(int square)
        {
            return boardPosition[square] / 10;
        }

        public int GetCol(int square)
        {
            return boardPosition[square] % 10;
        }

        public int SlideTo(int square)
        {
            int[] slideToNumber = { 0,
                   1,  2,  3,  4,  5,  6, 30,  8,  9, 10,
                  11, 12, 13, 14, 15, 33, 17, 18, 19, 38,
                  21, 22, 23, 24,  3, 26, 27, 28, 29, 30,
                  31, 32, 33, 34, 35, 83, 37, 38, 39, 40,
                  41,  1, 43, 44, 45, 46, 47, 48, 49, 68,
                  51, 52, 53, 54, 55, 48, 57, 58, 59, 60,
                  43, 62, 81, 64, 65, 66, 67, 68, 69, 70,
                  89, 72, 73, 74, 75, 76, 77, 78, 79, 80,
                  81, 82, 83, 84, 85, 97, 87, 88, 89, 90,
                  91, 67, 93, 12, 95, 96, 97, 80, 99, 100,
                  101, 80, 103, 104, 105, 12
                  };
            return slideToNumber[square];
        }
    }
}
