using FluentAssertions;
using UnityReferenceFinder.YamlParser.Lexers;
using Xunit;

namespace UnityReferenceFinder.YamlParser.Tests
{
    public class LexerTests
    {
        [Fact]
        public void InternalTest()
        {
            var text = @"
GameObject:
  m_test: 10
".Trim();
            var lexer = new Lexer(text);
            lexer = lexer.Next();
            lexer.Value.ToString().Should().Be("GameObject");
            lexer.LexerType.Should().Be(LexerType.Scalar);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.Colon);

            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.LineBreak);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.WhiteSpace);
            lexer.Length.Should().Be(2);
            
            lexer = lexer.Next();
            lexer.Value.ToString().Should().Be("m_test");
            lexer.LexerType.Should().Be(LexerType.Scalar);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.Colon);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.WhiteSpace);
            
            lexer = lexer.Next();
            lexer.Value.ToString().Should().Be("10");
            lexer.LexerType.Should().Be(LexerType.Scalar);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.EndOfFile);
        }

        [Fact]
        public void FileSeparatorTest()
        {
            var text = @"
--- !u!29 &1
OcclusionCullingSettings:
  m_ObjectHideFlags: 0
  serializedVersion: 2
".Trim();
            var lexer = new Lexer(text);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.FileSeparator);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.WhiteSpace);
            lexer.Length.Should().Be(1);

            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.UnityDeclaration);
            lexer.Value.ToString().Should().Be("!u!29");

            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.WhiteSpace);
            lexer.Length.Should().Be(1);
            
            lexer = lexer.Next();
            lexer.Value.ToString().Should().Be("&1");
            lexer.LexerType.Should().Be(LexerType.UnityFileId);
            
            lexer = lexer.Next();
            lexer.LexerType.Should().Be(LexerType.LineBreak);

            while (lexer.LexerType != LexerType.EndOfFile)
            {
                lexer = lexer.Next();
            }
        }

        [Theory]
        [InlineData("0")]
        [InlineData("0.1")]
        [InlineData("test")]
        [InlineData("m_test")]
        public void ScalarLexingShouldWork(string text)
        {
            var lexer = new Lexer(text).Next();
            lexer.LexerType.Should().Be(LexerType.Scalar);
        }
    }

}