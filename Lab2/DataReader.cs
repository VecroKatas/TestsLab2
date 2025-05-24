namespace Lab2;

public class DataReader
{
    IDataProvider _dataProvider;

    public DataReader(IDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }
    
    public string ReadAllText(string filePath)
    {
        var exists = _dataProvider.FileExists(filePath);
        var content = _dataProvider.ReadAllText(filePath);
        return content;
    }
}