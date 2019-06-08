using NUnit.Framework;

[TestFixture]
[Category("Extensions")]
public class StringExtensions_Tests
{
    #region DelimitedFormat(string tag, char delimiter, params object[] parameters)
    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Normal_Usage()
    {
        string expected = "TAG A B C D";
        string actual = StringExtension.DelimitedFormat("TAG", ' ', "A", "B", "C", "D");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Null_Tag_Null_Params_Array()
    {
        string actual = StringExtension.DelimitedFormat(null, ' ', null);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Nonnull_Tag_Null_Params_Array()
    {
        string actual = StringExtension.DelimitedFormat("TAG", ' ', null);
        string expected = "TAG";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Null_Params()
    {
        string actual = StringExtension.DelimitedFormat("TAG", ' ', null, null, null);
        string expected = "TAG";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Leading_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.DelimitedFormat("TAG", ' ', null, null, "S");
        string expected = "TAG S";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Test_Mixed_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.DelimitedFormat("TAG", ' ', null, "S", null, "X");
        string expected = "TAG S X";
        Assert.AreEqual(expected, actual);
    }
    #endregion

    #region DelimitedFormat(char delimiter, params object[] parameters)
    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Override_Char_Params_Test_Normal_Usage()
    {
        string expected = "A B C D";
        string actual = StringExtension.DelimitedFormat(' ', "A", "B", "C", "D");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Override_Char_Params_Test_Null_Params_Array()
    {
        string actual = StringExtension.DelimitedFormat(' ', null);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Override_Char_Params_Test_Null_Params()
    {
        string actual = StringExtension.DelimitedFormat(' ', null, null, null);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Override_Char_Params_Test_Leading_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.DelimitedFormat(' ', null, null, "S");
        string expected = "S";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("DelimitedFormat")]
    public void DelimetedFormat_Override_Char_Params_Test_Mixed_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.DelimitedFormat(' ', null, "S", null, "X");
        string expected = "S X";
        Assert.AreEqual(expected, actual);
    }
    #endregion


    #region CSVFormat(string tag, params object[] parameters)
    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Normal_Usage()
    {
        string expected = "TAG A,B,C,D";
        string actual = StringExtension.CSVFormat("TAG", "A", "B", "C", "D");
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Null_Params_Array()
    {
        string actual = StringExtension.CSVFormat(null, null);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Null_Params()
    {
        string actual = StringExtension.CSVFormat("TAG", null, null, null);
        string expected = "TAG";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Leading_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.CSVFormat("TAG", null, null, "S");
        string expected = "TAG S";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Mixed_Null_Param_Nonnull_Param()
    {
        string actual = StringExtension.CSVFormat("TAG", null, "S", null, "X");
        string expected = "TAG S,X";
        Assert.AreEqual(expected, actual);
    }


    [Test]
    [Category("CSVFormat")]
    public void CSVFormat_String_Params_Test_Normal_Usage_Null_Tag()
    {
        string actual = StringExtension.CSVFormat(null, "A", "B", "C", "D");
        string expected = "A,B,C,D";
        Assert.AreEqual(expected, actual);
    }
    #endregion


    #region Truncate(this string value, int maxLength)
    [Test]
    [Category("Truncate")]
    public void Truncate_Test_Normal_Usage()
    {
        string actual = "ABCDE".Truncate(4);
        string expected = "ABCD";
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("Truncate")]
    public void Truncate_Test_Empty()
    {
        string actual = string.Empty.Truncate(4);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("Truncate")]
    public void Truncate_Test_ZeroLength()
    {
        string actual = "ABCDE".Truncate(0);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("Truncate")]
    public void Truncate_Test_NegativeLength()
    {
        string actual = "ABCDE".Truncate(-1);
        string expected = string.Empty;
        Assert.AreEqual(expected, actual);
    }

    [Test]
    [Category("Truncate")]
    public void Truncate_Test_MaxLength()
    {
        string actual = "ABCDE".Truncate(int.MaxValue);
        string expected = "ABCDE";
        Assert.AreEqual(expected, actual);
    }
    #endregion
}
