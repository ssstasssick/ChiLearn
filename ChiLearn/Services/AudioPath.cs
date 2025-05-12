using Core.Domain.Abstractions.Sevices;
using Core.Domain.Entity;
using Plugin.Maui.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Domain.Services
{
    internal static class AudioPath
    {
        public static string GetAudioPath(Word selectedWord) 
        { 
            return Path.Combine("Audio", selectedWord.AudioPath) + ".wav";
        }
    }
}
