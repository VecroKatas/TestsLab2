using System.Globalization;

namespace Lab2;

public class TxtFileDataProvider : IDataProvider
{
    public string[] ReadAllLines(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }
        
        return File.ReadAllLines(filePath);
    }
    
    public string ReadAllText(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }
        
        return File.ReadAllText(filePath);
    }
    
    public string ReadLine(string filePath, int lineNumber)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty", nameof(filePath));
        }
        
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }
        
        if (lineNumber < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), "Line number must be non-negative");
        }
        
        string[] lines = File.ReadAllLines(filePath);
        
        if (lineNumber >= lines.Length)
        {
            throw new ArgumentOutOfRangeException(nameof(lineNumber), 
                $"Line number {lineNumber} is out of range. File contains {lines.Length} lines.");
        }
        
        return lines[lineNumber];
    }
    
    public bool FileExists(string filePath)
    {
        var fileExists = File.Exists(filePath);
        
        if (string.IsNullOrWhiteSpace(filePath) || !fileExists)
        {
            throw new FileNotFoundException($"File not found: {filePath}");
        }
        
        return fileExists;
    }
    
    public InputData ReadInputData(string filePath)
    {
        var content = ReadAllText(filePath).Trim();
        return ParseInputData(content);
    }
    
    public InputData ReadInputDataFromFirstLine(string filePath)
    {
        var lines = ReadAllLines(filePath);
        
        if (lines.Length == 0)
        {
            throw new ArgumentException("The file is empty", nameof(filePath));
        }
        
        return ParseInputData(lines[0]);
    }
    
    public InputData[] ReadMultipleInputData(string filePath)
    {
        var lines = ReadAllLines(filePath);
        
        if (lines.Length == 0)
        {
            throw new ArgumentException("The file is empty", nameof(filePath));
        }
        
        var results = new List<InputData>();
        var invalidLines = new List<(int lineNumber, string error)>();
        
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            
            try
            {
                var data = ParseInputData(line);
                results.Add(data);
            }
            catch (Exception ex)
            {
                invalidLines.Add((i + 1, ex.Message));
            }
        }
        
        if (invalidLines.Count > 0)
        {
            var errorMessages = string.Join(Environment.NewLine, 
                invalidLines.Select(l => $"Line {l.lineNumber}: {l.error}"));
            
            Console.WriteLine($"Warning: Some lines could not be parsed:{Environment.NewLine}{errorMessages}");
        }
        
        return results.ToArray();
    }
    
    private InputData ParseInputData(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException("Input string cannot be null or empty", nameof(input));
        }
        
        string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 4)
        {
            throw new FormatException($"Invalid input format. Expected 4 values, got {parts.Length}. Format should be 'a b e x0'");
        }
        
        try
        {
            float a = float.Parse(parts[0], CultureInfo.InvariantCulture);
            float b = float.Parse(parts[1], CultureInfo.InvariantCulture);
            
            if (!int.TryParse(parts[2], out int e))
            {
                throw new FormatException($"Failed to parse '{parts[2]}' as an integer for parameter 'e'");
            }
            
            float x0 = float.Parse(parts[3].Replace(',', '.'), CultureInfo.InvariantCulture);
            
            return new InputData(a, b, e, x0);
        }
        catch (FormatException ex)
        {
            throw new FormatException($"Error parsing input data: {ex.Message}", ex);
        }
    }
}
