using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab1;

public static class MyMantrixManager
{
    public static MyMatrix GetContrastColors(MyMatrix picture, Color sub)
    {
        for (var i = 0; i < picture.Width; i++)
        {
            for (var j = 0; j < picture.Height; j++)
            {
                picture.Values[i, j] = GetColorDifference(picture.Values[i, j], sub);
            }
        }

        return picture;
    }

    private static Color GetColorDifference(Color first, Color second)
    {
        var r = GetPositiveDifference(first.R, second.R);
        var g = GetPositiveDifference(first.G, second.G);
        var b = GetPositiveDifference(first.B, second.B);
        return Color.FromArgb(r, g, b);
    }

    private static byte GetPositiveDifference(byte value, byte sub)
    {
        var result = value - sub;
        if (result < byte.MinValue)
        {
            return byte.MinValue;
        }

        return (byte)result;
    }
}