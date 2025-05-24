namespace Lab2;

public class DataReader
{
    IDataProvider _dataProvider;

    public DataReader(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    public bool FileExists(string filePath)
    {
        return _dataProvider.FileExists(filePath);
    }
    
    public string ReadAllText(string filePath)
    {
        string content = string.Empty;
        if (_dataProvider.FileExists(filePath))
            content = _dataProvider.ReadAllText(filePath);
        return content;
    }
    
    public string[] ReadAllLines(string filePath)
    {
        return _dataProvider.ReadAllLines(filePath);
    }

    public string ReadLine(string filePath, int lineNumber)
    {
        return _dataProvider.ReadLine(filePath, lineNumber);
    }

    public InputData ReadInputData(string filePath)
    {
        return _dataProvider.ReadInputData(filePath);
    }
    
    public InputData ReadInputDataFromFirstLine(string filePath)
    {
        return _dataProvider.ReadInputDataFromFirstLine(filePath);
    }

    public InputData[] ReadMultipleInputData(string filePath)
    {
        return _dataProvider.ReadMultipleInputData(filePath);
    }
}