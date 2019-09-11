namespace UnityReferenceFinder.YamlParser.Lexers
{
    internal enum LexerType
    {
        None,
        EndOfFile,
        Identifier,
        Colon,
        FileSeparator,
        LineBreak,
        Dash,
        YamlDeclaration,
        TagDeclaration,
        WhiteSpace,
        Scalar,
        UnityDeclaration,
        UnityFileId,
        LeftBrace,    // {
        RightBrace,   // }
        Comma,
        LeftBracket,  // [
        RightBracket, // ]
    }
}