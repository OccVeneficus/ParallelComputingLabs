using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Lab1;

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

public class MainVM : ObservableObject
{
    public int D1 { get; set; }

    public int S1 { get; set; }

    public int D2 { get; set; }

    public int S2 { get; set; }

    public int G { get; set; }

    public IRelayCommand CalculateCommand { get; set; }

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

        var r = await CalculateMatrix(subMatrices[0], M2[0, 0]);
    }

    private Matrix<double> CreateMatrix(int rowCount, int columnCount)
    {
        var random = new Random();
        return Matrix<double>.Build.Dense(rowCount, columnCount,
            (_,_) => (double)random.Next(0, 255));
    }

    private async Task<Matrix<double>> CalculateMatrix(Matrix<double> matrix, double divider)
    {
        return await Task.Run(() => matrix - divider);
    }
}