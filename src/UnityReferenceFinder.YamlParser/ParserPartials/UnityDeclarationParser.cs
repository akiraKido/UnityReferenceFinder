using System;
using UnityReferenceFinder.YamlParser.Lexers;

namespace UnityReferenceFinder.YamlParser
{
    public partial class UnityYamlTree
    {
        private static Lexer ParseUnityDeclaration(Lexer lexer, out ReadOnlySpan<char> value)
        {
            lexer = lexer.SkipWhiteSpace();
            lexer.ShouldBe(LexerType.UnityDeclaration);
            value = lexer.Value.Slice(3);
            return lexer.Next();
        }

        private static Lexer ParseFileId(Lexer lexer, out ReadOnlySpan<char> value)
        {
            lexer = lexer.SkipWhiteSpace();
            lexer.ShouldBe(LexerType.UnityFileId);
            value = lexer.Value.Slice(1);
            return lexer.Next();
        }
    }
}