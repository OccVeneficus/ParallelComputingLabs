using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lab1;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using System.Windows.Documents;

public class MainVM : ObservableObject
{
    private long _elapsedMs;
    private string _firstImageName = "Resources\\image1_512.jpg";
    private string _firstImageSmallName = "Resources\\image1_128.jpg";
    private string _firstImageVerySmallName = "Resources\\image1_64.jpg";
    private string _firstImageMediumName = "Resources\\image1_256.jpg";
    private string _secondImageLarge = "Resources\\image2_4096.jpg";
    private string _secondImageSmall = "Resources\\image2_1024.jpg";
    private string CurrentDir => Directory.GetCurrentDirectory();

    public IRelayCommand CalculateCommand { get; set; }

    public long ElapsedMs
    {
        get => _elapsedMs;
        set => SetProperty(ref _elapsedMs, value);
    }

    public MainVM()
    {
        CalculateCommand = new AsyncRelayCommand(CalculateAsync);
        
       
        //var result = new List<MyMatrix>();
        //for (var i = 0; i < list.Count; i++)
        //{
        //    result.Add(MyMantrixManager.GetContrastColors(list[i], subList[i].Values[0,0]));
        //}
        //var a = new MyMatrix(result, M2.Height , M2.Height );
        //ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateAll(MyMatrix M1, MyMatrix M2)
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var firstStartIndex = 0;
        var firstEndIndex = list.Count / 2;
        var secondStartIndex = list.Count / 2;
        var secondEndIndex = list.Count;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 = await Calculate(firstStartIndex, firstEndIndex, list, subList);
        var part2 = await Calculate(secondStartIndex, secondEndIndex, list, subList);
        stopwatch.Stop();
        ElapsedMs = stopwatch.ElapsedMilliseconds;
        part1.AddRange(part2);
        var a = new MyMatrix(part1, M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateAsync()
    {
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));

        await CalculateAll(M1, M2);
        //await CalculateInForThreads(M1, M2);
    }

    private async Task CalculateInForThreads(MyMatrix M1, MyMatrix M2)
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var subPartSize = list.Count / 4;
        var firstStartIndex = 0;
        var firstEndIndex = subPartSize;
        var secondStartIndex = subPartSize;
        var secondEndIndex = subPartSize * 2;
        var thirdStartIndex = subPartSize * 2;
        var thirdEndIndex = subPartSize * 3;
        var fourthStartIndex = subPartSize * 3;
        var fourthEndIndex = subPartSize * 4;

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 = await Calculate(firstStartIndex, firstEndIndex, list, subList);
        var part2 = await Calculate(secondStartIndex, secondEndIndex, list, subList);
        var part3 = await Calculate(thirdStartIndex, thirdEndIndex, list, subList);
        var part4 = await Calculate(fourthStartIndex, fourthStartIndex, list, subList);
        stopwatch.Stop();
        ElapsedMs = stopwatch.ElapsedMilliseconds;
        part1.AddRange(part2);
        part1.AddRange(part3);
        part1.AddRange(part4);
        var a = new MyMatrix(part1, M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task<List<MyMatrix>> Calculate(int startIndex, int stopIndex, List<MyMatrix> m1, List<MyMatrix> m2)
    {
        return await Task.Run(() =>
        {
            var result = new List<MyMatrix>();

            for (var i = startIndex; i < stopIndex; i++)
            {
                result.Add(MyMantrixManager.GetContrastColors(m1[i], m2[i].Values[0, 0]));
            }

            return result;
        });
    }
}