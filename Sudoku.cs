using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    class Sudoku
    {
        int[,] values;
        int size;

        public Sudoku(int size)
        {
            values = new int[size, size];
            this.size = size;
        }

        public void SetRow(int[] row, int index)
        {
            for (int i = 0; i < row.Length; i++)
            {
                values[index, i] = row[i];
            }
        }

        public int[,] Values
        {
            get { return values; }
        }

        public int Size
        {
            get { return size; }
        }

        public static Tuple<List<bool[]>, List<Tuple<int, int, int>>> CalculateMatrix(Sudoku s)
        {
            List<bool[]> matrix = new List<bool[]>();
            List<Tuple<int,int,int>> rcv = new List<Tuple<int,int,int>>(); //Row, Column, Value

            for (int row = 0; row < s.Size; row++)
            {
                for (int column = 0; column < s.size; column++)
                {
                    if (s.Values[row, column] == 0)
                    {
                        for (int value = 1; value <= s.Size; value++)
                        {
                            matrix.Add(new bool[s.Size * s.Size * 4]);
                            setMatrixValues(matrix[matrix.Count - 1], s.Size, row, column, value);
                            rcv.Add(new Tuple<int,int,int>(row, column, value));
                        }
                    }
                    else
                    {
                        matrix.Add(new bool[s.Size * s.Size * 4]);
                        setMatrixValues(matrix[matrix.Count - 1], s.Size, row, column, s.Values[row,column]);
                        rcv.Add(new Tuple<int,int,int>(row, column, s.Values[row, column]));
                    }
                }
            }

            return new Tuple<List<bool[]>, List<Tuple<int, int, int>>>(matrix, rcv);
        }

        static void setMatrixValues(bool[] mRow, int size, int row, int column, int value)
        {
            int positionConstraint = row * size + column;
            int rowConstraint = size * size + row * size + (value - 1);
            int columnConstraint = size * size * 2 + column * size + (value - 1);
            int regionSize = (int)Math.Sqrt(size);
            int regionNum = (int)Math.Floor((double)row / (double)regionSize) * regionSize + (int)Math.Floor((double)column / (double)regionSize);
            int regionConstraint = size * size * 3 + regionNum * size + (value - 1);

            mRow[positionConstraint] = true;
            mRow[rowConstraint] = true;
            mRow[columnConstraint] = true;
            mRow[regionConstraint] = true;
        }
    }
}
