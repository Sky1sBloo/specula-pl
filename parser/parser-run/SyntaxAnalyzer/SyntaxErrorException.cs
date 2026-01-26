using System.Text;
namespace SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

public class SyntaxErrorException : Exception
{
    public override string Message { get; }

    public SyntaxErrorException(List<string> expectedTokens, Token? receivedToken, bool getLastTokenPos = false)
    {
        StringBuilder outMsg = new StringBuilder();
        outMsg.Append("Expected token: ");
        foreach (string token in expectedTokens)
        {
            outMsg.AppendFormat("{0}, ", token);
        }
        outMsg.Length--;

        if (receivedToken == null)
        {
            outMsg.Append(" but reached end of input.");
            Message = outMsg.ToString();
            return;
        }
        outMsg.AppendFormat(" Received: {0}. ", receivedToken.Type.ToString());
        if (getLastTokenPos)
            outMsg.Append(getTokenEndPos(receivedToken));
        else
            outMsg.Append(getTokenPos(receivedToken));
        Message = outMsg.ToString();

    }

    public SyntaxErrorException(Token? prevToken)
    {
        StringBuilder outMsg = new StringBuilder();
        outMsg.Append("Unexpected end of input.");
        if (prevToken != null)
        {
            outMsg.AppendFormat(" Last token: ", prevToken.Type.ToString());
            outMsg.Append(getTokenEndPos(prevToken));
        }
        Message = outMsg.ToString();
    }

    public SyntaxErrorException(string message)
    {
        Message = message;
    }

    private string getTokenPos(Token token)
    {
        return $" at {token.Line}:{token.CharStart}";
    }

    private string getTokenEndPos(Token token)
    {
        return $" at {token.Line}:{token.CharEnd}";
    }
}
