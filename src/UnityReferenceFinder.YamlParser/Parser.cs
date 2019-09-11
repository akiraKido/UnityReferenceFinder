using System;
using System.Collections.Generic;
using UnityReferenceFinder.YamlParser.Lexers;
using UnityReferenceFinder.YamlParser.Nodes;

namespace UnityReferenceFinder.YamlParser
{
    public partial class UnityYamlTree
    {
        public YamlNode Parse(string text)
        {
            var lexer = new Lexer(text).Next();

            if (lexer.LexerType == LexerType.YamlDeclaration)
            {
                lexer = lexer.SkipWhiteSpace();
                lexer.ShouldBe(LexerType.Scalar);
                if (lexer.Value.IsMatch("1.1") == false) throw new Exception("supported YAML version: 1.1");
                lexer = lexer.Next().Skip(LexerType.LineBreak);
            }

            if (lexer.LexerType == LexerType.TagDeclaration)
            {
                // ignore tag...
                lexer = lexer.SkipUntil(LexerType.LineBreak).Skip(LexerType.LineBreak);
            }

            if (lexer.LexerType == LexerType.FileSeparator)
            {
                var files = new List<YamlNode>();

                while (lexer.LexerType != LexerType.LineBreak)
                {
                    lexer = ParseFile(lexer, out var node);
                    files.Add(node);
                }

                return new UnityScene(files);
            }

            if (lexer.LexerType == LexerType.Scalar)
            {
                ParseKeyValue(lexer, out var node);
                return node.value;
            }

            throw new NotImplementedException();
        }

        private static Lexer ParseKeyValue(Lexer lexer, out (string key, YamlNode value) result)
        {
            lexer = ParseScalar(lexer, out var rawKey);
            lexer = lexer.Skip(LexerType.Colon);

            var key = rawKey.ToString();

            if (lexer.LexerType == LexerType.LineBreak)
            {
                lexer = lexer.Skip(LexerType.LineBreak);

                if (lexer.SkipWhiteSpace().LexerType == LexerType.Dash)
                {
                    // list
                    lexer = ParseList(lexer, out var list);

                    result = (key, new YamlList(list));
                    return lexer;
                }
                else
                {
                    // object
                    lexer = ParseBlock(lexer, out var content);

                    result = (key, content);
                    return lexer;
                }
            }

            lexer = lexer.SkipWhiteSpace();
            

            if (lexer.LexerType == LexerType.LeftBracket)
            {
                // Literal list
                lexer = ParseLiteralList(lexer, out var content);
                result = (key, content);
                return lexer;
            }
            if (lexer.LexerType == LexerType.LeftBrace)
            {
                // Literal object
                lexer = ParseLiteralDictionary(lexer, out var content);

                result = (key, content);
                return lexer;
            }

            if (lexer.LexerType == LexerType.LineBreak)
            {
                // no value
                lexer = lexer.Skip(LexerType.LineBreak);
                
                result = (key, null);
                return lexer;
            }

            lexer = ParseScalar(lexer, out var value);

            result = (key, new YamlScalar(value));
            return lexer;
        }

        private static Lexer ParseScalar(Lexer lexer, out ReadOnlySpan<char> result)
        {
            lexer.ShouldBe(LexerType.Scalar);
            result = lexer.Value;
            return lexer.Next();
        }

        private static int CheckIndent(Lexer lexer)
        {
            lexer.ShouldBe(LexerType.WhiteSpace);
            return lexer.Length;
        }
    }
}