using Misc;

namespace ProcCGen
{
    public interface INoise
    {
        float Get(params float[] input);
        float Get(NoiseConfig config, params float[] input);
    }
}