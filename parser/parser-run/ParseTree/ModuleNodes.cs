
namespace SpeculaSyntaxAnalyzer.ParseTree;

public interface ModuleNode : RootStatement;

public interface ExportModuleNode : ModuleNode;
public record ExportNode(RootStatement Statement) : ExportModuleNode;
public record ExportDefaultNode(RootStatement Statement) : ExportModuleNode;

public interface ImportModuleNode : ModuleNode;
public record ImportNodes(PrintableList<ImportModuleNode> Imports) : ModuleNode;
public record ImportAliasNode(string ModuleName, string Alias) : ImportModuleNode;
public record ImportNode(string ModuleName, PrintableList<string> Identifiers) : ImportModuleNode;