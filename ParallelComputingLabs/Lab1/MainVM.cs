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
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_firstImageSmallName}"));
        var list = M1.GetSubMatrices(M1.Width / 4);

        //int id = 0;
        //foreach (var myMatrix in list)
        //{
        //    ImageService.SaveImage(myMatrix.Values, @$"{CurrentDir}\Resources\part{id}.jpeg");
        //    id++;
        //}
        var a = new MyMatrix(list, 4, 4);
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