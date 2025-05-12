using NAudio.Wave;
using NAudio.Lame;
using NAudio.Wave.SampleProviders;
using NWaves.FeatureExtractors;
using NWaves.FeatureExtractors.Options;
using NWaves.Signals;
using FFmpeg.AutoGen;
using System;
using System.IO;
using System.Linq;

namespace Core.Domain.Services
{
    public class PronunciationComparisonService
    {
        private const int MFCC_COEFFICIENTS = 13;
        private const float FRAME_DURATION = 0.025f;
        private const float HOP_DURATION = 0.01f;

        public float ComparePronunciations(Stream referenceWavStream, Stream userWavStream)
        {
            try
            {
                var userSignal = PreprocessSignal(LoadSignalFromWavStream(userWavStream));
                var refSignal = PreprocessSignal(LoadSignalFromWavStream(referenceWavStream));

                if (refSignal.SamplingRate != userSignal.SamplingRate)
                {
                    userSignal = ResampleSignal(userSignal, refSignal.SamplingRate);
                }

                var mfccExtractor = CreateMfccExtractor(refSignal.SamplingRate);

                var refMfcc = mfccExtractor.ComputeFrom(refSignal).Select(v => v.ToArray()).ToArray();
                var userMfcc = mfccExtractor.ComputeFrom(userSignal).Select(v => v.ToArray()).ToArray();

                var refFeatures = AddDeltaFeatures(refMfcc);
                var userFeatures = AddDeltaFeatures(userMfcc);

                var (alignedRef, alignedUser) = AlignFeatures(refFeatures, userFeatures);

                NormalizeFeatures(refFeatures);
                NormalizeFeatures(userFeatures);

                var dtw = new Dtw();
                var distance = dtw.Compute(alignedRef, alignedUser);

                return CalculateSimilarityScore(distance);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Ошибка при сравнении произношений", ex);
            }
        }
        private (double[][], double[][]) AlignFeatures(double[][] refFeatures, double[][] userFeatures)
        {
            var dtw = new Dtw();
            var alignment = dtw.ComputeWithPath(refFeatures, userFeatures);

            var alignedRef = new List<double[]>();
            var alignedUser = new List<double[]>();

            foreach (var (i, j) in alignment.Path)
            {
                alignedRef.Add(refFeatures[i]);
                alignedUser.Add(userFeatures[j]);
            }

            return (alignedRef.ToArray(), alignedUser.ToArray());
        }

        private class Dtw
        {

            public (double Distance, (int i, int j)[] Path) ComputeWithPath(double[][] seq1, double[][] seq2)
            {
                int n = seq1.Length;
                int m = seq2.Length;

                var dtwMatrix = new double[n + 1, m + 1];
                var pathMatrix = new (int, int)[n + 1, m + 1];

                // Инициализация границ матрицы
                for (int x = 1; x <= n; x++)
                    dtwMatrix[x, 0] = double.PositiveInfinity;
                for (int y = 1; y <= m; y++)
                    dtwMatrix[0, y] = double.PositiveInfinity;

                // Заполнение матрицы DTW
                for (int x = 1; x <= n; x++)
                {
                    for (int y = 1; y <= m; y++)
                    {
                        double cost = EuclideanDistance(seq1[x - 1], seq2[y - 1]);
                        double minVal = dtwMatrix[x - 1, y - 1];
                        (int, int) minPath = (x - 1, y - 1);

                        if (dtwMatrix[x - 1, y] < minVal)
                        {
                            minVal = dtwMatrix[x - 1, y];
                            minPath = (x - 1, y);
                        }
                        if (dtwMatrix[x, y - 1] < minVal)
                        {
                            minVal = dtwMatrix[x, y - 1];
                            minPath = (x, y - 1);
                        }

                        dtwMatrix[x, y] = cost + minVal;
                        pathMatrix[x, y] = minPath;
                    }
                }

                // Восстановление пути
                var path = new List<(int, int)>();
                (int a, int b) = (n, m);
                while (a > 0 && b > 0)
                {
                    path.Add((a - 1, b - 1));
                    (a, b) = pathMatrix[a, b];
                }
                path.Reverse();

                return (dtwMatrix[n, m], path.ToArray());
            }

            public double Compute(double[][] seq1, double[][] seq2)
            {
                int n = seq1.Length;
                int m = seq2.Length;

                var dtwMatrix = new double[n + 1, m + 1];

                for (int i = 1; i <= n; i++)
                    dtwMatrix[i, 0] = double.PositiveInfinity;
                for (int j = 1; j <= m; j++)
                    dtwMatrix[0, j] = double.PositiveInfinity;

                for (int i = 1; i <= n; i++)
                {
                    for (int j = 1; j <= m; j++)
                    {
                        double cost = EuclideanDistance(seq1[i - 1], seq2[j - 1]);
                        dtwMatrix[i, j] = cost + Math.Min(dtwMatrix[i - 1, j],
                                            Math.Min(dtwMatrix[i, j - 1],
                                            dtwMatrix[i - 1, j - 1]));
                    }
                }

                return dtwMatrix[n, m];
            }

            private double EuclideanDistance(double[] v1, double[] v2)
            {
                double sum = 0;
                for (int i = 0; i < v1.Length; i++)
                {
                    sum += Math.Pow(v1[i] - v2[i], 2);
                }
                return Math.Sqrt(sum);
            }
        }

        private DiscreteSignal PreprocessSignal(DiscreteSignal signal)
        {
            return NormalizeVolume(signal);
        }

