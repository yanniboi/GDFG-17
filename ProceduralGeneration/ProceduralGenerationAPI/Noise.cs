//using ProcCGen;
//using System;
//using System.Collections.Generic;
//using System.Linq;

//public class Noise : INoise
//{
//    int[] HashTable { get; set; }
//    Float2[] Gradients { get; set; }
//    Random Random { get; set; }

//    int HashTableSizeMask { get; set; }
//    int GradientsSize { get; set; }

//    public Noise(int seed) : this(seed, 256)
//    {

//    }

//    public Noise(int seed, int gradients)
//    {
//        this.Random = new Random();

//        this.GradientsSize = gradients;
//        this.HashTableSizeMask = this.GradientsSize - 1;

//        this.Gradients = new Float2[gradients];

//        for (int i = 0; i < this.Gradients.Length; i++)
//        {
//            this.Gradients[i] = new Float2(
//                ((float)(this.Random.NextDouble() * 2 - 1)),
//                ((float)(this.Random.NextDouble() * 2 - 1)));
//        }

//        this.HashTable = new int[this.GradientsSize * 2];

//        for (int i = 0; i < this.GradientsSize; i++)
//        {
//            this.HashTable[i] = i;
//        }

//        // Shuffle
//        for (int i = 0; i < this.GradientsSize; i++)
//        {
//            for (int j = 0; j < 5; j++)
//            {
//                int idx = this.Random.Next(this.GradientsSize);
//                int atIdx = this.HashTable[idx];

//                this.HashTable[idx] = this.HashTable[i];
//                this.HashTable[i] = atIdx;
//            }
//        }

//        // Clone first half to second half
//        for (int i = 0; i < this.GradientsSize; i++)
//        {
//            this.HashTable[this.GradientsSize + i] = this.HashTable[i];
//        }
//    }

//    public float Get(NoiseConfig config, params float[] input)
//    {
//        float toReturn = 0;

//        float amp = 1;
//        float freq = config.Frequency;

//        float[] finalInput = new float[input.Length];

//        for (int layer = 0; layer < config.Layers; layer++)
//        {
//            for (int i = 0; i < input.Length; i++)
//            {
//                finalInput[i] = (input[i] * freq);
//            }

//            toReturn += this.Get(finalInput) * amp;

//            amp *= .5f;
//            freq *= 2f;
//        }

//        return this.Get(input);
//    }

//    public float Get(params float[] input)
//    {
//        int inputLength = input.Length;
//        int cornerCount = 2.Pow(inputLength);

//        int[] intInput = new int[inputLength];
//        for (int i = 0; i < inputLength; i++)
//        {
//            intInput[i] = (int)Math.Floor(input[i]);
//        }

//        int[] hashTabelIndices = new int[2 * inputLength];
//        for (int i = 0; i < inputLength; i++)
//        {
//            int baseIndex = i * 2;
//            hashTabelIndices[baseIndex + 0] = intInput[i] & this.HashTableSizeMask;
//            hashTabelIndices[baseIndex + 1] = (hashTabelIndices[baseIndex + 0] + 1) & this.HashTableSizeMask;
//        }

//        int[] gradientIndexMasks = new int[inputLength];
//        gradientIndexMasks[0] = 1;
//        for (int i = 1; i < inputLength; i++)
//        {
//            gradientIndexMasks[i] = 2.Pow(i);
//        }

//        int[] gradientIndices = new int[cornerCount];
//        for (int i = 0; i < cornerCount; i++)
//        {
//            int index = i;
//            int baseIndex = 0;
//            int divider = 1;

//            for (int j = 0; j < inputLength; j++)
//            { // Masks
//                gradientIndices[i] = this.HashTable[gradientIndices[i] + hashTabelIndices[baseIndex + (index & gradientIndexMasks[j]) / divider]];
//                divider *= 2;
//                baseIndex += 2;
//            }
//        }

//        Float2[] gradients = new Float2[cornerCount];
//        for (int i = 0; i < cornerCount; i++)
//        {
//            gradients[i] = this.Gradients[gradientIndices[i]];
//        }

//        float[] fractions = new float[2];
//        fractions[0] = input[0] - intInput[0];
//        fractions[1] = input[1] - intInput[1];

//        Float2[] inputDelta = new Float2[4];
//        inputDelta[0] = new Float2(fractions[0], fractions[1]);
//        inputDelta[1] = new Float2(-fractions[0], fractions[1]);
//        inputDelta[2] = new Float2(fractions[0], -fractions[1]);
//        inputDelta[3] = new Float2(-fractions[0], -fractions[1]);

//        float[] smoothSteps = new float[inputLength];
//        for (int i = 0; i < inputLength; i++)
//        {
//            smoothSteps[i] = fractions[i].SmoothStep();
//        }


//        float lerp1 = smoothSteps[0].Lerp(inputDelta[0].Dot(gradients[0]), inputDelta[1].Dot(gradients[1]));
//        float lerp2 = smoothSteps[0].Lerp(inputDelta[2].Dot(gradients[2]), inputDelta[3].Dot(gradients[3]));
//        //int iteration = 0;

//        //float[] currentValues = new float[2.Pow(inputLength - 1)];
//        //for (int i = 0; i < currentValues.Length; i++)
//        //{
//        //    int baseIndex = i * 2;
//        //    currentValues[i + 0] = smoothSteps[iteration].Lerp(
//        //        gradients[baseIndex + 0].Dot(inputDelta[baseIndex + 0]),
//        //        gradients[baseIndex + 1].Dot(inputDelta[baseIndex + 1]));
//        //    //currentValues[i + 0] = smoothSteps[iteration].Lerp(gradients[i * 2 + 0], gradients[i * 2 + 1]);
//        //}

