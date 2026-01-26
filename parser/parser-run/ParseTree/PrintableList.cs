namespace SpeculaSyntaxAnalyzer.ParseTree;

/// <summary>
/// A custom List that prints its contents when ToString() is called
/// </summary>
public class PrintableList<T> : List<T>
{
    public override string ToString()
    {
        if (Count == 0)
            return "[ ]";
        
        var items = new List<string>();
        foreach (var item in this)
        {
            items.Add(item?.ToString() ?? "null");
        }
        
        return "[ " + string.Join(", ", items) + " ]";
    }
}
