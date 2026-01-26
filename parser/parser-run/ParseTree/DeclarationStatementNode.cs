using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.ParseTree;

public record DeclarationStatementNode(string Identifier, TypeDefinitionNode VarDefinition, Expression Value) : Statement, RootStatement, ForInit, ListenerBody;
