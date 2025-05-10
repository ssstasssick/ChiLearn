using NAudio.Wave;
using NWaves.FeatureExtractors;
using NWaves.FeatureExtractors.Options;
using NWaves.Signals;
using System;
using System.Linq;

namespace Core.Domain.Services
{
    public class PronunciationComparisonService
    {
        public float ComparePronunciations(string referencePath, string userPath)
        {
            // 1. Загружаем WAV-файлы
            var refSignal = LoadSignal(referencePath);
            var userSignal = LoadSignal(userPath);

            // 2. Извлекаем MFCC
            var mfccExtractor = new MfccExtractor(new MfccOptions
            {
                SamplingRate = refSignal.SamplingRate,
                FeatureCount = 13
            });

            var refMfcc = mfccExtractor.ComputeFrom(refSignal);
            var userMfcc = mfccExtractor.ComputeFrom(userSignal);

            // 3. Преобразуем MFCC в двумерные массивы для DTW (float[][] -> double[][])
            var refFeatures = refMfcc.ToArray().Select(x => x.Select(f => (double)f).ToArray()).ToArray();
            var userFeatures = userMfcc.ToArray().Select(x => x.Select(f => (double)f).ToArray()).ToArray();

            // 4. Сравниваем с помощью DTW
            var dtw = new Dtw();
            var distance = dtw.GetDistance(refFeatures, userFeatures);

            // 5. Нормализуем (чем меньше — тем лучше)
            return 1.0f / (1.0f + (float)distance); // 0...1
        }

        private DiscreteSignal LoadSignal(string path)
        {
            using var audioFile = new AudioFileReader(path);
            var samples = new float[audioFile.Length / sizeof(float)];
            int read = audioFile.Read(samples, 0, samples.Length);
            return new DiscreteSignal(audioFile.WaveFormat.SampleRate, samples.Take(read).ToArray());
        }
    }
}
