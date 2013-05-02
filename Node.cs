using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DancingLinks
{
    class Node<T>
    {
        ColumnNode<T> columnNode;
        Node<T> left, right, up, down;
        T val;
        int index;

        public Node(int index)
        {
            this.index = index;
        }

        public void RemoveVertical()
        {
            Up.Down = Down;
            Down.Up = Up;

            ColumnNode.DecSize();
        }

        public void RemoveHorizontal()
        {
            Right.Left = Left;
            Left.Right = Right;
        }

        public void ReplaceVertical()
        {
            Up.Down = this;
            Down.Up = this;

            ColumnNode.IncSize();
        }

        public void ReplaceHorizontal()
        {
            Right.Left = this;
            Left.Right = this;
        }

        public Node<T> Left
        {
            get { return left; }
            set { left = value; }
        }

        public Node<T> Right
        {
            get { return right; }
            set { right = value; }
        }

        public Node<T> Up
        {
            get { return up; }
            set { up = value; }
        }

        public Node<T> Down
        {
            get { return down; }
            set { down = value; }
        }

        public ColumnNode<T> ColumnNode
        {
            get { return columnNode; }
            set { columnNode = value; }
        }

        public int Index
        {
            get { return index; }
        }

        public T Value
        {
            get { return val; }
            set { val = value; }
        }
    }

    class ColumnNode<T> : Node<T>
    {
        int id;
        int size = 0;

        public ColumnNode(int id) : base(-1)
        {
            this.id = id;
            Up = this;
            Down = this;
            ColumnNode = this;
        }

        internal void IncSize()
        {
            size++;
        }

        internal void DecSize()
        {
            size--;
        }

        public int ID
        {
            get { return id; }
        }

        public int Size
        {
            get { return size; }
        }
    }

    class TorodialDoubleLinkList<T>
    {
        ColumnNode<T> h = new ColumnNode<T>(-1);
        List<ColumnNode<T>> columns = new List<ColumnNode<T>>();

        public TorodialDoubleLinkList(int noColumns)
        {
            for (int i = 0; i < noColumns; i++)
            {
                columns.Add(new ColumnNode<T>(i));
            }
            
            h.Right = columns[0];
            columns[0].Left = h;
            h.Left = columns[noColumns - 1];
            columns[noColumns - 1].Right = h;

            for (int i = 0; i < noColumns-1; i++)
            {
                columns[i].Right = columns[i + 1];
                columns[i+1].Left = columns[i];
            }
        }

        public void ProcessMatrix(bool[,] matrix)
        {
            for (int y = 0; y < matrix.GetLength(0); y++)
            {
                List<KeyValuePair<int, Node<T>>> nodes = new List<KeyValuePair<int, Node<T>>>();

                for (int x = 0; x < columns.Count; x++)
                {
                    if (matrix[y, x])
                    {
                        nodes.Add(new KeyValuePair<int, Node<T>>(x, new Node<T>(y)));
                    }
                }

                ProcessMatrixRow(nodes);
            }
        }

        void ProcessMatrixRow(List<KeyValuePair<int, Node<T>>> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Value.Left = nodes[WrapIndex(i - 1, nodes.Count)].Value;
                nodes[i].Value.Right = nodes[WrapIndex(i + 1, nodes.Count)].Value;

                AddToColumn(nodes[i].Key, nodes[i].Value);
            }
        }

        public void ProcessMatrix(List<bool[]> matrix)
        {
            for (int y = 0; y < matrix.Count; y++)
            {
                List<KeyValuePair<int, Node<T>>> nodes = new List<KeyValuePair<int, Node<T>>>();

                for (int x = 0; x < columns.Count; x++)
                {
                    if (matrix[y][x])
                    {
                        nodes.Add(new KeyValuePair<int, Node<T>>(x, new Node<T>(y)));
                    }
                }

                ProcessMatrixRow(nodes);
            }
        }

        int WrapIndex(int val, int length)
        {
            if (val >= length) return val - length;
            if (val < 0) return val + length;
            
            return val;
        }

        public void AddToColumn(int index, Node<T> node)
        {
            Node<T> lowestNode = columns[index].Up;

            lowestNode.Down = node;
            node.Up = lowestNode;
            columns[index].Up = node;
            node.Down = columns[index];
            node.ColumnNode = columns[index];

            columns[index].IncSize();
        }

        public ColumnNode<T> H
        {
            get { return h; }
        }

        public List<ColumnNode<T>> Columns
        {
            get { return columns; }
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();

            List<List<byte>> table = new List<List<byte>>();
            List<int> columns = new List<int>();

            ColumnNode<T> horizontalNode = H;
            int maxHeight = 0;

            char[] letters = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            while (horizontalNode.Right != H)
            {
                horizontalNode = (ColumnNode<T>)horizontalNode.Right;

                Node<T> verticalNode = horizontalNode;

                columns.Add(horizontalNode.ID);
                table.Add(new List<byte>());

                while (verticalNode.Down != horizontalNode)
                {
                    verticalNode = verticalNode.Down;

                    insertAt(verticalNode.Index, table[table.Count - 1], 1);

                    if(verticalNode.Index >= maxHeight) maxHeight = verticalNode.Index+1;
                }
            }

            builder.Append("H ");

            for (int x = 0; x < columns.Count; x++)
            {
                builder.Append(letters[columns[x]%26] + " ");
            }

            builder.AppendLine("|");

            for (int y = 0; y < maxHeight; y++)
            {
                builder.Append("  ");

                for (int x = 0; x < columns.Count; x++)
                {
                    if (table[x].Count > y && table[x][y] > 0)
                    {
                        builder.Append("1 ");
                    }
                    else
                    {
                        builder.Append("0 ");
                    }
                }

                builder.AppendLine("|");
            }

            return builder.ToString();
        }

        void insertAt(int index, List<byte> list, byte val)
        {
            while (list.Count < index)
            {
                list.Add(0);
            }

            list.Add(val);
        }
    }
}
