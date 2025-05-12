using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using NWaves.FeatureExtractors.Options;
using NWaves.FeatureExtractors;
using NWaves.Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    public class Dtw
    {
        private const int MFCC_COEFFICIENTS = 13;
        private const float FRAME_DURATION = 0.025f;
        private const float HOP_DURATION = 0.01f;

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
            int frameCount = mfccFeatures.Length;
            if (frameCount == 0) return Array.Empty<double[]>();

            int featureCount = mfccFeatures[0].Length;
            var featuresWithDeltas = new double[frameCount][];

            for (int i = 0; i < frameCount; i++)
            {
                featuresWithDeltas[i] = new double[featureCount * 3];

                // Основные коэффициенты
                for (int j = 0; j < featureCount; j++)
                {
                    featuresWithDeltas[i][j] = mfccFeatures[i][j];
                }

                // Дельта-коэффициенты
                for (int j = 0; j < featureCount; j++)
                {
                    double delta;
                    if (i == 0)
                    {
                        delta = mfccFeatures[i + 1][j] - mfccFeatures[i][j];
                    }
                    else if (i == frameCount - 1)
                    {
                        delta = mfccFeatures[i][j] - mfccFeatures[i - 1][j];
                    }
                    else
                    {
                        delta = (mfccFeatures[i + 1][j] - mfccFeatures[i - 1][j]) / 2.0;
                    }
                    featuresWithDeltas[i][j + featureCount] = delta;
                }

                // Дельта-дельта коэффициенты
                for (int j = 0; j < featureCount; j++)
                {
                    double deltaDelta;
                    if (i <= 1)
                    {
                        deltaDelta = featuresWithDeltas[i + 1][j + featureCount] - featuresWithDeltas[i][j + featureCount];
                    }
                    else if (i >= frameCount - 2)
                    {
                        deltaDelta = featuresWithDeltas[i][j + featureCount] - featuresWithDeltas[i - 1][j + featureCount];
                    }
                    else
                    {
                        deltaDelta = (featuresWithDeltas[i + 1][j + featureCount] - featuresWithDeltas[i - 1][j + featureCount]) / 2.0;
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
