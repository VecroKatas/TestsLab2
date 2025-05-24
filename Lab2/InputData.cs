namespace Lab2;

public struct InputData
{
    public float A { get; set; }
    
    public float B { get; set; }
    
    public float E { get; set; }
    
    public float X0 { get; set; }
    
    public InputData(float a, float b, float e, float x0)
    {
        A = a;
        B = b;
        E = e;
        X0 = x0;
    }
    
    public override string ToString()
    {
        return $"A: {A}, B: {B}, E: {E}, X0: {X0}";
    }
}
