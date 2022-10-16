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

public class MainVM : ObservableObject
{
    private long _elapsedMs;
    private string _firstImageName = "Resources\\image1_512.jpg";
    private string _firstImageSmallName = "Resources\\image1_128.jpg";
    private string _firstImageVerySmallName = "Resources\\image1_64.jpg";
    private string _firstImageMediumName = "Resources\\image1_256.jpg";
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
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_firstImageName}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_firstImageVerySmallName}"));
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var result = new List<MyMatrix>();
        for (var i = 0; i < list.Count; i++)
        {
            result.Add(MyMantrixManager.GetContrastColors(list[i], subList[i].Values[0,0]));
        }
        var a = new MyMatrix(result, M2.Height , M2.Height );
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateAsync()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();


        stopwatch.Stop();
        ElapsedMs = stopwatch.ElapsedMilliseconds;
    }
}