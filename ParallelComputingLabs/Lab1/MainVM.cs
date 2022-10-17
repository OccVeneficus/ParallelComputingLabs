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

    public IRelayCommand CalculateOne { get; set; }

    public IRelayCommand CalculateTwo { get; set; }

    public IRelayCommand CalculateFour { get; set; }

    public IRelayCommand CalculateSixteen { get; set; }

    public long ElapsedMs
    {
        get => _elapsedMs;
        set => SetProperty(ref _elapsedMs, value);
    }

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

    public MainVM()
    {
        CalculateOne = new AsyncRelayCommand(CalculateOneInternal);
        CalculateTwo = new AsyncRelayCommand(CalculateTwoInternal);
        CalculateFour = new AsyncRelayCommand(CalculateFourInternal);
        CalculateSixteen = new AsyncRelayCommand(CalculateSixteenInternal);
    }

    private async Task CalculateInOneThread(MyMatrix M1, MyMatrix M2)
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var firstStartIndex = 0;
        var firstEndIndex = list.Count;
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 = await Calculate(firstStartIndex, firstEndIndex, list, subList);
        stopwatch.Stop();
        OneTimeSpan = stopwatch.ElapsedMilliseconds;
        var a = new MyMatrix(part1, M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateInTwoThreads(MyMatrix M1, MyMatrix M2)
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
        TwoTimeSpan = stopwatch.ElapsedMilliseconds;
        part1.AddRange(part2);
        var a = new MyMatrix(part1, M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateOneInternal()
    {
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));

        await CalculateInOneThread(M1, M2);
    }


    private async Task CalculateTwoInternal()
    {
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));

        await CalculateInTwoThreads(M1, M2);
    }

    private async Task CalculateFourInternal()
    {
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));

        await CalculateInFourThreads(M1, M2);
    }

    private async Task CalculateSixteenInternal()
    {
        var M1 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageLarge}"));
        var M2 = new MyMatrix(ImageService.LoadBitmapAsMatrix(@$"{CurrentDir}\{_secondImageSmall}"));

        await CalculateInSixteenThreads(M1, M2);
    }

    private async Task CalculateInFourThreads(MyMatrix M1, MyMatrix M2)
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
        var part4 = await Calculate(fourthStartIndex, fourthEndIndex, list, subList);
        stopwatch.Stop();
        FourTimeSpan = stopwatch.ElapsedMilliseconds;
        part1.AddRange(part2);
        part1.AddRange(part3);
        part1.AddRange(part4);
        var a = new MyMatrix(part1, M2.Height, M2.Height);
        ImageService.SaveImage(a.Values, @$"{CurrentDir}\Resources\test.jpeg");
    }

    private async Task CalculateInSixteenThreads(MyMatrix M1, MyMatrix M2)
    {
        var elementCount = M1.Height / M2.Height;
        var list = M1.GetSubMatrices(elementCount);
        var subList = M2.GetSubMatrices(1);
        var inexes = GetIndexes(list.Count, 16);

        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var part1 = await Calculate(inexes[0], inexes[1], list, subList);
        var part2 = await Calculate(inexes[2], inexes[3], list, subList);
        var part3 = await Calculate(inexes[4], inexes[5], list, subList);
        var part4 = await Calculate(inexes[6], inexes[7], list, subList);
        var part5 = await Calculate(inexes[8], inexes[9], list, subList);
        var part6 = await Calculate(inexes[10], inexes[11], list, subList);
        var part7 = await Calculate(inexes[12], inexes[13], list, subList);
        var part8 = await Calculate(inexes[14], inexes[15], list, subList);
        var part9 = await Calculate(inexes[16], inexes[17], list, subList);
        var part10 = await Calculate(inexes[18], inexes[19], list, subList);
        var part11 = await Calculate(inexes[20], inexes[21], list, subList);
        var part12 = await Calculate(inexes[22], inexes[23], list, subList);
        var part13 = await Calculate(inexes[24], inexes[25], list, subList);
        var part14 = await Calculate(inexes[26], inexes[27], list, subList);
        var part15 = await Calculate(inexes[28], inexes[29], list, subList);
        var part16 = await Calculate(inexes[30], inexes[31], list, subList);
        stopwatch.Stop();
        SixteenTimeSpan = stopwatch.ElapsedMilliseconds;
        part1.AddRange(part2);
        part1.AddRange(part3);
        part1.AddRange(part4);
        part1.AddRange(part5);
        part1.AddRange(part6);
        part1.AddRange(part7);
        part1.AddRange(part8);
        part1.AddRange(part9);
        part1.AddRange(part10);
        part1.AddRange(part11);
        part1.AddRange(part12);
        part1.AddRange(part13);
        part1.AddRange(part14);
        part1.AddRange(part15);
        part1.AddRange(part16);
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