        private DiscreteSignal NormalizeVolume(DiscreteSignal signal)
        {
            float max = signal.Samples.Max(Math.Abs);
            if (max <= 0) return signal;

            float ratio = 0.9f / max;
            var normalized = signal.Samples.Select(s => s * ratio).ToArray();

            return new DiscreteSignal(signal.SamplingRate, normalized);
        }

        private MfccExtractor CreateMfccExtractor(int sampleRate)
        {
            return new MfccExtractor(new MfccOptions
            {
                SamplingRate = sampleRate,
                FeatureCount = MFCC_COEFFICIENTS,
                FrameDuration = FRAME_DURATION,
                HopDuration = HOP_DURATION,
                IncludeEnergy = true,
                FilterBankSize = 24,
                FftSize = 512
            });
        }

        private double[][] AddDeltaFeatures(float[][] mfccFeatures)
        {
            // 1. Проверка входных данных
            if (mfccFeatures == null || mfccFeatures.Length == 0)
                return Array.Empty<double[]>();

            // 2. Проверка целостности внутренних массивов
            int featureCount = mfccFeatures[0]?.Length ?? 0;
            if (featureCount == 0 || mfccFeatures.Any(row => row == null || row.Length != featureCount))
                return Array.Empty<double[]>();

            int frameCount = mfccFeatures.Length;
            var featuresWithDeltas = new double[frameCount][];

            // 3. Вычисление основных коэффициентов и дельт
            for (int i = 0; i < frameCount; i++)
            {
                featuresWithDeltas[i] = new double[featureCount * 3];

                // Основные коэффициенты
                for (int j = 0; j < featureCount; j++)
                {
                    featuresWithDeltas[i][j] = mfccFeatures[i][j];
                }

                // Дельта-коэффициенты (первые производные)
                for (int j = 0; j < featureCount; j++)
                {
                    double delta = 0;

                    if (frameCount > 1)
                    {
                        if (i == 0)
                            delta = mfccFeatures[i + 1][j] - mfccFeatures[i][j];
                        else if (i == frameCount - 1)
                            delta = mfccFeatures[i][j] - mfccFeatures[i - 1][j];
                        else
                            delta = (mfccFeatures[i + 1][j] - mfccFeatures[i - 1][j]) / 2.0;
                    }

                    featuresWithDeltas[i][j + featureCount] = delta;
                }
            }

            // 4. Вычисление дельта-дельта коэффициентов (вторые производные)
            for (int i = 0; i < frameCount; i++)
            {
                for (int j = 0; j < featureCount; j++)
                {
                    double deltaDelta = 0;

                    if (frameCount > 2) // Нужно минимум 3 кадра для вычисления
                    {
                        if (i == 0)
                        {
                            // Для первого кадра используем следующий
                            deltaDelta = featuresWithDeltas[i + 1][j + featureCount]
                                       - featuresWithDeltas[i][j + featureCount];
                        }
                        else if (i == frameCount - 1)
                        {
                            // Для последнего кадра используем предыдущий
                            deltaDelta = featuresWithDeltas[i][j + featureCount]
                                       - featuresWithDeltas[i - 1][j + featureCount];
                        }
                        else
                        {
                            // Для средних кадров используем центральную разность
                            deltaDelta = (featuresWithDeltas[i + 1][j + featureCount]
                                        - featuresWithDeltas[i - 1][j + featureCount]) / 2.0;
                        }
                    }

                    featuresWithDeltas[i][j + 2 * featureCount] = deltaDelta;
                }
            }

            return featuresWithDeltas;
        }

        private void NormalizeFeatures(double[][] features)
        {
            if (features.Length == 0) return;

            int featureCount = features[0].Length;

            for (int i = 0; i < featureCount; i++)
            {
                var values = features.Select(f => f[i]).ToArray();
                double mean = values.Average();
                double stdDev = Math.Sqrt(values.Select(v => Math.Pow(v - mean, 2)).Average());

                if (stdDev > 0)
                {
                    foreach (var feature in features)
                    {
                        feature[i] = (feature[i] - mean) / stdDev;
                    }
                }
            }
        }

        private float CalculateSimilarityScore(double dtwDistance)
        {
            return (float)Math.Exp(-0.1 * dtwDistance);
        }

        private DiscreteSignal LoadSignalFromWavStream(Stream wavStream)
        {
            if (!wavStream.CanSeek)
            {
                var mem = new MemoryStream();
                wavStream.CopyTo(mem);
                mem.Position = 0;
                wavStream = mem;
            }

            wavStream.Position = 0;

            using var reader = new WaveFileReader(wavStream);
            return ReadSamples(reader);
        }

        private DiscreteSignal ReadSamples(WaveStream waveStream)
        {
            var sampleProvider = waveStream.ToSampleProvider();

            if (sampleProvider.WaveFormat.Channels > 1)
            {
                sampleProvider = new StereoToMonoSampleProvider(sampleProvider);
            }

            var samples = new List<float>();
            var buffer = new float[waveStream.WaveFormat.SampleRate * 5];
            int samplesRead;

            while ((samplesRead = sampleProvider.Read(buffer, 0, buffer.Length)) > 0)
            {
                samples.AddRange(buffer.Take(samplesRead));
            }

            return new DiscreteSignal(waveStream.WaveFormat.SampleRate, samples.ToArray());
        }

        private DiscreteSignal ResampleSignal(DiscreteSignal signal, int targetSampleRate)
        {
            if (signal.SamplingRate == targetSampleRate)
                return signal;

            var resampler = new NWaves.Operations.Resampler();
            return resampler.Resample(signal, targetSampleRate);
        }
    }

}
