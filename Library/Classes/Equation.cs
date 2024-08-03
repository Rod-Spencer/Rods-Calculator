using Rod.Calculator.Library.Enumerations;

namespace Rod.Calculator.Library.Classes
{
    public class Equation
    {
        public Equation(Math_Symbols symbols)
        {
            Symbol = symbols;
        }

        public Equation(Decimal? left, Math_Symbols symbols)
        {
            Left = left;
            Symbol = symbols;
        }

        public Decimal? Left { get; set; }
        public Math_Symbols Symbol { get; set; }

        public Decimal? Right { get; set; }
    }
}
