using System.Collections.Generic;
using UnityReferenceFinder.YamlParser.Lexers;
using UnityReferenceFinder.YamlParser.Nodes;

namespace UnityReferenceFinder.YamlParser
{
    public partial class UnityYamlTree
    {
        private Lexer ParseFile(Lexer lexer, out YamlNode result)
        {
            lexer.ShouldBe(LexerType.FileSeparator);

            lexer = ParseUnityDeclaration(lexer, out var unityId);
            lexer = ParseFileId(lexer, out var fileId);

            if (lexer.LexerType == LexerType.WhiteSpace)
            {
                // ignore stripped
                lexer = lexer.SkipUntil(LexerType.LineBreak);
            }
            
            lexer = lexer.Skip(LexerType.LineBreak);

            var block = new Dictionary<string, YamlNode>();

            // TODO iterate?
            lexer = ParseScalar(lexer, out var blockName);
            lexer = lexer.Skip(LexerType.Colon);
            lexer = lexer.Skip(LexerType.LineBreak);
            lexer = ParseBlock(lexer, out var blockContent);

            block.Add(blockName.ToString(), blockContent);

            result = new UnityYamlFile(unityId, fileId, block);
            return lexer;
        }

    }
}