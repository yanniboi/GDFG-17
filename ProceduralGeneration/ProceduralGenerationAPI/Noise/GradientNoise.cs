using Misc;
using ProcCGen;
using System;

namespace Noise
{
    public class GradientNoise : INoise
{
    Random Random { get; set; }

    Float3[] Gradients { get; set; }
    int[] HashTable { get; set; }

    int TableSize { get; set; }
    int TableSizeMask { get; set; }

    public GradientNoise(int seed)
    {
        this.TableSize = 256;
        this.TableSizeMask = this.TableSize - 1;

        this.Random = new Random(seed);

        this.Gradients = new Float3[this.TableSize];
        this.HashTable = new int[this.TableSize * 2];

        for (int i = 0; i< this.TableSize; ++i) { 

            // better
            float theta = (float)Math.Acos(2 * this.Random.NextDouble() - 1);
            float phi = (float)(2 * this.Random.NextDouble() * Math.PI);

            float x = (float)Math.Cos(phi) * (float)Math.Sin(theta);
            float y = (float)Math.Sin(phi) * (float)Math.Sin(theta);
            float z = (float)Math.Cos(theta);

            this.Gradients[i] = new Float3(x, y, z);
            this.HashTable[i] = i; 
        }

        for (int i = 0; i < this.TableSize; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                int idx = this.Random.Next(this.TableSize);
                int atIdx = this.HashTable[idx];

                this.HashTable[idx] = this.HashTable[i];
                this.HashTable[i] = atIdx;
            }
        }

        for (int i = 0; i < this.TableSize; i++)
        {
            this.HashTable[this.TableSize + i] = this.HashTable[i];
        }
    }


    public float Get(NoiseConfig config, params float[] input)
    {
        float toReturn = 0;

        float amp = config.Amplitude;
        float freq = config.Frequency;

        float[] finalInput = new float[input.Length];

        for (int layer = 0; layer < config.Layers; layer++)
        {
            for (int i = 0; i < input.Length; i++)
            {
                finalInput[i] = (input[i] * freq);
            }

            toReturn += this.Get(finalInput) * amp;

            amp *= .5f;
            freq *= 2f;
        }

        return toReturn;
    }

    public float Get(params float[] input)
    {
        int inputLength = input.Length;

        this.GetInput(input, inputLength, out int[] inputMin, out int[] inputMax, out int[] intInput);

        this.GetFractions(inputLength, input, out float[] fractions);

        this.GetSmoothSteps(fractions, inputLength, out float[] smoothSteps);

        int[] indexMasks = new int[inputLength];
        indexMasks[0] = 1;
        for (int i = 1; i < inputLength; i++)
        {
            indexMasks[i] = 2.Pow(i);
        }

        Float3[] gradients = new Float3[2.Pow(inputLength)];
        int[] gradientIndices = new int[gradients.Length];

        // Create gradients at the corners
        for (int i = 0; i < gradientIndices.Length; i++)
        {
            for (int j = 0; j < inputLength; j++)
            {
                int index = this.HashTable[gradientIndices[i] + intInput[j * 2 + (i & indexMasks[j]) / indexMasks[j]]];
                gradientIndices[i] = index;
            }

            gradients[i] = this.Gradients[gradientIndices[i]];
        }

        //towards input... generate vectors going from the grid points to p
        float[] toInput = new float[inputLength * 2];
        for (int i = 0; i < inputLength; i++)
        {
            toInput[i * 2 + 0] = fractions[i + 0];
            toInput[i * 2 + 1] = fractions[i + 0] - 1; //new Float2(fractions[i + 0], fractions[i + 0] - 1);
        }

        Float3[] vectorToInput = new Float3[2.Pow(inputLength)];

        for (int i = 0; i < vectorToInput.Length; i++)
        {
            Float3 value = new Float3();
            for (int j = 0; j < inputLength; j++)
            {
                value[j] = toInput[j * 2 + (i & indexMasks[j]) / indexMasks[j]];
            }

            vectorToInput[i] = value;
        }

        float[] lerpValues = new float[vectorToInput.Length / 2];

        int run = 0;
        for (int i = 0; i < lerpValues.Length; i++)
        {
            lerpValues[i] = smoothSteps[run].Lerp(
                vectorToInput[i * 2 + 0].Dot(gradients[i * 2 + 0]), 
                vectorToInput[i * 2 + 1].Dot(gradients[i * 2 + 1]));
        }

        while (lerpValues.Length > 1)
        {
            run++;
            float[] tempValues = lerpValues;
            lerpValues = new float[lerpValues.Length / 2];

            for (int i = 0; i < lerpValues.Length; i++)
            {
                lerpValues[i] = smoothSteps[run].Lerp(tempValues[i * 2 + 0] , tempValues[i * 2 + 1]);
            }

        }

        return lerpValues[0];
    }

    private void GetSmoothSteps(float[] fractions, int inputLength, out float[] smoothSteps)
    {
        smoothSteps = new float[inputLength];
        for (int i = 0; i < smoothSteps.Length; i++)
        {
            smoothSteps[i] = fractions[i].SmoothStep();
        }
    }

    private void GetInput(float[] input, int inputLength, out int[] inputMin, out int[] inputMax, out int[] intInput)
    {
        inputMin = new int[inputLength];
        inputMax = new int[inputLength];
        intInput = new int[inputLength * 2];

        for (int i = 0; i < inputMin.Length; i++)
        {
            inputMin[i] = ((int)Math.Floor(input[i])) & this.TableSizeMask;
            inputMax[i] = (inputMin[i] + 1) & this.TableSizeMask;
            intInput[i * 2 + 0] = inputMin[i];
            intInput[i * 2 + 1] = inputMax[i];
        }
    }

    private void GetFractions(int inputLength, float[] input, out float[] fractions)
    {
        fractions = new float[inputLength];
        for (int i = 0; i < fractions.Length; i++)
        {
            fractions[i] = input[i] - ((int)Math.Floor(input[i]));
        }
    }

    float Smoothstep(float t)
    {
        return t * t * (3 - 2 * t);
    }

    int Hash(int x, int y)
    { 
        int tmp = this.HashTable[this.HashTable[x] + y];
        int hash = this.HashTable[x];
        hash = this.HashTable[hash + y];
        return hash;
    }
}
}