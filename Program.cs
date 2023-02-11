using System;
using System.IO;
using NAudio.Wave;

namespace FSK
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string inputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "keys.txt");
            string[] keys = File.ReadAllLines(inputFilePath);
            int keysLength = keys.Length;
            Console.WriteLine($"{keysLength} 256-bit keys successfully loaded");
            
            Console.Write("Enter the string to be modulated: ");
            string input = Console.ReadLine();

            int keyNum = new Random().Next(0, keysLength);
            string SelectedKey = keys[keyNum];

            Console.WriteLine($"Selected key #{keyNum}");

            byte[] encryptedBytes = AES.Encrypt(input, SelectedKey);
            string encryptedInput = Convert.ToBase64String(encryptedBytes);

            double carrierFrequency = 144;
            carrierFrequency *= 1000000; // MHz to Hz

            int sampleRate = 44100;
            double duration = 0.01; // Duration of each bit

            byte[] data = System.Text.Encoding.UTF8.GetBytes(encryptedInput.Insert(0, keyNum.ToString())); //inserts the key number in front of the encrypted string
            string outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "modulated_signal.wav");

            using (WaveFileWriter writer = new WaveFileWriter(outputFilePath, new WaveFormat(sampleRate, 16, 1)))
            {
                writer.WriteSamples(BFSK(data, sampleRate, duration, carrierFrequency), 0, data.Length * 8 * (int)(sampleRate * duration));
            }

            Console.WriteLine("Modulated signal written to modulated_signal.wav");
        }

        static float[] BFSK(byte[] data, int sampleRate, double duration, double carrierFrequency)
        {
            int samplesPerBit = (int)(sampleRate * duration);
            int numberOfSamples = samplesPerBit * data.Length * 8;
            float[] samples = new float[numberOfSamples];

            int sampleIndex = 0;
            foreach (byte b in data)
            {
                for (int i = 0; i < 8; i++)
                {
                    int bit = (b >> i) & 1;
                    float[] bitSamples = new float[samplesPerBit];
                    for (int j = 0; j < samplesPerBit; j++)
                    {
                        bitSamples[j] = (float)Math.Sin(2 * Math.PI * carrierFrequency * (bit == 0 ? 1 : 2) * j / sampleRate);
                    }
                    Array.Copy(bitSamples, 0, samples, sampleIndex, samplesPerBit);
                    sampleIndex += samplesPerBit;
                }
            }
            return samples;
        }
    }
}
