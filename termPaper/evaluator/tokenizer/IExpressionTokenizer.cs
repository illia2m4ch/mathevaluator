namespace termPaper.evaluator.tokenizer; 

public interface IExpressionTokenizer {
    Token Next();
    bool HasNext();

    public class Token {
        public readonly int Type;
        public readonly string Raw;

        public Token(int type, string raw) {
            Type = type;
            Raw = raw;
        }
    }
}