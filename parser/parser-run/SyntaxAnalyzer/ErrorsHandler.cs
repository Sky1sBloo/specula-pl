namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class ErrorsHandler
{
    public List<string> ErrorList { get; private set; } = new();
    public bool SuppressErrors { get; set; } = false;

    public void AddError(string errorMsg)
    {
        if (!SuppressErrors && !ErrorList.Contains(errorMsg))
        {
            ErrorList.Add(errorMsg);
        }
    }

    public void AddError(SyntaxErrorException exception)
    {
        if (!SuppressErrors && !ErrorList.Contains(exception.Message))
        {
            ErrorList.Add(exception.Message);
        }
    }

    public void ClearErrors()
    {
        ErrorList.Clear();
    }
}
