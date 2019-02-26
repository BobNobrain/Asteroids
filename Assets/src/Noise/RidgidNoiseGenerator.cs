using UnityEngine;

namespace Noise {
public class RidgidNoiseGenerator: INoiseGenerator
{
    public Settings settings;
    public Noise noise;

    public RidgidNoiseGenerator(Settings settings, int seed)
    {
        this.settings = settings;
        this.noise = new Noise(seed);
    }

    public float Eval(Vector3 point)
    {
        float noiseValue = 0;
        float frequency = settings.baseRoughness;
        float amplitude = 1;
        float weight = 1;

        for (int i = 0; i < settings.numLayers; i++)
        {
            float v = 1-Mathf.Abs(noise.Evaluate(point * frequency + settings.centre));
            v *= v;
            v *= weight;
            weight = Mathf.Clamp01(v * settings.weightMultiplier);

            noiseValue += v * amplitude;
            frequency *= settings.roughness;
            amplitude *= settings.persistence;
        }

        noiseValue = Mathf.Max(0, noiseValue - settings.minValue);
        return noiseValue * settings.strength;
    }

    [System.Serializable]
    public struct Settings
    {
        public float strength;
        [Range(1, 8)]
        public int numLayers;
        public float baseRoughness;
        public float roughness;
        public float persistence;
        public Vector3 centre;
        public float minValue;
        public float weightMultiplier;

        public Settings(int seed)
        {
            strength = 1;
            numLayers = 1;
            baseRoughness = 1;
            roughness = 2;
            persistence = .5f;
            weightMultiplier = .8f;

            centre = Vector3.zero;
            minValue = 0f;
        }
    }
}

}
