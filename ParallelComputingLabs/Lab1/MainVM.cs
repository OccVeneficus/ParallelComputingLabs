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
    private string _firstImageName = "Resources\\image1_128.jpg";
    private string _firstImageSmallName = "Resources\\image1_512.jpg";
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
        var a = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_firstImageName}"));
        var b = a.GetSubMatrix(a.Width / 2, a.Width/2, 0, a.Height / 2);
        ImageService.SaveImage(b.Values, $@"{CurrentDir}\Resources\testsave.jpeg");

    }

    private async Task CalculateAsync()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();


        stopwatch.Stop();
        ElapsedMs = stopwatch.ElapsedMilliseconds;
    }
}