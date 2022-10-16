using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    public int D1 { get; set; }

    public int S1 { get; set; }

    public int D2 { get; set; }

    public int S2 { get; set; }

    public int G { get; set; }

    public IRelayCommand CalculateCommand { get; set; }

    public long ElapsedMs
    {
        get => _elapsedMs;
        set => SetProperty(ref _elapsedMs, value);
    }

    public MainVM()
    {
        CalculateCommand = new AsyncRelayCommand(CalculateAsync);
        D1 = 2;
        S1 = 2;
        D2 = 1;
        S2 = 1;
    }

    private async Task CalculateAsync()
    {
        var m1RowCount = (int)Math.Pow(2, D1);
        var m1ColumnCount = (int)Math.Pow(2, S1);
        var m2RowCount = (int)Math.Pow(2, D2);
        var m2ColumnCount = (int)Math.Pow(2, S2);

        var M1 = CreateMatrix(m1RowCount, m1ColumnCount);
        var M2 = CreateMatrix(m2RowCount, m2ColumnCount);

        var subElementsRowCount = m1RowCount / m2RowCount;
        var subElementsColumntCount = m1ColumnCount / m2ColumnCount;
        var subMatrixRowCount = m2RowCount;
        var subMatrixColumnCount = m2ColumnCount;
        var subMatrices = new List<Matrix<double>>();
        for (var i = 0; i < subMatrixRowCount; i++)
        {
            for (var j = 0; j < subMatrixColumnCount; j++)
            {
                subMatrices.Add(M1.SubMatrix(i, subElementsRowCount, j, subElementsColumntCount));
            }
        }

        Stopwatch stopwatch = new Stopwatch();
        Matrix<double> result = Matrix<double>.Build.Dense(m1RowCount, m1ColumnCount);
        stopwatch.Start();
        for (var i = 0; i < subMatrixRowCount / 2; i++)
        {
            var resultSubMatrix = await CalculateMatrix(subMatrices[i], M2[i, 0]);
        }
        stopwatch.Stop();

        ElapsedMs = stopwatch.ElapsedMilliseconds;
    }

    private Matrix<double> CreateMatrix(int rowCount, int columnCount)
    {
        var random = new Random();
        return Matrix<double>.Build.Dense(rowCount, columnCount,
            (_,_) => (double)random.Next(0, 255));
    }

    private async Task<Matrix<double>> CalculateMatrix(Matrix<double> matrix, double divider)
    { 
        return await Task.Run(() =>
        {
            var enumerateIndexed = matrix.EnumerateIndexed();
            foreach (var valueTuple in enumerateIndexed)
            {
                var result = valueTuple.Item3 - divider;

                if (result < 0)
                {
                    result = 0;
                }

                if (result > 255)
                {
                    result = 255;
                }

                matrix[valueTuple.Item1, valueTuple.Item2] = result;
            }

            return matrix;
        });
    }
}