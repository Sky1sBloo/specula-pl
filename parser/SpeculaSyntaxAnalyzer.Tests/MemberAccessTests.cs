using NUnit.Framework;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;
using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class MemberAccessTests
{
    private ExpressionHandler exprHandler = null!;
    private ErrorsHandler errorsHandler = null!;

    [SetUp]
    public void SetUp()
    {
        errorsHandler = new();
        exprHandler = new(errorsHandler);
    }

    [Test]
    public void ChainedMemberAccess()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/Chained.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberAccessValue>());

        var level4 = (MemberAccessValue)node.node!; // ipv4
        Assert.That(level4.Member, Is.EqualTo("ipv4"));

        var level3 = (MemberAccessValue)level4.Object; // address
        Assert.That(level3.Member, Is.EqualTo("address"));

        var level2 = (MemberAccessValue)level3.Object; // client
        Assert.That(level2.Member, Is.EqualTo("client"));

        var level1 = (IdentifierValue)level2.Object; // network
        Assert.That(level1.Value, Is.EqualTo("network"));
    }

    [Test]
    public void SelfMemberAccess()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/Self.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberAccessValue>());

        var access = (MemberAccessValue)node.node!;
        Assert.That(access.Member, Is.EqualTo("url"));
        var obj = (IdentifierValue)access.Object;
        Assert.That(obj.Value, Is.EqualTo("self"));
    }

    [Test]
    public void ThisMemberAccess()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/This.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberAccessValue>());

        var access = (MemberAccessValue)node.node!;
        Assert.That(access.Member, Is.EqualTo("port"));
        var obj = (IdentifierValue)access.Object;
        Assert.That(obj.Value, Is.EqualTo("this"));
    }

    [Test]
    public void FunctionCallMemberAccess()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/FunctionCall.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberAccessValue>());

        var access = (MemberAccessValue)node.node!;
        Assert.That(access.Member, Is.EqualTo("url"));
        var func = (FunctionCallValue)access.Object;
        Assert.That(func.Identifier, Is.EqualTo("getServer"));
    }

    [Test]
    public void MemberFunctionCall()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/MemberFunctionCall.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberFunctionCallValue>());

        var call = (MemberFunctionCallValue)node.node!;
        Assert.That(call.Method, Is.EqualTo("send"));
        Assert.That(call.Parameters.Count, Is.EqualTo(0));
        var obj = (IdentifierValue)call.Object;
        Assert.That(obj.Value, Is.EqualTo("test"));
    }

    [Test]
    public void MemberFunctionCallWithParameters()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/MemberFunctionCallWithArgs.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberFunctionCallValue>());

        var call = (MemberFunctionCallValue)node.node!;
        Assert.That(call.Method, Is.EqualTo("setAge"));
        Assert.That(call.Parameters.Count, Is.EqualTo(1));
        
        var param = (LiteralValue)call.Parameters[0];
        Assert.That(param.Value, Is.EqualTo("25"));
        
        var obj = (IdentifierValue)call.Object;
        Assert.That(obj.Value, Is.EqualTo("user"));
    }

    [Test]
    public void ChainedMemberFunctionCalls()
    {
        var output = LexerFileReader.ParseFile("Samples/MemberAccess/ChainedMemberFunctionCalls.json");
        HandlerOutput node = exprHandler.HandleToken(output.Tokens, 0);

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<MemberFunctionCallValue>());

        var processCall = (MemberFunctionCallValue)node.node!;
        Assert.That(processCall.Method, Is.EqualTo("process"));
        Assert.That(processCall.Parameters.Count, Is.EqualTo(0));
        
        var getDataCall = (MemberFunctionCallValue)processCall.Object;
        Assert.That(getDataCall.Method, Is.EqualTo("getData"));
        Assert.That(getDataCall.Parameters.Count, Is.EqualTo(0));
        
        var userObj = (IdentifierValue)getDataCall.Object;
        Assert.That(userObj.Value, Is.EqualTo("user"));
    }
}
