using System.IO;
using Moq;
using Xunit;

namespace Lab2.Tests;

public class DataProviderTests
{
    [Fact]
    public void ReadAllLines_WhenCalled_ReturnsExpectedData()
    {
        var mockDataProvider = new Mock<IDataProvider>(MockBehavior.Strict);
        var testFilePath = "Tests/test.txt";
        var expectedLines = new[] { "Line 1", "Line 2", "Line 3" };

        mockDataProvider.Setup(dp => dp.FileExists(testFilePath))
            .Returns(true);
        mockDataProvider.Setup(dp => dp.ReadAllLines(testFilePath))
            .Returns(expectedLines)
            .Verifiable();
        
        var result = new DataReader(mockDataProvider.Object).ReadAllLines(testFilePath);
        
        Assert.Equal(expectedLines, result);
        mockDataProvider.Verify(dp => dp.ReadAllLines(testFilePath), Times.Once);
    }
    
    [Fact]
    public void ReadAllLines_FileNotFound_ThrowsFileNotFoundException()
    {
        var mockDataProvider = new Mock<IDataProvider>();
        var nonExistentFilePath = "Tests/test.txt";
        
        mockDataProvider.Setup(dp => dp.ReadAllLines(nonExistentFilePath))
            .Throws(new FileNotFoundException($"File not found: {nonExistentFilePath}"));
        
        var exception = Assert.Throws<FileNotFoundException>(() => 
            new DataReader(mockDataProvider.Object).ReadAllLines(nonExistentFilePath));
        
        Assert.Contains(nonExistentFilePath, exception.Message);
    }
    
    [Fact ]
    public void Methods_CalledInSpecificOrder_VerifiesSequence()
    {
        var mockDataProvider = new Mock<IDataProvider>(MockBehavior.Strict);
        var testFilePath = "Tests/test.txt";
        var expectedText = "Sample text content";
        
        var sequence = new MockSequence();
        
        mockDataProvider.InSequence(sequence).Setup(dp => dp.FileExists(testFilePath)).Returns(true);
        mockDataProvider.InSequence(sequence).Setup(dp => dp.ReadAllText(testFilePath)).Returns(expectedText);

        var result = new DataReader(mockDataProvider.Object).ReadAllText(testFilePath);
        
        mockDataProvider.Verify(dp => dp.ReadAllText(testFilePath), Times.Once);
        Assert.Equal(expectedText, result);
    }
    
    [Fact]
    public void ReadAllText_MultipleCallsReturnDifferentValues()
    {
        var mockDataProvider = new Mock<IDataProvider>();
        var filePath = "Tests/test.txt";
        var results = new string[] {"First call result", "Second call result", "Third call result"};
        
        mockDataProvider.SetupSequence(dp => dp.ReadLine(filePath, It.IsAny<int>()))
            .Returns("First call result")
            .Returns("Second call result")
            .Returns("Third call result")
            .Throws(new ArgumentOutOfRangeException("Line number 3 is out of range. File contains 3 lines."));

        var dataReader = new DataReader(mockDataProvider.Object);
        
        for (int i = 0; i < 4; i++)
        {
            try
            {
                var result = dataReader.ReadLine(filePath, i);
                Assert.Equal(results[i], result);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Assert.Equal("Line number 3 is out of range. File contains 3 lines.", e.ParamName);
            }
        }
    }
}
