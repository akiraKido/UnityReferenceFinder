using System.Collections.Generic;
using UnityReferenceFinder.YamlParser.Lexers;
using UnityReferenceFinder.YamlParser.Nodes;

namespace UnityReferenceFinder.YamlParser
{
    public partial class UnityYamlTree
    {
        private static Lexer ParseBlock(Lexer lexer, out YamlObject result)
        {
            var indent = CheckIndent(lexer);

            var block = new Dictionary<string, YamlNode>();

            while (lexer.LexerType == LexerType.WhiteSpace && lexer.Length == indent)
            {
                lexer = lexer.SkipWhiteSpace();
                lexer = ParseKeyValue(lexer, out var keyValue);
                block[keyValue.key] = keyValue.value;

                if (lexer.LexerType == LexerType.EndOfFile)
                {
                    break;
                }

                if (lexer.LexerType == LexerType.LineBreak)
                {
                    lexer = lexer.Skip(LexerType.LineBreak);
                }
            }

            result = new YamlObject(block);
            return lexer;
        }

        private static Lexer ParseLiteralDictionary(Lexer lexer, out YamlObject result)
        {
            lexer = lexer.Skip(LexerType.LeftBrace);

            var block = new Dictionary<string, YamlNode>();

            while (lexer.LexerType != LexerType.RightBrace)
            {
                lexer = ParseKeyValue(lexer, out var keyValue);
                block[keyValue.key] = keyValue.value;

                if (lexer.LexerType == LexerType.Comma)
                {
                    lexer = lexer.Skip(LexerType.Comma);
                    if (lexer.LexerType == LexerType.LineBreak)
                    {
                        lexer = lexer.Skip(LexerType.LineBreak);
                    }

                    lexer = lexer.SkipWhiteSpace();
                }
            }

            lexer = lexer.Skip(LexerType.RightBrace);

            result = new YamlObject(block);
            return lexer;
        }
        
        private static Lexer ParseList(Lexer lexer, out IReadOnlyList<YamlNode> result)
        {
            var indent = CheckIndent(lexer);
            
            var list = new List<YamlNode>();

            while (lexer.LexerType == LexerType.WhiteSpace && lexer.Length == indent)
            {
                lexer = lexer.SkipWhiteSpace();
                
                lexer = ParseListEntry(lexer, out var listEntry);
                list.Add(listEntry);
            }

            result = list;
            return lexer;
        }

        private static Lexer ParseLiteralList(Lexer lexer, out YamlList result)
        {
            lexer = lexer.Skip(LexerType.LeftBracket);
            // ignore until we find an example...
            lexer = lexer.Skip(LexerType.RightBracket);
            result = new YamlList(new List<YamlNode>());
            return lexer;
        }

        private static Lexer ParseListEntry(Lexer lexer, out YamlObject result)
        {
            lexer = lexer.Skip(LexerType.Dash);

            var dict = new Dictionary<string, YamlNode>();

            while (lexer.SkipWhiteSpace().LexerType != LexerType.Dash)
            {
                lexer = lexer.SkipWhiteSpace();

                lexer = ParseKeyValue(lexer, out var keyValue);
                dict[keyValue.key] = keyValue.value;

                if (lexer.LexerType == LexerType.EndOfFile)
                {
                    break;
                }

                if (lexer.LexerType == LexerType.LineBreak)
                {
                    lexer = lexer.Skip(LexerType.LineBreak);
                }
            }

            result = new YamlObject(dict);
            return lexer;
        }
    }
}