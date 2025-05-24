using Lab2;

var mathSolver = new MathSolver();
var dataProvider = new TxtFileDataProvider();

// x^2 * lg(x) - 1 = 0
// log(x) - 1/x^2=0

Console.WriteLine("Do you want to read input from a file? (y/n)");
string response = Console.ReadLine().ToLower();

if (response == "y" || response == "yes")
{
    ProcessFileInput();
}
else
{
    ProcessConsoleInput();
}

void ProcessFileInput()
{
    Console.WriteLine("Enter the path to the input file:");
    string filePath = Console.ReadLine();
    
    try
    {
        if (!dataProvider.FileExists(filePath))
        {
            Console.WriteLine($"File not found: {filePath}");
            ProcessConsoleInput();
            return;
        }
        
        InputData[] dataSets = dataProvider.ReadMultipleInputData(filePath);
        
        if (dataSets.Length == 0)
        {
            Console.WriteLine("No valid data found in the file. Using console input instead.");
            ProcessConsoleInput();
            return;
        }
        
        Console.WriteLine($"Found {dataSets.Length} data sets in the file.");
        
        for (int dataIndex = 0; dataIndex < dataSets.Length; dataIndex++)
        {
            var data = dataSets[dataIndex];
            Console.WriteLine();
            Console.WriteLine($"Processing data set #{dataIndex + 1}: {data}");
            
            // Validate input data
            if (data.A <= 0)
            {
                Console.WriteLine($"Skipping data set #{dataIndex + 1}. 'a' must be > 0");
                continue;
            }
            
            float funcA = mathSolver.Func(data.A);
            float funcB = mathSolver.Func(data.B);
            
            if (!((funcA < 0 && funcB > 0) || (funcA > 0 && funcB < 0)))
            {
                Console.WriteLine($"Skipping data set #{dataIndex + 1}. f(a) and f(b) must have different signs");
                continue;
            }
            
            // Process valid data set
            RunBisectionMethod(data.A, data.B, data.E);
            
            // Check if x0 is valid for Newton's method
            bool x0Valid = data.X0 >= data.A && data.X0 <= data.B && 
                           mathSolver.Func(data.X0) * mathSolver.F2(data.X0) > 0;
            
            if (x0Valid)
            {
                RunNewtonMethod(data.A, data.B, data.E, data.X0);
            }
            else
            {
                Console.WriteLine("X0 in the data set is not valid for Newton's method.");
                
                // Ask for x0 input
                float x0 = GetValidX0ForNewton(data.A, data.B);
                RunNewtonMethod(data.A, data.B, data.E, x0);
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error processing file: {ex.Message}");
        ProcessConsoleInput();
    }
}

void ProcessConsoleInput()
{
    float a = 0, b = 0;
    int e;
    
    // Get a and b
    while (a == 0)
    {
        Console.WriteLine("Enter a and b");
        string[] input = Console.ReadLine().Split();
        float _a = float.Parse(input[0]);
        float _b = float.Parse(input[1]);

        if (_a > 0)
        {
            float A = mathSolver.Func(_a), B = mathSolver.Func(_b);
            
            if ((A < 0 && B > 0) || (A > 0 && B < 0))
            {
                a = _a;
                b = _b;
            }
            else
            {
                Console.WriteLine("Wrong input. f(a) and f(b) has to have different signs");
            }
        }
        else
        {
            Console.WriteLine("Wrong input. a must be > 0");
        }
    }

    Console.WriteLine("Enter e (as in 10^-e)");
    e = Convert.ToInt32(Console.ReadLine());
    
    // Run Bisection method
    RunBisectionMethod(a, b, e);
    
    // Get x0 for Newton's method
    float x0 = GetValidX0ForNewton(a, b);
    
    // Run Newton's method
    RunNewtonMethod(a, b, e, x0);
}

float GetValidX0ForNewton(float a, float b)
{
    float x0 = 0;
    while (x0 == 0)
    {
        Console.WriteLine("Type x_0 for Newton's method");

        float _x0 = float.Parse(Console.ReadLine());
        if (_x0 >= a && _x0 <= b)
        {
            if (mathSolver.Func(_x0) * mathSolver.F2(_x0) > 0)
            {
                x0 = _x0;
                
                Console.WriteLine("f(x_0) = " + mathSolver.Func(x0) + "; f``(x_0) = " + mathSolver.F2(x0));
            }
            else
            {
                Console.WriteLine("Wrong x_0 input.");
            }
            Console.WriteLine("f(x_0) * f``(x_0) > 0 = " + (mathSolver.Func(_x0) * mathSolver.F2(_x0) > 0));
        }
        else
            Console.WriteLine("Wrong input. x0 must be a <= x0 <= b");
    }
    
    return x0;
}

void RunBisectionMethod(float a, float b, float e)
{
    Console.WriteLine();
    Console.WriteLine("Метод ділення навпіл");
    Console.WriteLine($"Parameters: a = {a}, b = {b}, e = {e}");
    
    float init_a = a, init_b = b;
    float x = (a + b) / 2;
    int n = (int)Math.Floor(MathF.Log2((b - a) / MathF.Pow(10, -e))) + 1;

    Console.WriteLine("Iterations: " + n);

    float tmp = mathSolver.Func(x);

    Console.WriteLine("n \t\t x_n \t\t f(x_n)");
    Console.WriteLine(0 + "\t\t" + x + "\t\t" + tmp);

    float x_ = 0;

    for (int i = 1; i <= n; i++)
    {
        x_ = x;
        if ((a <= 0 && tmp >= 0) || (a >= 0 && tmp < 0))
            a = x;
        else if ((b >= 0 && tmp >= 0) || (b <= 0 && tmp < 0))
            b = x;
        x = (a + b) / 2;
            
        tmp = mathSolver.Func(x);

        if (MathF.Abs(x_ - x) < MathF.Pow(10, -e))
        {
            Console.WriteLine();
            Console.WriteLine("Вихід раніше: " + i + "\t\t" + x + "\t\t" + tmp);
            Console.WriteLine();
            break;
        }
            
        Console.WriteLine(i + "\t\t" + x + "\t\t" + tmp);
    }
    
    Console.WriteLine($"Root found with bisection method: {x}");
}

void RunNewtonMethod(float a, float b, float e, float x0)
{
    Console.WriteLine();
    Console.WriteLine("Метод Ньютона");
    Console.WriteLine($"Parameters: a = {a}, b = {b}, e = {e}, x0 = {x0}");

    float M2, m1, z0, q, xi = x0;

    var coefficients = mathSolver.CalcCoefficients(a, b, x0);

    m1 = coefficients[0];
    M2 = coefficients[1];
    z0 = coefficients[2];
    q = coefficients[3];

    Console.WriteLine("m1 = " + m1);
    Console.WriteLine("M2 = " + M2);
    Console.WriteLine("z0 = " + z0);
    Console.WriteLine("q = " + q);

    float n0 = MathF.Floor(MathF.Log2(MathF.Log(z0 / MathF.Pow(10, -e)) / MathF.Log(1 / q) + 1)) + 1;
    Console.WriteLine("n0 = " + n0);

    Console.WriteLine("n \t\t x_n \t\t f(x_n)");
    Console.WriteLine("0 \t\t" + xi + "\t\t" + mathSolver.Func(xi));

    for (int i = 1; i <= n0; i++)
    {
        xi = xi - mathSolver.Func(xi) / mathSolver.F1(xi);
        Console.WriteLine(i + "\t\t" + xi + "\t\t" + mathSolver.Func(xi));
    }
    Console.WriteLine();

    int j = 0;
    xi = x0;
    float x_ = 0;

    while (MathF.Abs(x_ - xi) > MathF.Pow(10, -e))
    {
        x_ = xi;
        xi = xi - mathSolver.Func(xi) / mathSolver.F1(xi);
        j++;
        Console.WriteLine(j + "\t\t" + xi + "\t\t" + mathSolver.Func(xi));
    }
    
    Console.WriteLine($"Root found with Newton's method: {xi}");
}