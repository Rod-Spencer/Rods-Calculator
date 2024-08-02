using Prism.Commands;
using Prism.Mvvm;
using Rod.Calculator.Library.Classes;
using Rod.Calculator.Library.Enumerations;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;

namespace Rod.Calculator.Standard.ViewModels
{
    public class Standard_ViewModel : BindableBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructors

        public Standard_ViewModel()
        {
            ButtonClickCommand = new DelegateCommand<Object>(CommandButtonClick);
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Static Properties/Members
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Static Properties/Members
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Properties

        private Double MemoryStore = 0;
        private Double _ResultPrev = 0;
        private Double? _Result = 0;
        private String _Current = String.Empty;
        private Math_Symbols mathSymbol = Math_Symbols.None;
        private Equation? equation = null;
        private List<Equation>? _fullEquation = null;

        private List<Equation>? fullEquation
        {
            get
            {
                if (_fullEquation == null) { _fullEquation = new List<Equation>(); }
                return _fullEquation;
            }
            set { _fullEquation = value; }
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Properties

        //#region ResultString


        ///// <summary>Property ResultString of type String?</summary>
        //public String? ResultString
        //{
        //    get => $"{_Result}";
        //    set
        //    {
        //        Double d = 0;
        //        Double.TryParse((String?)value, out d);
        //        SetProperty<Double?>(ref _Result, d);
        //    }
        //}
        //#endregion

        #region ResultString

        private String? _ResultString;

        /// <summary>Property ResultString of type String?</summary>
        public String? ResultString
        {
            get => $"{_Result}";
            set
            {
                Double d = 0;
                if (Double.TryParse(value, out d) == true) _Result = d;
                SetProperty<String?>(ref _ResultString, value);
            }
        }
        #endregion


        #region EquationString

        /// <summary>Property EquationString of type String?</summary>
        public String? EquationString
        {
            get
            {
                if ((fullEquation == null) || (fullEquation.Count == 0)) return String.Empty;
                StringBuilder? sb = null;
                fullEquation.ForEach(e =>
                {
                    if (e.Left != null)
                    {
                        sb = new StringBuilder();
                        sb.Append($"{e.Left} {e.Symbol} {e.Right}");
                    }
                    else
                    {
                        sb.Insert(0, "(");
                        sb.Append($") {e.Symbol} {e.Right}");
                    }
                });
                return sb.ToString();
            }
            //set { SetProperty<String?>(ref _EquationString, value); }
        }
        #endregion

        #region DivisionSign

        /// <summary>Property DivisionSign of type String</summary>
        public String DivisionSign
        {
            get { return ((Char)Math_Symbols.Divide).ToString(); }
        }
        #endregion

        #region MultiplySign

        /// <summary>Property MultiplySign of type String</summary>
        public String MultiplySign
        {
            get { return ((Char)Math_Symbols.Multiply).ToString(); }
        }
        #endregion

        #region MinusSign

        /// <summary>Property MinusSign of type String</summary>
        public String MinusSign
        {
            get { return ((Char)Math_Symbols.Minus).ToString(); }
        }
        #endregion

        #region PlusSign

        /// <summary>Property PlusSign of type String</summary>
        public String PlusSign
        {
            get { return ((Char)Math_Symbols.Plus).ToString(); }
        }
        #endregion

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Static Methods
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Static Methods
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Methods
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Private Methods

        private Double? Equate_Equation()
        {
            Double? result = 0;
            foreach (var e in fullEquation)
            {
                Double? right = e.Right;

                if (e.Left != null)
                {
                    Double? left = e.Left;

                    if (e.Symbol == Math_Symbols.Minus)
                    {
                        result = e.Left - e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Plus)
                    {
                        result = e.Left + e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Multiply)
                    {
                        result = e.Left * e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Divide)
                    {
                        if ((e.Right != null) && (e.Right != 0))
                        {
                            result = e.Left / e.Right;
                        }
                        else
                        {
                            throw new DivideByZeroException();
                        }
                    }
                }
                else
                {
                    if (e.Symbol == Math_Symbols.Minus)
                    {
                        result -= e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Plus)
                    {
                        result += e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Multiply)
                    {
                        result *= e.Right;
                    }
                    else if (e.Symbol == Math_Symbols.Divide)
                    {
                        if ((e.Right != null) && (e.Right != 0))
                        {
                            result /= e.Right;
                        }
                        else
                        {
                            throw new DivideByZeroException();
                        }
                    }
                }
            }
            return result;
        }


        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Command Handlers

        #region ButtonClickCommand
        /////////////////////////////////////////////

        /// <summary>Delegate Command: ButtonClickCommand</summary>
        public ICommand ButtonClickCommand { get; set; }

        private void CommandButtonClick(Object sender)
        {
            try
            {
                String s = (String)sender;
                if (s == null) { return; }
                if (s.StartsWith("Number") == true)
                {
                    _Current += s.Replace("Number", "");
                    ResultString = _Current;
                }
                else if (s == "Decimal")
                {
                    if (_Current.Contains(".") == false)
                    {
                        _Current += ".";
                    }
                }
                else if (s == "Equal")
                {
                    if (equation != null)
                    {
                        equation.Right = _Result;
                        _Result = Equate_Equation();
                        ResultString = ResultString;
                    }
                }
                else if (s.StartsWith("Math") == true)
                {
                    if (Enum.TryParse<Math_Symbols>(s.Substring(4), out mathSymbol) == true)
                    {
                        if (equation == null)
                        {
                            equation = new Equation(_Result, mathSymbol);
                        }
                        fullEquation?.Add(equation);
                    }
                }
                else if (s == "Percent")
                {
                    _Result = _Result / 100.0;
                    ResultString = ResultString;
                }
                else if (s == "Invert")
                {
                    _Result = 1 / _Result;
                    ResultString = ResultString;
                }
                else if (s == "SqRt")
                {
                    if (_Result != null)
                    {
                        _Result = Math.Sqrt((Double)_Result);
                        _Current = _Result?.ToString();
                        ResultString = _Current;
                    }
                }
                else if (s == "Sqr")
                {
                    if (_Result != null)
                    {
                        _Result = _Result * _Result;
                        ResultString = ResultString;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /////////////////////////////////////////////
        #endregion


        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Event Handlers
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
