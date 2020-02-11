namespace LogAnalyzer.Analyzers.Bookings.Parsers {
    public interface IParser<T> {
        bool Parse(string text);
        T Output { get; }
    }
}
