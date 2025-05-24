namespace Lab2;

public interface IDataProvider
{
    string[] ReadAllLines(string filePath);
    
    string ReadAllText(string filePath);
    
    bool FileExists(string filePath);
    
    InputData ReadInputData(string filePath);
    
    InputData ReadInputDataFromFirstLine(string filePath);
    
    InputData[] ReadMultipleInputData(string filePath);
}
