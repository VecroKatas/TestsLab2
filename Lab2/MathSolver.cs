namespace Lab2;

public class MathSolver
{
    public float Func(float x)
    {
        if (x <= 0)
        {
            throw new Exception("Input value must be greater than zero");
        }
        return MathF.Pow(x, 2) * MathF.Log10(x) - 1;
    }

    public float F1(float x)
    {
        return 2 * x * MathF.Log10(x) + x / MathF.Log(10);
    }

    public float F2(float x)
    {
        if (x < 1)
        {
            return -(2 * MathF.Log10(x) + 3 / MathF.Log(10));
        }
        return 2 * MathF.Log10(x) + 3 / MathF.Log(10);
    }

    public float Calcm1(float a, float b)
    {
        float ma = MathF.Abs(F1(a));
        float mb = MathF.Abs(F1(b));
        return MathF.Min(ma, mb);
    }

    public float CalcM2(float a, float b)
    {
        float ma = MathF.Abs(F2(a));
        float mb = MathF.Abs(F2(b));
        return MathF.Max(ma, mb);
    }

    public float[] CalcCoefficients(float a, float b, float x0)
    {
        if (x0 > b || x0 < a)
            throw new Exception("x0 must be a <= x0 <= b");
        
        float M2, m1, z0, q;

        m1 = Calcm1(a, b);
        M2 = CalcM2(a, b);
        z0 = MathF.Abs(a - x0) > MathF.Abs(b - x0) ? MathF.Abs(a - x0) : MathF.Abs(b - x0);
        q = M2 * z0 / (2 * m1);
        
        return [m1, M2, z0, q];
    }
}