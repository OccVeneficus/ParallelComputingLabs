using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Color = System.Drawing.Color;

namespace Lab1;

public class MyMatrix
{
    public Color[,] Values { get; set; }

    public int Width => Values.GetLength(0);

    public int Height => Values.GetLength(1);

    public MyMatrix(Color[,] values)
    {
        Values = values;
    }

    public MyMatrix(List<MyMatrix> subMatrices, int subRowCount, int subColumnCount)
    {
        MergeFromSubMatrices(subMatrices, subRowCount, subColumnCount);
    }

    public MyMatrix GetSubMatrix(int rowIndex, int rowCount, int columnIndex, int columnCount)
    {
        var colors = new Color[rowCount, columnCount];
        var savedColumnIndex = columnIndex;
        for (var newRowIndex = 0; newRowIndex < rowCount; newRowIndex++)
        {
            columnIndex = savedColumnIndex;
            for (var newColumnIndex = 0; newColumnIndex < columnCount; newColumnIndex++)
            {
                colors[newRowIndex, newColumnIndex] = Values[rowIndex, columnIndex];
                columnIndex++;
            }

            rowIndex++;
        }

        return new MyMatrix(colors);
    }

    public List<MyMatrix> GetSubMatrices(int subMatricesElementsCount)
    {
        var result = new List<MyMatrix>();
        for (var i = 0; i < Width; i += subMatricesElementsCount)
        {
            for (var j = 0; j < Height; j += subMatricesElementsCount)
            {
                result.Add(GetSubMatrix(i,subMatricesElementsCount, j, subMatricesElementsCount));
            }
        }

        return result;
    }

    private void MergeFromSubMatrices(List<MyMatrix> subMatrices, int subRowCount, int subColumnCount)
    {
        var newWidth = subMatrices.Sum(subMatrix => subMatrix.Width) / subRowCount;
        var newHeight = subMatrices.Sum(subMatrix => subMatrix.Height) / subColumnCount;
        Values = new Color[newWidth, newHeight];
        var rowCounter = 0;
        var columnCounter = 0;
        var savedRow = 0;
        var savedColumn = 0;
        var a = 0;
        foreach (var subMatrix in subMatrices)
        {
            if (savedRow > 0 && savedColumn < newHeight)
            {
                rowCounter = savedRow - subMatrix.Width;
            }

            for (var i = 0; i < subMatrix.Width; i++)
            {
                if (savedColumn == newHeight)
                {
                    columnCounter = 0;
                }
                else
                {
                    columnCounter = savedColumn;
                }
                for (var j = 0; j < subMatrix.Height; j++)
                {
                    Values[rowCounter, columnCounter] = subMatrix.Values[i, j];
                    columnCounter++;
                }
                rowCounter++;
            }

            savedRow = rowCounter;
            savedColumn = columnCounter;
            if (savedRow == newWidth)
            {
                rowCounter = savedRow - subMatrix.Width;
            }
            else
            {
                rowCounter = savedRow;
            }
            a++;
        }
    }
}