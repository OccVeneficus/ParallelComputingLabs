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
}