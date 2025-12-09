using NAudio.Lame;
using NAudio.Wave;

namespace StoryTeller.Services.Utils
{
    public class AudioHelper
    {
        public AudioHelper() { }
        public byte[] GenerateSilence(WaveFormat format, int durationMs)
        {
            int bytesPerMillisecond = format.AverageBytesPerSecond / 1000;
            int totalBytes = bytesPerMillisecond * durationMs;
            return new byte[totalBytes]; // Zeroed array = silence
        }

        public async Task<byte[]> MergeMp3Async(List<byte[]> mp3Chunks, int silenceMs = 500)
        {
            using var outputStream = new MemoryStream();
            var buffer = new byte[4096];
            bool writerInitialized = false;
            LameMP3FileWriter? writer = null;

            try
            {
                foreach (var (chunk, index) in mp3Chunks.Select((c, i) => (c, i)))
                {
                    using var ms = new MemoryStream(chunk);
                    using var reader = new Mp3FileReader(ms);
                    using var pcmStream = WaveFormatConversionStream.CreatePcmStream(reader);

                    if (!writerInitialized || writer == null)
                    {
                        var bitrate = GetBitrateFromChunk(chunk);
                        writer = new LameMP3FileWriter(outputStream, pcmStream.WaveFormat, bitrate);
                        writerInitialized = true;
                    }

                    // Write chunk audio
                    int bytesRead;
                    while ((bytesRead = pcmStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }

                    // Add silence except after the last chunk
                    if (index < mp3Chunks.Count - 1)
                    {
                        var silenceBytes = GenerateSilence(pcmStream.WaveFormat, silenceMs);
                        writer.Write(silenceBytes, 0, silenceBytes.Length);
                    }
                }
            }
            finally
            {
                writer?.Flush();
                writer?.Dispose();
            }

            return outputStream.ToArray();
        }

        public int GetBitrateFromChunk(byte[] chunk)
        {
            using var reader = new Mp3FileReader(new MemoryStream(chunk));
            var firstFrame = reader.ReadNextFrame();
            return firstFrame?.BitRate ?? 128;
        }
    }
}
