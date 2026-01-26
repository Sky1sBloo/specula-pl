using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class ContractRoleHandlerTests
{
    private ContractRoleHandler roleHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        roleHandler = new(errorsHandler);
    }

    [Test]
    public void SingleRole()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/SingleRole.json");
        HandlerOutput node = roleHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<RolesNode>());
        
        var roles = (RolesNode)node.node!;
        Assert.That(roles.Roles.Count, Is.EqualTo(1));
        Assert.That(roles.Roles[0].Name, Is.EqualTo("Admin"));
    }

    [Test]
    public void MultipleRoles()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/MultipleRoles.json");
        HandlerOutput node = roleHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<RolesNode>());
        
        var roles = (RolesNode)node.node!;
        Assert.That(roles.Roles.Count, Is.EqualTo(2));
        Assert.That(roles.Roles[0].Name, Is.EqualTo("Robot"));
        Assert.That(roles.Roles[1].Name, Is.EqualTo("Controller"));
    }

    [Test]
    public void InvalidNoColon()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/Invalid/NoColon.json");
        Assert.Throws<SyntaxErrorException>(() => roleHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing colon");
    }

    [Test]
    public void InvalidMissingSemiColon()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/Invalid/MissingSemiColon.json");
        Assert.Throws<SyntaxErrorException>(() => roleHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing semicolon");
    }

    [Test]
    public void InvalidEmptyAfterColon()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/Invalid/EmptyAfterColon.json");
        Assert.Throws<SyntaxErrorException>(() => roleHandler.HandleToken(output.Tokens, 0),
            "Should throw error for empty roles after colon");
    }

    [Test]
    public void InvalidNoComma()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/Invalid/NoComma.json");
        Assert.Throws<SyntaxErrorException>(() => roleHandler.HandleToken(output.Tokens, 0),
            "Should throw error for missing comma between roles");
    }

    [Test]
    public void InvalidTrailingComma()
    {
        var output = LexerFileReader.ParseFile("Samples/Contract/Roles/Invalid/TrailingComma.json");
        Assert.Throws<SyntaxErrorException>(() => roleHandler.HandleToken(output.Tokens, 0),
            "Should throw error for trailing comma");
    }
}
