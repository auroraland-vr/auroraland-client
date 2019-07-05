namespace VLB
{
    public enum ColorMode
    {
        Flat,       // Apply a flat/plain/single color
        Gradient    // Apply a gradient
    }

    public enum AttenuationEquation
    {
        Linear = 0,     // Simple linear attenuation.
        Quadratic = 1,  // Quadratic attenuation, which usually gives more realistic results.
        Blend = 2       // Custom blending mix between linear and quadratic attenuation formulas. Use attenuationEquation property to tweak the mix.
    }
}
