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
            MemoryClickCommand = new DelegateCommand<Object>(CommandMemoryClick);
            MemoryRecallCommand = new DelegateCommand(CommandMemoryRecall, CanCommandMemoryRecall);
            MemoryClearCommand = new DelegateCommand(CommandMemoryClear, CanCommandMemoryClear);
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

        private Decimal? MemoryStore = null;
        private Decimal? _Result = 0;
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
        //        Decimal d = 0;
        //        Decimal.TryParse((String?)value, out d);
        //        SetProperty<Decimal?>(ref _Result, d);
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
                Decimal d = 0;
                if (Decimal.TryParse(value, out d) == true) _Result = d;

                if (value?.Length > 16)
                {
                    d = Math.Round(d, 14);
                    String IntegerStr = ((int)d).ToString();
                    String DecimalStr = (d - ((int)d)).ToString();
                    if (DecimalStr.Substring(1).Length > 0)
                    {
                        DecimalStr = DecimalStr.Substring(2);
                        while ((DecimalStr.Length > 0) && ((IntegerStr.Length + DecimalStr.Length) > 16))
                        {
                            DecimalStr = DecimalStr.Substring(0, DecimalStr.Length - 1);
                        }
                        while ((DecimalStr.Length > 0) && (DecimalStr[DecimalStr.Length - 1] == '0'))
                        {
                            DecimalStr = DecimalStr.Substring(0, DecimalStr.Length - 1);
                        }
                        String s = $"{IntegerStr}.{DecimalStr}";
                        _Result = Decimal.Parse(s);
                        SetProperty<String?>(ref _ResultString, s);
                        return;
                    }
                }
                SetProperty<String?>(ref _ResultString, value);
            }
        }
        #endregion


        #region EquationString

        String? _EquationString;

        /// <summary>Property EquationString of type String?</summary>
        public String? EquationString
        {
            get => _EquationString;
            set => SetProperty<String?>(ref _EquationString, value);
        }
        #endregion

        #region DivisionSign

        /// <summary>Property DivisionSign of type String</summary>
        public String DivisionSign
        {
            get => $"{(Char)Math_Symbols.Divide}";
        }
        #endregion

        #region MultiplySign

        /// <summary>Property MultiplySign of type String</summary>
        public String MultiplySign
        {
            get => $"{(Char)Math_Symbols.Multiply}";
        }
        #endregion

        #region MinusSign

        /// <summary>Property MinusSign of type String</summary>
        public String MinusSign
        {
            get => $"{(Char)Math_Symbols.Minus}";
        }
        #endregion

        #region PlusSign

        /// <summary>Property PlusSign of type String</summary>
        public String PlusSign
        {
            get => $"{(Char)Math_Symbols.Plus}";
        }
        #endregion

        #region SignToggle

        /// <summary>Property SignToggle of type String</summary>
        public String SignToggle
        {
            get => $"{(Char)Math_Symbols.Sign}";
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

        private string? Generate_Equation()
        {
            if ((fullEquation == null) || (fullEquation.Count == 0)) return String.Empty;
            StringBuilder? sb = new StringBuilder();
            fullEquation.ForEach(e =>
            {
                if (e.Symbol == Math_Symbols.Sqr)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, "Sqr(");
                        sb.Append(")");
                    }
                }
                else if (e.Symbol == Math_Symbols.SqRt)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, "SqRt(");
                        sb.Append(")");
                    }
                }
                else if (e.Symbol == Math_Symbols.Inv)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, "Inv(");
                        sb.Append(")");
                    }
                }
                else if (e.Symbol == Math_Symbols.Sign)
                {
                    if (sb.Length > 0)
                    {
                        sb.Insert(0, $"{(Char)e.Symbol}(");
                        sb.Append(")");
                    }
                }
                else if (e.Left != null)
                {
                    sb.Append($"{e.Left} {(Char)e.Symbol} {e.Right}");
                }
                else if (sb != null)
                {
                    sb.Insert(0, "(");
                    sb.Append($") {(Char)e.Symbol}");
                    if (e.Right.HasValue == true) sb.Append($" {e.Right}");
                }
            });
            return sb.ToString();
        }


        private Decimal? Equate_Equation()
        {
            Decimal? result = 0;
            if (fullEquation != null)
            {
                foreach (var e in fullEquation)
                {
                    Decimal? right = e.Right;

                    if (e.Left != null)
                    {
                        Decimal? left = e.Left;

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
                        EquationString = Generate_Equation();
                        equation = null;
                    }
                }
                else if (s.StartsWith("Math") == true)
                {
                    if (Enum.TryParse<Math_Symbols>(s.Substring(4), out mathSymbol) == true)
                    {
                        if (equation != null)
                        {
                            equation.Right = _Result;
                            equation = new Equation(mathSymbol);
                        }
                        else if (fullEquation.Count == 0)
                        {
                            equation = new Equation(_Result, mathSymbol);
                        }
                        else
                        {
                            equation = new Equation(mathSymbol);
                        }
                        fullEquation?.Add(equation);

                        _Result = 0;
                        _Current = String.Empty;
                        EquationString = Generate_Equation();
                    }
                }
                else if (s == "Percent")
                {
                    if (_Result.HasValue == true) _Result = (Decimal?)(((Double)_Result) / 100.0);
                    ResultString = ResultString;
                    fullEquation?.Add(new Equation(Math_Symbols.Pcnt));
                }
                else if (s == "Invert")
                {
                    _Result = 1 / _Result;
                    ResultString = ResultString;
                    fullEquation?.Add(new Equation(Math_Symbols.Inv));
                }
                else if (s == "SqRt")
                {
                    if (_Result != null)
                    {
                        _Result = (Decimal?)Math.Sqrt((Double)_Result);
                        if (_Result.HasValue == true) _Current = $"{_Result}";
                        ResultString = _Current;
                        fullEquation?.Add(new Equation(Math_Symbols.SqRt));
                    }
                }
                else if (s == "Sqr")
                {
                    if (_Result != null)
                    {
                        _Result = _Result * _Result;
                        ResultString = ResultString;
                        fullEquation?.Add(new Equation(Math_Symbols.Sqr));
                    }
                }
                else if (s == "ClearEntry")
                {
                    _Result = 0;
                    _Current = String.Empty;
                    _ResultString = String.Empty;
                    ResultString = ResultString;
                }
                else if (s == "Clear")
                {
                    _Result = 0;
                    _Current = String.Empty;
                    _ResultString = String.Empty;
                    ResultString = ResultString;
                    fullEquation?.Clear();
                    equation = null;
                    EquationString = String.Empty;
                }
                else if (s == "Sign")
                {
                    _Result = 0 - _Result;
                    ResultString = ResultString;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /////////////////////////////////////////////
        #endregion


        #region MemoryClickCommand
        /////////////////////////////////////////////

        /// <summary>Delegate Command: MemoryClickCommand</summary>
        public ICommand MemoryClickCommand { get; set; }
        private Boolean CanCommandMemoryClick()
        {
            return true;
        }

        private void CommandMemoryClick(Object sender)
        {
            try
            {
                String s = (String)sender;
                if (s == null) { return; }
                if (s.StartsWith("MemAdd") == true)
                {
                    if (MemoryStore.HasValue == false) MemoryStore = 0;
                    MemoryStore += _Result;
                }
                else if (s.StartsWith("MemSub") == true)
                {
                    if (MemoryStore.HasValue == false) MemoryStore = 0;
                    MemoryStore -= _Result;
                }
                else if (s.StartsWith("MemStore") == true)
                {
                    MemoryStore = _Result;
                }
                MemoryRecallCommand.RaiseCanExecuteChanged();
                MemoryClearCommand.RaiseCanExecuteChanged();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /////////////////////////////////////////////
        #endregion


        #region MemoryRecallCommand
        /////////////////////////////////////////////

        /// <summary>Delegate Command: MemoryRecallCommand</summary>
        public DelegateCommand MemoryRecallCommand { get; set; }
        private Boolean CanCommandMemoryRecall()
        {
            return MemoryStore.HasValue;
        }

        private void CommandMemoryRecall()
        {
            try
            {
                _Result = MemoryStore;
                ResultString = ResultString;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        /////////////////////////////////////////////
        #endregion


        #region MemoryClearCommand
        /////////////////////////////////////////////

        /// <summary>Delegate Command: MemoryClearCommand</summary>
        public DelegateCommand MemoryClearCommand { get; set; }

        private Boolean CanCommandMemoryClear()
        {
            return MemoryStore.HasValue;
        }

        private void CommandMemoryClear()
        {
            try
            {
                MemoryStore = null;
                MemoryRecallCommand.RaiseCanExecuteChanged();
                MemoryClearCommand.RaiseCanExecuteChanged();
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
