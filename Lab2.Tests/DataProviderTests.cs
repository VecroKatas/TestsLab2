using System.IO;
using Moq;
using Xunit;

namespace Lab2.Tests;

public class DataProviderTests
{
    [Fact]
    public void ReadAllLines_WhenCalled_ReturnsExpectedData()
    {
        // Arrange
        var mockDataProvider = new Mock<IDataProvider>(MockBehavior.Strict);
        var testFilePath = "Tests/test1.txt";
        var expectedLines = new[] { "Line 1", "Line 2", "Line 3" };
        
        mockDataProvider.Setup(dp => dp.ReadAllLines(testFilePath))
            .Returns(expectedLines)
            .Verifiable();
        
        // Act
        var result = mockDataProvider.Object.ReadAllLines(testFilePath);
        
        // Assert
        Assert.Equal(expectedLines, result);
        mockDataProvider.Verify(dp => dp.ReadAllLines(testFilePath), Times.Once);
    }
    
    [Fact]
    public void ReadAllLines_FileNotFound_ThrowsFileNotFoundException()
    {
        // Arrange
        var mockDataProvider = new Mock<IDataProvider>();
        var nonExistentFilePath = "Tests/nonexistent.txt";
        
        mockDataProvider.Setup(dp => dp.ReadAllLines(nonExistentFilePath))
            .Throws(new FileNotFoundException($"File not found: {nonExistentFilePath}"));
        
        // Act & Assert
        var exception = Assert.Throws<FileNotFoundException>(() => 
            mockDataProvider.Object.ReadAllLines(nonExistentFilePath));
        
        Assert.Contains(nonExistentFilePath, exception.Message);
    }
    
    [Fact ]
    public void Methods_CalledInSpecificOrder_VerifiesSequence()
    {
        // Arrange
        var mockDataProvider = new Mock<IDataProvider>(MockBehavior.Strict);
        var testFilePath = "Tests/sequence.txt";
        var expectedText = "Sample text content";
        
        var sequence = new MockSequence();
        
        // Setup sequence expectations
        mockDataProvider.InSequence(sequence).Setup(dp => dp.FileExists(testFilePath)).Returns(true);
        mockDataProvider.InSequence(sequence).Setup(dp => dp.ReadAllText(testFilePath)).Returns(expectedText);

        // Act
        var _sut = new DataReader(mockDataProvider.Object);
        var result = _sut.ReadAllText(testFilePath);
        
        // Assert
        mockDataProvider.Verify(dp => dp.ReadAllText(testFilePath), Times.Once);
        
        Assert.Equal(expectedText, result);
    }
    
    [Fact]
    public void ReadAllText_MultipleCallsReturnDifferentValues()
    {
        var mockDataProvider = new Mock<IDataProvider>();
        var filePath = "Tests/changing.txt";
        
        mockDataProvider.SetupSequence(dp => dp.ReadAllText(filePath))
            .Returns("First call result")
            .Returns("Second call result")
            .Returns("Third call result")
            .Throws(new IOException("Connection lost after three calls"));
        
        var result1 = mockDataProvider.Object.ReadAllText(filePath);
        Assert.Equal("First call result", result1);
        
        var result2 = mockDataProvider.Object.ReadAllText(filePath);
        Assert.Equal("Second call result", result2);
        
        var result3 = mockDataProvider.Object.ReadAllText(filePath);
        Assert.Equal("Third call result", result3);
        
        var exception = Assert.Throws<IOException>(() => mockDataProvider.Object.ReadAllText(filePath));
        Assert.Equal("Connection lost after three calls", exception.Message);
    }
}
