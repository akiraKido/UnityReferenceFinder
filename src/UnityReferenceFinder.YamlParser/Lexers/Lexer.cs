using System;
using System.Collections.Generic;

namespace UnityReferenceFinder.YamlParser.Lexers
{
    internal readonly ref struct Lexer
    {
        private static readonly HashSet<char> SpecialCharacters = new HashSet<char> {':', '\r', '\n'};

        private static readonly IReadOnlyDictionary<char, LexerType> SingleTextTokens
            = new Dictionary<char, LexerType>
            {
                {':', LexerType.Colon},
                {'{', LexerType.LeftBrace},
                {'}', LexerType.RightBrace},
                {',', LexerType.Comma},
                {'[', LexerType.LeftBracket},
                {']', LexerType.RightBracket},
            };

        private readonly ReadOnlySpan<char> _text;
        private readonly int _offset;

        public readonly int Length;
        public readonly LexerType LexerType;
        public ReadOnlySpan<char> Value => _text.Slice(_offset, Length);

        public Lexer(string text)
        {
            _text = text.AsSpan();
            _offset = 0;
            Length = 0;
            LexerType = LexerType.None;
        }

        public Lexer(ReadOnlySpan<char> text, int offset, int length, LexerType lexerType)
        {
            _text = text;
            _offset = offset;
            Length = length;
            LexerType = lexerType;
        }

        public Lexer SkipWhiteSpace()
        {
            var next = Next();
            if (next.LexerType == LexerType.WhiteSpace)
            {
                next = next.Next();
            }

            return next;
        }

        public Lexer Skip(LexerType type)
        {
            if (LexerType != type)
            {
                throw new Exception($"[{GetCurrentPosition()}]expected {type} but found {LexerType}");
            }

            return Next();
        }

        private string GetCurrentPosition()
        {
            var line = _text.Slice(0, _offset).Count('\n') + 1;
            var pos = _text.BackTrackCount(_offset, '\n');
            return $"{line}:{pos}";
        }

        public Lexer SkipUntil(LexerType type)
        {
            var next = this;
            while ((next = next.Next()).LexerType != type) { }

            return next;
        }

        public Lexer Next()
        {
            var index = _offset + Length;
            if (index >= _text.Length)
            {
                return new Lexer(_text, index, 0, LexerType.EndOfFile);
            }

            if (SingleTextTokens.ContainsKey(_text[index]))
            {
                var type = SingleTextTokens[_text[index]];
                return new Lexer(_text, index, 1, type);
            }

            if (_text[index] == '%')
            {
                switch (_text[index + 1])
                {
                    case 'Y':
                        if (_text.Slice(index + 1, "YAML".Length).IsMatch("YAML") == false)
                            throw new Exception("invalid identifier");
                        return new Lexer(_text, index, "%YAML".Length, LexerType.YamlDeclaration);
                    case 'T':
                        if (_text.Slice(index + 1, "TAG".Length).IsMatch("TAG") == false)
                            throw new Exception("invalid identifier");
                        return new Lexer(_text, index, "%TAG".Length, LexerType.TagDeclaration);
                    default:
                        throw new Exception("invalid identifier");
                }
            }

            // File Separator
            if (_text[index] == '-')
            {
                if (index + 1 < _text.Length && char.IsLetterOrDigit(_text[index + 1]))
                {
                    // probably negative number
                }
                else
                {
                    if (index + 2 >= _text.Length || _text.Slice(index, 3).IsMatch("---") == false)
                    {
                        return new Lexer(_text, index, 1, LexerType.Dash);
                    }

                    return new Lexer(_text, index, 3, LexerType.FileSeparator);
                }
            }

            // Unity Declaration
            if (_text[index] == '!')
            {
                if (index + 2 >= _text.Length || _text.Slice(index, 3).IsMatch("!u!") == false)
                {
                    // Just a normal scalar
                }
                else
                {
                    var length = 3;
                    while (index + length < _text.Length && char.IsDigit(_text[index + length]))
                    {
                        length += 1;
                    }

                    return new Lexer(_text, index, length, LexerType.UnityDeclaration);
                }
            }

            // Unity file name
            if (_text[index] == '&')
            {
                var length = 1;
                while (index + length < _text.Length && char.IsDigit(_text[index + length]))
                {
                    length += 1;
                }

                return new Lexer(_text, index, length, LexerType.UnityFileId);
            }


            // Line Break
            if (_text[index] == '\r' || _text[index] == '\n')
            {
                var length = 1;
                if (_text[index] == '\r')
                {
                    if (index + 1 >= _text.Length)
                    {
                        return new Lexer(_text, index + 1, 0, LexerType.EndOfFile);
                    }

                    if (_text[index + 1] != '\n')
                    {
                        throw new NotSupportedException("\\r must be followed by \\n");
                    }

                    length = 2;
                }

                return new Lexer(_text, index, length, LexerType.LineBreak);
            }

            // White space
            if (char.IsWhiteSpace(_text[index]))
            {
                var length = 1;
                while (index + length < _text.Length 
                       && char.IsWhiteSpace(_text[index + length])
                       && _text[index + length] != '\n')
                {
                    length += 1;
                }

                return new Lexer(_text, index, length, LexerType.WhiteSpace);
            }

            // Scalar values
            {
                var length = 0;
                while (index + length < _text.Length && IsScalarLetter(_text[index + length]))
                {
                    length += 1;
                }

                return new Lexer(_text, index, length, LexerType.Scalar);
            }
        }

        private static bool IsScalarLetter(char c)
            => char.IsLetterOrDigit(c)
               || c == '_'
               || c == '.'
               || c == '-';

        public void ShouldBe(LexerType lexerType)
        {
            if (LexerType != lexerType)
                throw new Exception($"[{GetCurrentPosition()}] expected: {lexerType}, but was {LexerType}");
        }
    }
}