//        //float[] tempValues = new float[currentValues.Length / 2];
//        //while (tempValues.Length > 0)
//        //{
//        //    iteration++;
//        //    for (int i = 0; i < tempValues.Length; i++)
//        //    {
//        //        int idx = i * 2;
//        //        tempValues[i] = smoothSteps[iteration].Lerp(currentValues[idx + 0], currentValues[idx + 1]);
//        //    }

//        //    currentValues = tempValues;
//        //    tempValues = new float[currentValues.Length / 2];
//        //}

//        return smoothSteps[1].Lerp(lerp1, lerp2);
//    }

//    public float Get3(params float[] input)
//    {
//        int inputLength = input.Length;

//        int[] intInput = input.Select(val => (int)Math.Floor(val)).ToArray();

//        int[] hashTableIndices = new int[4];
//        for (int i = 0; i < inputLength; i++)
//        {
//            int baseIndex = i * 2;
//            hashTableIndices[baseIndex + 0] = intInput[i] & this.HashTableSizeMask;
//            hashTableIndices[baseIndex + 1] = (hashTableIndices[baseIndex + 0] + 1) & this.HashTableSizeMask;
//        }

//        int[] indexMasks = new int[inputLength];
//        indexMasks[0] = 1;
//        for (int i = 1; i < inputLength; i++)
//        {
//            indexMasks[i] = 2.Pow(i);
//        }

//        int cornerCount = 4;

//        int[] gradientIndices = new int[cornerCount];
//        for (int i = 0; i < cornerCount; i++)
//        {
//            int index = i;
//            int baseIndex = 0;
//            int divider = 1;

//            for (int j = 0; j < inputLength; j++)
//            { // Masks
//                gradientIndices[i] = this.HashTable[gradientIndices[i] + hashTableIndices[baseIndex + (index & indexMasks[j]) / divider]];
//                divider *= 2;
//                baseIndex += 2;
//            }
//        }

//        float[] fractions = new float[2];
//        fractions[0] = input[0] - intInput[0]; // minx
//        fractions[1] = input[1] - intInput[1]; // miny
//        // Pos = input
//        // xmin Floor, xMax = xmin + 1

//        Float2[] inputDelta = new Float2[4];
//        inputDelta[0] = new Float2(input[0] - intInput[0], input[1] - intInput[1]);
//        inputDelta[1] = new Float2(input[0] - (intInput[0] + 1), input[1] - intInput[1]);
//        inputDelta[2] = new Float2(input[0] - intInput[0], input[1] - (intInput[1] + 1));
//        inputDelta[3] = new Float2(input[0] - (intInput[0] + 1), input[1] - (intInput[1] + 1));

//        float[] steps = new float[2];
//        steps[0] = fractions[0].SmoothStep();
//        steps[1] = fractions[1].SmoothStep();

//        float[] values = new float[2];
//        values[0] = steps[0].Lerp(
//            inputDelta[0].Dot(this.Gradients[gradientIndices[0]]),
//            inputDelta[1].Dot(this.Gradients[gradientIndices[1]]));

//        values[1] = steps[0].Lerp(
//            inputDelta[2].Dot(this.Gradients[gradientIndices[2]]),
//            inputDelta[3].Dot(this.Gradients[gradientIndices[3]]));

//        float toReturn = steps[1].Lerp(values[0], values[1]);

//        return toReturn.Map(-1, 1, 0, 1);
//    }

//    public float Get2(params float[] input)
//    {
//        int inputLength = input.Length;

//        int[] intInput = new int[inputLength];
//        for (int i = 0; i < inputLength; i++)
//        {
//            intInput[i] = (int)Math.Floor(input[i]);
//        }

//        int[] gradientIndices = new int[4];
//        for (int i = 0; i < inputLength; i++)
//        {
//            int baseIndex = i * 2;
//            gradientIndices[baseIndex + 0] = intInput[i] & this.HashTableSizeMask;
//            gradientIndices[baseIndex + 1] = (gradientIndices[baseIndex + 0] + 1) & this.HashTableSizeMask;
//        }

//        float[] fractions = new float[2];
//        fractions[0] = input[0] - intInput[0];
//        fractions[1] = input[1] - intInput[1];

//        Float2[] inputDelta = new Float2[4];
//        inputDelta[0] = new Float2(fractions[0], fractions[1]);
//        inputDelta[1] = new Float2(-fractions[0], fractions[1]);
//        inputDelta[2] = new Float2(fractions[0], -fractions[1]);
//        inputDelta[3] = new Float2(-fractions[0], -fractions[1]);

//        float[] steps = new float[2];
//        steps[0] = fractions[0].SmoothStep();
//        steps[1] = fractions[1].SmoothStep();

//        float[] values = new float[2];
//        values[0] = steps[0].Lerp(
//            inputDelta[0].Dot(this.Gradients[gradientIndices[0]]),
//            inputDelta[1].Dot(this.Gradients[gradientIndices[1]]));
//        values[1] = steps[0].Lerp(
//            inputDelta[2].Dot(this.Gradients[gradientIndices[2]]),
//            inputDelta[3].Dot(this.Gradients[gradientIndices[3]]));

//        float toReturn = steps[1].Lerp(values[0], values[1]);

//        return toReturn.Map(-1, 1, 0, 1);
//    }
//}
