using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Rod.Calculator.Library.Classes;
using Rod.Calculator.Library.Enumerations;
using Rod.Calculator.Library.Events;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace Rod.Calculator.Standard.ViewModels
{
    public class Standard_ViewModel : BindableBase
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Constructors

        public Standard_ViewModel(IEventAggregator eventAggregator)
        {
            #region Event Subscriptions

            eventAggregator.GetEvent<Keyboard_Released_Event>().Subscribe(Keyboard_Released_Handler, ThreadOption.BackgroundThread, true);

            #endregion

            #region Command Delegates

            ButtonClickCommand = new DelegateCommand<Object>(CommandButtonClick);
            MemoryClickCommand = new DelegateCommand<Object>(CommandMemoryClick);
            MemoryRecallCommand = new DelegateCommand(CommandMemoryRecall, CanCommandMemoryRecall);
            MemoryClearCommand = new DelegateCommand(CommandMemoryClear, CanCommandMemoryClear);

            #endregion
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

        private List<String> _AllActions;

        private List<String> AllEntries
        {
            get
            {
                if (_AllActions == null) _AllActions = new List<String>();
                return _AllActions;
            }
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Public Properties

        ////////////////////////////////////////////////////////////////////
        // The following block of code are the binding for the XAML view
        ////////////////////////////////////////////////////////////////////

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

        #region EquationForeground

        private Brush _EquationForeground = Brushes.LightGray;

        /// <summary>Property EquationForeground of type Brush</summary>
        public Brush EquationForeground
        {
            get => _EquationForeground;
            set => SetProperty<Brush>(ref _EquationForeground, value);
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

        /// <summary>Method to go through the entries list to generate the result</summary>
        /// <returns>Decimal - result</returns>
        /// <exception cref="Exception"></exception>
        private Decimal Calculate_Result()
        {
            Decimal result = 0;
            String number = String.Empty;
            Math_Symbols symbol = Math_Symbols.None;

            // Iterate through the entries list
            foreach (var act in AllEntries)
            {
                if (act.StartsWith("Number", StringComparison.OrdinalIgnoreCase) == true)
                {
                    number += act.Replace("Number", "");
                }
                else if (act.StartsWith("MR:") == true)
                {
                    number = act.Substring(3);
                }
                else if (String.Compare(act, "Decimal", true) == 0)
                {
                    if (number.Contains('.') == false)
                    {
                        number += ".";
                    }
                }
                else if (String.Compare(act, "BackSp", true) == 0)
                {
                    if (number.Length > 0)
                    {
                        number = number.Substring(0, number.Length - 1);
                    }
                }
                else if (String.Compare(act, "Sign", true) == 0)
                {
                    if (number.Length > 0)
                    {
                        if (Decimal.TryParse(number, out Decimal d) == true)
                        {
                            d = 0 - d;
                            number = $"{d}";
                        }
                    }
                    else if (result != 0)
                    {
                        result = 0 - result;
                    }
                }
                else if (act.StartsWith("Math", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (symbol == Math_Symbols.Plus)
                        {
                            result += Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Minus)
                        {
                            result -= Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Multiply)
                        {
                            result *= Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Divide)
                        {
                            result /= Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.None)
                        {
                            result = Decimal.Parse(number);
                        }
                    }
                    symbol = Enum.Parse<Math_Symbols>(act.Substring(4));
                    EquationString = Calculate_Equation();
                    number = String.Empty;
                }
                else if (String.Compare(act, "Equal", true) == 0)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (symbol == Math_Symbols.Plus)
                        {
                            result += Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Minus)
                        {
                            result -= Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Multiply)
                        {
                            result *= Decimal.Parse(number);
                        }
                        else if (symbol == Math_Symbols.Divide)
                        {
                            result /= Decimal.Parse(number);
                        }
                    }
                    symbol = Math_Symbols.None;
                    EquationString = Calculate_Equation();
                    number = String.Empty;
                }
                else if (act.StartsWith("Invert", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (Decimal.TryParse(number, out Decimal d) == false)
                        {
                            throw new Exception($"Unable to parse: {number}");
                        }
                        d = 1 / d;
                        number = $"{d}";
                    }
                    else
                    {
                        result = 1 / result;
                        number = String.Empty;
                    }
                    symbol = Math_Symbols.None;
                    EquationString = Calculate_Equation();
                }
                else if (act.StartsWith("Percent", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (Decimal.TryParse(number, out Decimal d) == false)
                        {
                            throw new Exception($"Unable to parse: {number}");
                        }
                        d /= 100;
                        number = $"{d}";
                    }
                    else
                    {
                        result /= 100;
                        number = String.Empty;
                    }
                    symbol = Math_Symbols.None;
                    EquationString = Calculate_Equation();
                }
                else if (act.StartsWith("SqRt", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (Decimal.TryParse(number, out Decimal d) == false)
                        {
                            throw new Exception($"Unable to parse: {number}");
                        }
                        d = (Decimal)Math.Sqrt((Double)d);
                        number = $"{d}";
                    }
                    else
                    {
                        result = (Decimal)Math.Sqrt((Double)result);
                        number = String.Empty;
                    }
                    symbol = Math_Symbols.None;
                    EquationString = Calculate_Equation();
                }
                else if (act.StartsWith("Sqr", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if ((String.IsNullOrEmpty(number) == false) && (number.Length > 0))
                    {
                        if (Decimal.TryParse(number, out Decimal d) == false)
                        {
                            throw new Exception($"Unable to parse: {number}");
                        }
                        d = (Decimal)Math.Pow((Double)d, 2);
                        number = $"{d}";
                    }
                    else
                    {
                        result = (Decimal)Math.Pow((Double)result, 2);
                        number = String.Empty;
                    }
                    symbol = Math_Symbols.None;
                    EquationString = Calculate_Equation();
                }
            }
            if (String.IsNullOrEmpty(number) == false) return Decimal.Parse(number);
            return result;
        }

        /// <summary>Method to iterate through the action list to generate a string representation</summary>
        /// <returns>String? - string representation </returns>
        private String Calculate_Equation()
        {
            StringBuilder result = new StringBuilder();

            // Iterate through the entries list
            foreach (var act in AllEntries)
            {
                if (act.StartsWith("Number", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result.Append(act.Replace("Number", ""));
                }
                else if (act.StartsWith("MR:", StringComparison.OrdinalIgnoreCase) == true)
                {
                    result.Append(act.Substring(3));
                }
                else if (String.Compare(act, "Decimal", true) == 0)
                {
                    result.Append(".");
                }
                else if (act.StartsWith("Math", StringComparison.OrdinalIgnoreCase) == true)
                {
                    if (Enum.TryParse<Math_Symbols>(act.Substring(4), out Math_Symbols mathSymbol) == true)
                    {
                        result.Insert(0, $"(");
                        result.Append($"){(Char)mathSymbol}");
                    }
                }

                else if (String.Compare(act, "SqRt", true) == 0)
                {
                    result.Insert(0, $"{act}(");
                    result.Append(")");
                }
                else if (String.Compare(act, "Invert", true) == 0)
                {
                    result.Insert(0, $"Inv(");
                    result.Append(")");
                }
                else if (String.Compare(act, "Sqr", true) == 0)
                {
                    result.Insert(0, $"{act}(");
                    result.Append(")");
                }
                else if (String.Compare(act, "Percent", true) == 0)
                {
                    result.Append("%");
                }
                else if (String.Compare(act, "Sign", true) == 0)
                {
                    result.Insert(0, "-(");
                    result.Append(")");
                }
            }
            return result.ToString();
        }

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Command Handlers

        #region ButtonClickCommand
        /////////////////////////////////////////////

        /// <summary>Delegate Command: ButtonClickCommand</summary>
        public ICommand ButtonClickCommand { get; set; }

        /// <summary>Main handler for Button clicks and key presses</summary>
        /// <param name="sender"></param>
        /// <exception cref="DivideByZeroException"></exception>
        private void CommandButtonClick(Object sender)
        {
            try
            {
                String s = (String)sender;
                if (String.IsNullOrEmpty(s) == true)
                {
                    return;
                }
                else if (s == "ClearEntry")
                {
                    _Result = 0;
                    ResultString = String.Empty;
                    return;
                }

                // Checks if the Clear (C) button has been entered
                else if (s == "Clear")
                {
                    _Result = 0;
                    ResultString = String.Empty;
                    AllEntries.Clear();
                    EquationString = String.Empty;
                    EquationForeground = Brushes.LightGray;
                    return;
                }
                AllEntries.Add(s);


                ResultString = $"{Calculate_Result()}";
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                EquationForeground = Brushes.Red;
                EquationString = ex.Message;
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
                AllEntries.Add($"MR:{MemoryStore}");

                ResultString = $"{Calculate_Result()}";
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


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #region Keyboard_Released_Handler  -- Event: Keyboard_Released_Event Handler
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        private void Keyboard_Released_Handler(String obj)
        {
            CommandButtonClick(obj);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
