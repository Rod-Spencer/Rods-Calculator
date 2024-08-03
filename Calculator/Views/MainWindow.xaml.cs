using Prism.Events;
using Rod.Calculator.Library.Events;
using System;
using System.Windows;
using System.Windows.Input;

namespace Rod.Calculator.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IEventAggregator aggregator;


        ////////////////////////////////////////////////////////////////////
        // Indicators for when special keys are pressed
        Boolean IsRightControlKeyPressed = false;
        Boolean IsRightShiftKeyPressed = false;
        Boolean IsRightAltKeyPressed = false;

        Boolean IsLeftControlKeyPressed = false;
        Boolean IsLeftShiftKeyPressed = false;
        Boolean IsLeftAltKeyPressed = false;
        // Indicators for when special keys are pressed
        ////////////////////////////////////////////////////////////////////

        public MainWindow(IEventAggregator eventAggregator)
        {
            this.aggregator = eventAggregator;
            InitializeComponent();
        }

        private void Keyboard_KeyUp(object sender, KeyEventArgs e)
        {
            ////////////////////////////////////////////////////////////////////
            // Determine which Ctrl, Alt, Shift keys were released
            if (e.Key == Key.RightCtrl)
            {
                IsRightControlKeyPressed = false;
            }
            else if (e.Key == Key.RightShift)
            {
                IsRightShiftKeyPressed = false;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                IsLeftControlKeyPressed = false;
            }
            else if (e.Key == Key.LeftShift)
            {
                IsLeftShiftKeyPressed = false;
            }
            else if (e.Key == Key.LeftAlt)
            {
                IsLeftAltKeyPressed = false;
            }
            else if (e.Key == Key.RightAlt)
            {
                IsRightAltKeyPressed = false;
            }
            // Determine which Ctrl, Alt, Shift keys were released
            ////////////////////////////////////////////////////////////////////


            ////////////////////////////////////////////////////////////////////
            // When no Ctrl, Alt, Shift keys are pressed
            else if ((IsRightControlKeyPressed == false) &&
                    (IsRightShiftKeyPressed == false) &&
                    (IsRightAltKeyPressed == false) &&
                    (IsLeftControlKeyPressed == false) &&
                    (IsLeftShiftKeyPressed == false) &&
                    (IsLeftAltKeyPressed == false))
            {
                if ((e.Key >= Key.NumPad0) && (e.Key <= Key.NumPad9))
                {
                    String keyName = $"{e.Key}".Replace("NumPad", "Number");
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish(keyName);
                }

                else if ((e.Key >= Key.D0) && (e.Key <= Key.D9))
                {
                    String keyName = $"{e.Key}".Replace("D", "Number");
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish(keyName);
                }
                else if ((e.Key == Key.OemPeriod) || (e.Key == Key.Decimal))
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("Decimal");
                }
                else if ((e.Key == Key.OemMinus) || (e.Key == Key.Subtract))
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathMinus");
                }
                else if ((e.Key == Key.Oem2) || (e.Key == Key.Divide))
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathDivide");
                }
                else if (e.Key == Key.Add)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathPlus");
                }
                else if (e.Key == Key.Multiply)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathMultiply");
                }
                else if (e.Key == Key.Back)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("BackSp");
                }
                else if (e.Key == Key.OemPlus)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("Equal");
                }
            }
            // When no Ctrl, Alt, Shift keys are pressed
            ////////////////////////////////////////////////////////////////////

            ////////////////////////////////////////////////////////////////////
            // When no Ctrl, Alt keys are pressed and a Shift key is pressed
            else if (((IsRightControlKeyPressed == false) &&
                (IsRightAltKeyPressed == false) &&
                (IsLeftControlKeyPressed == false) &&
                (IsLeftAltKeyPressed == false)) &&
                ((IsLeftShiftKeyPressed == true) || (IsRightShiftKeyPressed == true)))
            {
                if (e.Key == Key.OemPlus)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathPlus");
                }
                else if (e.Key == Key.D8)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("MathMultiply");
                }
                else if (e.Key == Key.D5)
                {
                    aggregator.GetEvent<Keyboard_Released_Event>().Publish("Percent");
                }
            }
            // When no Ctrl, Alt keys are pressed and a Shift key is pressed
            ////////////////////////////////////////////////////////////////////
        }

        private void Keyboard_KeyDn(object sender, KeyEventArgs e)
        {
            ////////////////////////////////////////////////////////////////////
            // Determine which Ctrl, Alt, Shift keys are depressed
            if (e.Key == Key.RightCtrl)
            {
                IsRightControlKeyPressed = true;
            }
            else if (e.Key == Key.RightShift)
            {
                IsRightShiftKeyPressed = true;
            }
            else if (e.Key == Key.LeftCtrl)
            {
                IsLeftControlKeyPressed = true;
            }
            else if (e.Key == Key.LeftShift)
            {
                IsLeftShiftKeyPressed = true;
            }
            else if (e.Key == Key.LeftAlt)
            {
                IsLeftAltKeyPressed = true;
            }
            else if (e.Key == Key.RightAlt)
            {
                IsRightAltKeyPressed = true;
            }
            // Determine which Ctrl, Alt, Shift keys are depressed
            ////////////////////////////////////////////////////////////////////

        }
    }
}
