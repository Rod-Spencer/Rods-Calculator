using Rod.Calculator.Library.Enumerations;

namespace Rod.Calculator.Library.Classes
{
    public class Equation
    {
        public Equation(Math_Symbols symbols, Double? right)
        {
            Symbol = symbols;
            Right = right;
        }

        public Equation(Double? left, Math_Symbols symbols, Double? right = null)
        {
            Left = left;
            Symbol = symbols;
            Right = right;
        }

        public Double? Left { get; set; }
        public Math_Symbols Symbol { get; set; }

        public Double? Right { get; set; }
    }
}
