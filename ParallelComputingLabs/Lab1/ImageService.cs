using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using MathNet.Numerics.LinearAlgebra;

namespace Lab1;

public static class ImageService
{
    public static Color[,] LoadBitmapAsMatrix(string path)
    {
        var bitmap = new Bitmap(path);
        var height = bitmap.Height;
        var width = bitmap.Width;
        var matrix = new Color[width, height];
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                matrix[i, j] = bitmap.GetPixel(i, j);
            }
        }

        return matrix;
    }

    public static void SaveImage(Color[,] colors, string path)
    {
        var width = colors.GetLength(0);
        var height = colors.GetLength(1);
        var bitmap = new Bitmap(width, height);
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                bitmap.SetPixel(i, j, colors[i,j]);
            }
        }

        bitmap.Save(path, ImageFormat.Jpeg);
    }
}