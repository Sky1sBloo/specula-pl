namespace SpeculaSyntaxAnalyzer.ParseTree;


public record ListenerMessageEventNode(string Name, FuncParams Parameters, BodyNode Body) : ListenerBody;

public record ListenerEventRespondNode(string Name, PrintableList<Expression> Arguments) : Statement;
public record ListenerFail(string Name, PrintableList<Expression> Arguments) : Statement;