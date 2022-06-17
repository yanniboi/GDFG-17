using Misc;
using ProcCGen;
using System;

namespace Noise
{
    class ValueNoise
    {
        int[] HashTable { get; set; }
        float[] ValueTable { get; set; }

        int HashTableSize { get; set; }
        int HashTableSizeMask { get; set; }
        int ValueTableSize { get; set; }

        Random Random { get; set; }


        public ValueNoise() : this(new Random().Next()) { }

        public ValueNoise(int seed) : this(seed, 256, 256)
        {

        }

        public ValueNoise(int seed, int hashTableSize, int valueTableSize)
        {
            this.Random = new Random(seed);

            this.HashTableSize = hashTableSize;
            this.HashTableSizeMask = this.HashTableSize - 1;
            this.ValueTableSize = valueTableSize;

            this.HashTable = new int[this.HashTableSize * 2];
            this.ValueTable = new float[this.ValueTableSize];

            for (int i = 0; i < this.ValueTableSize; i++)
            {
                this.ValueTable[i] = (float)this.Random.NextDouble();
            }

            for (int i = 0; i < this.HashTableSize; i++)
            {
                this.HashTable[i] = i;
            }

            for (int i = 0; i < this.HashTableSize; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    int idx = this.Random.Next(this.HashTableSize);
                    int atIdx = this.HashTable[idx];

                    this.HashTable[idx] = this.HashTable[i];
                    this.HashTable[i] = atIdx;
                }
            }

            for (int i = 0; i < this.HashTableSize; i++)
            {
                this.HashTable[this.HashTableSize + i] = this.HashTable[i];
            }
        }


        public float Get(NoiseConfig config, params float[] input)
        {
            float toReturn = 0;

            int layers = config.Layers;
            float amplitude = config.Amplitude;
            float frequency = config.Frequency;

            int inputLength = input.Length;
            float[] finalInput = new float[inputLength];

            for (int layer = 0; layer < layers; layer++)
            {
                for (int i = 0; i < inputLength; i++)
                {
                    finalInput[i] = (input[i] * frequency);
                }

                toReturn += this.Get(input) * amplitude;

                amplitude *= .5f;
                frequency *= 2f;
            }

            return toReturn;
        }

        public float Get(params float[] input)
        {
            int inputLength = input.Length;

            this.PrepareInput(input, inputLength, 
                out int[] intInput, out int[] hashTabelIndices);

            this.PrepareCornerValues(inputLength, hashTabelIndices, 
                out int cornerCount, out int[] cornerIndices);

            this.GetCornerValues(input, inputLength, intInput, cornerCount, cornerIndices, 
                out float[] cornerValues, out float[]  smoothSteps);

            int iteration = 0;

            float[] currentValues = new float[2.Pow(inputLength - 1)];
            for (int i = 0; i < currentValues.Length; i++)
            {
                currentValues[i + 0] = smoothSteps[iteration].Lerp(cornerValues[i * 2 + 0], cornerValues[i * 2 + 1]);
            }

            float[] tempValues = new float[currentValues.Length / 2];
            while (tempValues.Length > 0)
            {
                iteration++;
                for (int i = 0; i < tempValues.Length; i++)
                {
                    int idx = i * 2;
                    tempValues[i] = smoothSteps[iteration].Lerp(currentValues[idx + 0], currentValues[idx + 1]);
                }

                currentValues = tempValues;
                tempValues = new float[currentValues.Length / 2];
            }

            return currentValues[0];
        }

        private void GetCornerValues(float[] input, int inputLength, int[] intInput, int cornerCount, int[] cornerIndices, out float[] cornerValues, out float[] smoothSteps)
        {
            cornerValues = new float[cornerCount];
            for (int i = 0; i < cornerCount; i++)
            {
                cornerValues[i] = this.ValueTable[cornerIndices[i]];
            }

            smoothSteps = new float[inputLength];
            for (int i = 0; i < inputLength; i++)
            {
                smoothSteps[i] = (input[i] - intInput[i]).SmoothStep();
            }
        }

        private void PrepareInput(float[] input, int inputLength, out int[] intInput, out int[] hashTabelIndices)
        {
            intInput = new int[inputLength];
            for (int i = 0; i < inputLength; i++)
            {
                intInput[i] = (int)Math.Floor(input[i]);
            }

            hashTabelIndices = new int[2 * inputLength];
            for (int i = 0; i < inputLength; i++)
            {
                int baseIndex = i * 2;
                hashTabelIndices[baseIndex + 0] = intInput[i] & this.HashTableSizeMask;
                hashTabelIndices[baseIndex + 1] = (hashTabelIndices[baseIndex + 0] + 1) & this.HashTableSizeMask;
            }
        }

        private void PrepareCornerValues(int inputLength, int[] hashTabelIndices, out int cornerCount, out int[] cornerIndices)
        {
            cornerCount = 2.Pow(inputLength);
            int[] indexMasks = new int[inputLength];
            indexMasks[0] = 1;
            for (int i = 1; i < inputLength; i++)
            {
                indexMasks[i] = 2.Pow(i);
            }

            cornerIndices = new int[cornerCount];
            for (int i = 0; i < cornerCount; i++)
            {
                int index = i;
                int baseIndex = 0;
                int divider = 1;

                for (int j = 0; j < inputLength; j++)
                { // Masks
                    cornerIndices[i] = this.HashTable[cornerIndices[i] + hashTabelIndices[baseIndex + (index & indexMasks[j]) / divider]];
                    divider *= 2;
                    baseIndex += 2;
                }
            }
        }
    }
}