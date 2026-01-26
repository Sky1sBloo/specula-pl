using SpeculaSyntaxAnalyzer.SyntaxAnalyzer;
using SpeculaSyntaxAnalyzer.LexerReader;
using SpeculaSyntaxAnalyzer.ParseTree;

namespace SpeculaSyntaxAnalyzer.Tests;

[TestFixture]
public class CapabilityTests
{
    private CapabilityHandler capabilityHandler;
    private ErrorsHandler errorsHandler;

    [SetUp]
    public void Setup()
    {
        errorsHandler = new();
        capabilityHandler = new(errorsHandler);
    }

    [Test]
    public void SingleCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/SingleCapability.json");
        HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<Capabilities>());
        
        var capabilities = (Capabilities)node.node!;
        Assert.That(capabilities.CapabilityList.Count, Is.EqualTo(1));
        Assert.That(capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));
        Assert.That(capabilities.CapabilityList[0].Configuration.Count, Is.EqualTo(0));
    }

    [Test]
    public void MultipleCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/MultipleCapability.json");
        HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<Capabilities>());
        
        var capabilities = (Capabilities)node.node!;
        Assert.That(capabilities.CapabilityList.Count, Is.EqualTo(2));
        Assert.That(capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));
        Assert.That(capabilities.CapabilityList[0].Configuration.Count, Is.EqualTo(0));
        Assert.That(capabilities.CapabilityList[1].Type, Is.EqualTo(CapabilityTypes.MUT));
        Assert.That(capabilities.CapabilityList[1].Configuration.Count, Is.EqualTo(0));
    }

    [Test]
    public void CapabilityWithSettings()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/CapabilityWithSettings.json");
        HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(0));
        Assert.That(node.node, Is.TypeOf<Capabilities>());
        
        var capabilities = (Capabilities)node.node!;
        Assert.That(capabilities.CapabilityList.Count, Is.EqualTo(3));
        
        Assert.That(capabilities.CapabilityList[0].Type, Is.EqualTo(CapabilityTypes.OWN));
        Assert.That(capabilities.CapabilityList[0].Configuration.Count, Is.EqualTo(0));
        
        Assert.That(capabilities.CapabilityList[1].Type, Is.EqualTo(CapabilityTypes.NETWORK));
        Assert.That(capabilities.CapabilityList[1].Configuration.Count, Is.EqualTo(1));
        Assert.That(capabilities.CapabilityList[1].Configuration[0], Is.EqualTo("json"));
        
        Assert.That(capabilities.CapabilityList[2].Type, Is.EqualTo(CapabilityTypes.SHARED));
        Assert.That(capabilities.CapabilityList[2].Configuration.Count, Is.EqualTo(1));
        Assert.That(capabilities.CapabilityList[2].Configuration[0], Is.EqualTo("this"));
    }

    [Test]
    public void EmptyCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/Invalid/EmptyCapability.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void TrailingComma()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/Invalid/TrailingComma.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void UnknownCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/Invalid/UnknownCapability.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }

    [Test]
    public void StackedCapability()
    {
        var output = LexerFileReader.ParseFile("Samples/Capability/Invalid/StackedCapability.json");
        Assert.Throws<SyntaxErrorException>(() => 
        {
            HandlerOutput node = capabilityHandler.HandleToken(output.Tokens, 0);
        });

        Assert.That(errorsHandler.ErrorList.Count, Is.EqualTo(1));
    }
}
