using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    class Program
    {
        static void Main(string[] args)
        {
            /*list.ProcessMatrix(new bool[,]{
                {false, false, true,false,true,true,false},
                {true, false, false, true, false, false, true},
                {false, true, true, false, false, true, false},
                {true, false, false, true, false, false, false},
                {false, true, false, false, false, false, true},
                {false, false, false, true, true, false, true}});
            */
            Sudoku s = new Sudoku(9);
            s.SetRow(new int[]{0,0,3,9,0,0,7,6,0}, 0);
            s.SetRow(new int[]{0,4,0,0,0,6,0,0,9}, 1);
            s.SetRow(new int[]{6,0,7,0,1,0,0,0,4}, 2);
            s.SetRow(new int[]{2,0,0,6,7,0,0,9,0}, 3);
            s.SetRow(new int[]{0,0,4,3,0,5,6,0,0}, 4);
            s.SetRow(new int[]{0,1,0,0,4,9,0,0,7}, 5);
            s.SetRow(new int[]{7,0,0,0,9,0,2,0,1}, 6);
            s.SetRow(new int[]{3,0,0,2,0,0,0,4,0}, 7);
            s.SetRow(new int[]{0,2,9,0,0,8,5,0,0}, 8);

            Tuple<List<bool[]>, List<Tuple<int, int, int>>> sudokuMatrix = Sudoku.CalculateMatrix(s);

            TorodialDoubleLinkList<bool> list = new TorodialDoubleLinkList<bool>(s.Size*s.Size*4);
            list.ProcessMatrix(sudokuMatrix.Item1);

            List<Node<bool>> results = DancingLinks(list);

            foreach (Node<bool> result in results)
            {
                Tuple<int,int,int> rcv = sudokuMatrix.Item2[result.Index];

                Console.WriteLine("Suduko Row " + rcv.Item1 + ", Column " + rcv.Item2 + ", Value " + rcv.Item3);
                s.Values[rcv.Item1, rcv.Item2] = rcv.Item3;
            }

            Console.WriteLine();

            int spacing = (int)Math.Sqrt(s.Size);

            for (int row = 0; row < s.Size; row++)
            {
                for (int col = 0; col < s.Size; col++)
                {
                    Console.Write(s.Values[row, col] + " ");
                    if (col % spacing == spacing - 1 && col + 1 != s.Size) Console.Write("| ");
                }
                Console.WriteLine();
                if (row % spacing == spacing - 1 && row + 1 != s.Size) Console.WriteLine(new string('-', (s.Size + spacing - 1)*2));
            }

            Console.ReadLine();
        }

        static List<Node<bool>> DancingLinks(TorodialDoubleLinkList<bool> list)
        {
            List<Node<bool>> solutions = new List<Node<bool>>();
            ColumnNode<bool> column = list.H;

            return Search(list, column, solutions);
        }

        static List<Node<bool>> Search(TorodialDoubleLinkList<bool> list, ColumnNode<bool> column, List<Node<bool>> solutions)
        {
            if (list.H.Right == list.H)
            {
                foreach (Node<bool> result in solutions)
                {
                    Console.Write(result.ColumnNode.ID + "," + result.Index + " ");
                }
                Console.WriteLine();
                return solutions;
            }
            else
            {
                column = getNextColumn(list);
                Cover(column);

                Node<bool> rowNode = column;

                while (rowNode.Down != column)
                {
                    rowNode = rowNode.Down;

                    solutions.Add(rowNode);

                    Node<bool> rightNode = rowNode;

                    while (rightNode.Right != rowNode)
                    {
                        rightNode = rightNode.Right;

                        Cover(rightNode);
                    }

                    List<Node<bool>> result = Search(list, column, solutions);

                    if (result != null) return result;

                    solutions.Remove(rowNode);
                    column = rowNode.ColumnNode;

                    Node<bool> leftNode = rowNode;

                    while (leftNode.Left != rowNode)
                    {
                        leftNode = leftNode.Left;

                        Uncover(leftNode);
                    }
                }

                Uncover(column);
            }

            return null;
        }

        static ColumnNode<bool> getNextColumn(TorodialDoubleLinkList<bool> list)
        {
            ColumnNode<bool> node = list.H;
            ColumnNode<bool> chosenNode = null;

            while (node.Right != list.H)
            {
                node = (ColumnNode<bool>)node.Right;

                if (chosenNode == null || node.Size < chosenNode.Size) chosenNode = node;
            }

            return chosenNode;
        }

        static void Cover(Node<bool> node)
        {
            ColumnNode<bool> column = node.ColumnNode;
            
            column.RemoveHorizontal();

            Node<bool> verticalNode = column;

            while (verticalNode.Down != column)
            {
                verticalNode = verticalNode.Down;

                Node<bool> removeNode = verticalNode;

                while (removeNode.Right != verticalNode)
                {
                    removeNode = removeNode.Right;

                    removeNode.RemoveVertical();
                }
            }
        }

        static void Uncover(Node<bool> node)
        {
            ColumnNode<bool> column = node.ColumnNode;
            Node<bool> verticalNode = column;

            while (verticalNode.Up != column)
            {
                verticalNode = verticalNode.Up;

                Node<bool> removeNode = verticalNode;

                while (removeNode.Left != verticalNode)
                {
                    removeNode = removeNode.Left;

                    removeNode.ReplaceVertical();
                }
            }

            column.ReplaceHorizontal();
        }
    }
}
