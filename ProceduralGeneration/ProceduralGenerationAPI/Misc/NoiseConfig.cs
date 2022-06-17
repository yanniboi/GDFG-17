namespace Misc
{
    public class NoiseConfig
    {
        public int Layers { get; private set; }
        public float Amplitude { get; private set; }
        public float Frequency { get; private set; }


        public NoiseConfig(int layers, float amplitude, float frequency)
        {
            this.Layers = layers;
            this.Amplitude = amplitude;
            this.Frequency = frequency;
        }

    }
}