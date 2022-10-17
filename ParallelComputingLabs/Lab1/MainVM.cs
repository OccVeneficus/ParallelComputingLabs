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
    private long _oneTimeSpan;
    private long _twoTimeSpan;
    private long _fourTimeSpan;
    private long _sixteenTimeSpan;

    private string _firstImageName = "Resources\\image1_512.jpg";
    private string _firstImageSmallName = "Resources\\image1_128.jpg";
    private string _firstImageVerySmallName = "Resources\\image1_64.jpg";
    private string _firstImageMediumName = "Resources\\image1_256.jpg";
    private string _secondImageLarge = "Resources\\image2_4096.jpg";
    private string _secondImageSmall = "Resources\\image2_1024.jpg";
    private string CurrentDir => Directory.GetCurrentDirectory();

    public IAsyncRelayCommand CalculateOne { get; set; }

    public IAsyncRelayCommand CalculateTwo { get; set; }

    public IAsyncRelayCommand CalculateFour { get; set; }

    public IAsyncRelayCommand CalculateSixteen { get; set; }

    public long OneTimeSpan
    {
        get => _oneTimeSpan;
        set => SetProperty(ref _oneTimeSpan, value);
    }

    public long TwoTimeSpan
    {
        get => _twoTimeSpan;
        set => SetProperty(ref _twoTimeSpan, value);
    }

    public long FourTimeSpan
    {
        get => _fourTimeSpan;
        set => SetProperty(ref _fourTimeSpan, value);
    }

    public long SixteenTimeSpan
    {
        get => _sixteenTimeSpan;
        set => SetProperty(ref _sixteenTimeSpan, value);
    }

    private MyMatrix M1 { get; }

    private MyMatrix M2 { get; }

    public MainVM()
    {
        M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));
        CalculateOne = new AsyncRelayCommand(CalculateOneInternal);
        CalculateTwo = new AsyncRelayCommand(CalculateTwoInternal);
        CalculateFour = new AsyncRelayCommand(CalculateFourInternal);
        CalculateSixteen = new AsyncRelayCommand(CalculateSixteenInternal);
    }

    private async Task CalculateInOneThread()
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var firstStartIndex = 0;
        var firstEndIndex = list.Count;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 = Calculate(firstStartIndex, firstEndIndex, list, subList);
        var b = await Task.WhenAll(part1);
        stopwatch.Stop();
        OneTimeSpan = stopwatch.ElapsedMilliseconds;
        var a = new MyMatrix(b[0], M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateInTwoThreads()
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
        var part1 = Calculate(firstStartIndex, firstEndIndex, list, subList);
        var part2 = Calculate(secondStartIndex, secondEndIndex, list, subList);
        var b = await Task.WhenAll(part1, part2);
        stopwatch.Stop();
        TwoTimeSpan = stopwatch.ElapsedMilliseconds;
        b[0].AddRange(b[1]);
        var a = new MyMatrix(b[0], M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateOneInternal()
    {
        await CalculateInOneThread();
    }


    private async Task CalculateTwoInternal()
    {
        await CalculateInTwoThreads();
    }

    private async Task CalculateFourInternal()
    {
        await CalculateInFourThreads();
    }

    private async Task CalculateSixteenInternal()
    {
        await CalculateInSixteenThreads();
    }

    private async Task CalculateInFourThreads()
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
        var part1 = Calculate(firstStartIndex, firstEndIndex, list, subList);
        var part2 = Calculate(secondStartIndex, secondEndIndex, list, subList);
        var part3 = Calculate(thirdStartIndex, thirdEndIndex, list, subList);
        var part4 = Calculate(fourthStartIndex, fourthEndIndex, list, subList);
        var b = await Task.WhenAll(part1, part2, part3, part4);
        stopwatch.Stop();
        FourTimeSpan = stopwatch.ElapsedMilliseconds;
        b[0].AddRange(b[1]);
        b[0].AddRange(b[2]);
        b[0].AddRange(b[3]);
        var a = new MyMatrix(b[0], M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateInSixteenThreads()
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var inexes = GetIndexes(list.Count, 16);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 =  Calculate(inexes[0], inexes[1], list, subList);
        var part2 =  Calculate(inexes[2], inexes[3], list, subList);
        var part3 =  Calculate(inexes[4], inexes[5], list, subList);
        var part4 =  Calculate(inexes[6], inexes[7], list, subList);
        var part5 =  Calculate(inexes[8], inexes[9], list, subList);
        var part6 =  Calculate(inexes[10], inexes[11], list, subList);
        var part7 =  Calculate(inexes[12], inexes[13], list, subList);
        var part8 =  Calculate(inexes[14], inexes[15], list, subList);
        var part9 =  Calculate(inexes[16], inexes[17], list, subList);
        var part10 = Calculate(inexes[18], inexes[19], list, subList);
        var part11 = Calculate(inexes[20], inexes[21], list, subList);
        var part12 = Calculate(inexes[22], inexes[23], list, subList);
        var part13 = Calculate(inexes[24], inexes[25], list, subList);
        var part14 = Calculate(inexes[26], inexes[27], list, subList);
        var part15 = Calculate(inexes[28], inexes[29], list, subList);
        var part16 = Calculate(inexes[30], inexes[31], list, subList);
        var b = await Task.WhenAll(part1, part2, part3, part4, part5, part6,
            part7, part8, part9, part10, part11, part12, part13, part14, part15, part16);
        stopwatch.Stop();
        SixteenTimeSpan = stopwatch.ElapsedMilliseconds;
        b[0].AddRange(b[1]);
        b[0].AddRange(b[2]);
        b[0].AddRange(b[3]);
        b[0].AddRange(b[4]);
        b[0].AddRange(b[5]);
        b[0].AddRange(b[6]);
        b[0].AddRange(b[7]);
        b[0].AddRange(b[8]);
        b[0].AddRange(b[9]);
        b[0].AddRange(b[10]);
        b[0].AddRange(b[11]);
        b[0].AddRange(b[12]);
        b[0].AddRange(b[13]);
        b[0].AddRange(b[14]);
        b[0].AddRange(b[15]);
        var a = new MyMatrix(b[0], M2.Height, M2.Height);
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

    private List<int> GetIndexes(int size, int partsCount)
    {
        var result = new List<int>();
        var subPartSize = size / partsCount;
        var rangesCount = partsCount;
        var nextIndex = 0;
        for (var i = 0; i < rangesCount; i++)
        {
            result.Add(nextIndex);
            nextIndex += subPartSize;
            result.Add(nextIndex);
        }

        return result;
    }
}