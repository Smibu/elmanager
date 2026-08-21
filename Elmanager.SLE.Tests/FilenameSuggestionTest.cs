using Elmanager.SLE.Dialogs.Settings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elmanager.SLE.Tests;

[TestClass]
public class FilenameSuggestionTest
{
    [TestMethod]
    public void CreatesFirstPaddedFilename()
    {
        var result = FilenameSuggestion.Create("MyLev???", []);

        Assert.AreEqual("MyLev001", result);
    }

    [TestMethod]
    public void ContinuesDescendingSequenceAboveOne()
    {
        string[] levelFiles = ["MyLev005.lev", "MyLev003.lev"];

        var result = FilenameSuggestion.Create("MyLev???", levelFiles);

        Assert.AreEqual("MyLev002", result);
    }

    [TestMethod]
    public void ContinuesAfterHighestNumberOnceOneExists()
    {
        string[] levelFiles = ["MyLev001.lev", "MyLev005.lev"];

        var result = FilenameSuggestion.Create("MyLev???", levelFiles);

        Assert.AreEqual("MyLev006", result);
    }

    [TestMethod]
    public void IgnoresUnrelatedAndNonNumericFilenames()
    {
        string[] levelFiles = ["MyLev005.lev", "MyLevDraft.lev", "Other001.lev"];

        var result = FilenameSuggestion.Create("MyLev???", levelFiles);

        Assert.AreEqual("MyLev004", result);
    }

    [TestMethod]
    public void SupportsSuffixAfterCounter()
    {
        string[] levelFiles = ["MyLev003-draft.lev"];

        var result = FilenameSuggestion.Create("MyLev???-draft", levelFiles);

        Assert.AreEqual("MyLev002-draft", result);
    }

    [TestMethod]
    public void ReturnsLiteralFilenameWithoutCounter()
    {
        var result = FilenameSuggestion.Create("MyLevel", ["MyLevel.lev"]);

        Assert.AreEqual("MyLevel", result);
    }

    [TestMethod]
    public void RejectsMultipleCounterGroups()
    {
        var error = FilenameSuggestion.ValidatePattern("My?Lev???");

        Assert.IsNotNull(error);
    }
}
