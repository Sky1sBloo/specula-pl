namespace SpeculaSyntaxAnalyzer.ParseTree;

public enum ListenerEventType
{
    BEFORE,
    AFTER
}
public record ListenerEventNode(string Name, ListenerEventType EventType, BodyNode Body) : ListenerBody;

public record ListenerFailEventNode(string Name, FuncParams Parameters, BodyNode Body) : ListenerBody;