using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public class Dtw
    {
        public double GetDistance(double[][] series1, double[][] series2)
        {
            int n = series1.Length;
            int m = series2.Length;

            // Создаем таблицу для хранения расстояний
            double[,] dtw = new double[n, m];

            // Инициализация таблицы
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    dtw[i, j] = double.PositiveInfinity;
                }
            }

            // Инициализация первого элемента
            dtw[0, 0] = EuclideanDistance(series1[0], series2[0]);

            // Заполняем таблицу DTW
            for (int i = 1; i < n; i++)
            {
                dtw[i, 0] = dtw[i - 1, 0] + EuclideanDistance(series1[i], series2[0]);
            }

            for (int j = 1; j < m; j++)
            {
                dtw[0, j] = dtw[0, j - 1] + EuclideanDistance(series1[0], series2[j]);
            }

            for (int i = 1; i < n; i++)
            {
                for (int j = 1; j < m; j++)
                {
                    double cost = EuclideanDistance(series1[i], series2[j]);
                    dtw[i, j] = cost + Math.Min(Math.Min(dtw[i - 1, j], dtw[i, j - 1]), dtw[i - 1, j - 1]);
                }
            }

            // Возвращаем расстояние в правом нижнем углу таблицы DTW
            return dtw[n - 1, m - 1];
        }

        // Вычисление евклидова расстояния между двумя точками
        private double EuclideanDistance(double[] x, double[] y)
        {
            double sum = 0;
            for (int i = 0; i < x.Length; i++)
            {
                sum += Math.Pow(x[i] - y[i], 2);
            }
            return Math.Sqrt(sum);
        }
    }

}
