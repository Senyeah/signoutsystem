using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;

namespace VisitorManagementSystem {
    /// <summary>
    /// Defines action types for keys so that different keys can correspond to different actions
    /// </summary>
    public enum KeyActionType {
        Input = 0, //Normal characters where their value does correspond to the correct virtual key code
        ForwardAction = 1, //Actions where the character value does not directly correspond to the appropriate virtual key code
        InternalAction = 2 //Actions which are best handled by the Keyboard class (eg. '123', 'Undo', Hide Keyboard)
    }

    /// <summary>
    /// Keys for easily referencing Key objects in the code
    /// </summary>
    public enum KeyType {
        InternalKeyTypeShift = '\u2191',
        InternalKeyKeyboard = '\u2328',
        InternalKeyNumeric = '\a', //1-byte key for the '123' sequence
        InternalKeyQwerty = '\b', //There must be a better way than this
        InternalKeyUndo = '\f', //Seriously.
        InternalKeyMoreNumeric = '\uE10C',
        ForwardKeyEnter = '\u21A9',
        ForwardKeyTypeBackspace = '\u232B',
        ForwardKeySpace = ' '
    }

    /// <summary>
    /// Implements keyboard-related Platform Invoke methods from user32.dll.
    /// 
    /// It is an internal class so that the functions can only be called from this
    /// assembly, which strengthens the application's security from code injections
    /// </summary>
    internal class NativeMethods {
        [DllImport("user32.dll", EntryPoint="keybd_event")]
        internal static extern void keybd_event(byte bVk, byte bScan, int dwFlags, IntPtr dwExtraInfo);

        [DllImport("user32.dll", EntryPoint="VkKeyScanEx")]
        internal static extern short VkKeyScanEx(char ch, IntPtr dwhkl);

        [DllImport("user32.dll", EntryPoint="LoadKeyboardLayout", CharSet=CharSet.Unicode)]
        internal static extern IntPtr LoadKeyboardLayout([MarshalAs(UnmanagedType.LPWStr)] string pwszKLID, uint flags);
    }

    /// <summary>
    /// Key class for the keys on the keyboard.
    /// 
    /// This class handles touches and related events for individual keys,
    /// and passes on events to the parent keyboard as required
    /// </summary>
    public class Key : ContentControl {
        private KeyActionType actionType;
        private Keyboard parent;
        private DispatcherTimer timer;
        private double keyDownTime;

        public Label keyTitle;
        public char value;

        public static readonly SolidColorBrush defaultKeyColour = new SolidColorBrush(Color.FromRgb(25, 25, 25));
        public static readonly SolidColorBrush hoverKeyColor = new SolidColorBrush(Color.FromRgb(60, 60, 60));

        /// <summary>
        /// Instantiates a new Key object
        /// </summary>
        /// <param name="keyCharValue">The character to display on the key</param>
        /// <param name="keyActionType">The type action to perform when the key is tapped</param>
        /// <param name="size">The size of the key object</param>
        /// <param name="location">The location on the keyboard</param>
        /// <param name="parentObject">The key's parent keyboard object</param>
        public Key(char keyCharValue, KeyActionType keyActionType, Size size, Point location, Keyboard parentObject) {
            this.Width = size.Width;
            this.Height = size.Height;

            this.MouseDown += new MouseButtonEventHandler(MouseDown_Event);
            this.MouseUp += new MouseButtonEventHandler(MouseUp_Event);
            this.MouseLeave += new MouseEventHandler(MouseUp_Event);

            if (keyActionType == KeyActionType.Input || keyCharValue == (char)KeyType.ForwardKeyTypeBackspace) {
                timer = new DispatcherTimer();
                timer.Interval = new TimeSpan(0, 0, 0, 0, 30);
                timer.Tick += new EventHandler(MouseDownTimer_Tick);
            }

            keyDownTime = 0;

            value = keyCharValue;
            parent = parentObject;

            Canvas.SetTop(this, location.Y);
            Canvas.SetLeft(this, location.X);

            keyTitle = new Label();
            keyTitle.VerticalAlignment = VerticalAlignment.Top;
            keyTitle.HorizontalAlignment = HorizontalAlignment.Left;

            keyTitle.Width = this.Width;
            keyTitle.Height = this.Height;

            keyTitle.Background = Key.defaultKeyColour;
            keyTitle.Foreground = Brushes.White;

            keyTitle.FontFamily = new FontFamily("Segoe UI Symbol");

            keyTitle.FontSize = (double)new LengthConverter().ConvertFrom("32pt");
            keyTitle.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center;
            keyTitle.VerticalContentAlignment = System.Windows.VerticalAlignment.Center;

            actionType = keyActionType;

            if (value == (char)KeyType.ForwardKeyEnter) {
                keyTitle.HorizontalContentAlignment = System.Windows.HorizontalAlignment.Right;
                keyTitle.Padding = new Thickness(0, 0, 15, 0);
            }

            string keyTitleValue = value.ToString();

            switch (value) {
                case (char)KeyType.InternalKeyNumeric:
                    keyTitleValue = "123";
                    break;
                
                case (char)KeyType.InternalKeyQwerty:
                    keyTitleValue = "ABC";
                    break;

                case (char)KeyType.InternalKeyMoreNumeric:
                    keyTitleValue = ((char)KeyType.InternalKeyMoreNumeric).ToString();
                    break;

                case (char)KeyType.InternalKeyUndo:
                    keyTitleValue = "Undo";
                    break;
            }

            keyTitle.Content = keyTitleValue;
            this.Content = keyTitle;
        }

        /// <summary>
        /// Perform a click event for the corresponding key. This function should not be invoked manually, but rather by the MouseDown event.
        /// </summary>
        public void clickKey() {
            if (this.actionType == KeyActionType.Input) {
                Keyboard.performKeyPress(Char.ToLower(this.value));

                if (parent.shiftEnabled == true) {
                    parent.shiftEnabled = false;
                    parent.setShiftKeyColour();

                    NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyUp, IntPtr.Zero);
                }
            } else if (this.actionType == KeyActionType.InternalAction) {
                parent.handleInternalEvent(this);
            } else if (this.actionType == KeyActionType.ForwardAction) {
                switch (this.value) {
                    case (char)KeyType.ForwardKeyTypeBackspace:
                        NativeMethods.keybd_event(Keyboard.backspaceKey, 0, Keyboard.keyDown, IntPtr.Zero);
                        break;

                    case (char)KeyType.ForwardKeyEnter:
                        NativeMethods.keybd_event(Keyboard.enterKey, 0, Keyboard.keyDown, IntPtr.Zero);
                        break;
                    case (char)KeyType.ForwardKeySpace:
                        NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyDown, IntPtr.Zero);
                        NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyDown, IntPtr.Zero);

                        parent.shiftEnabled = true;
                        Keyboard.performKeyPress(this.value);
                        break;
                }
            }
        }

        private void MouseDownTimer_Tick(object sender, EventArgs e) {
            this.keyDownTime += this.timer.Interval.Milliseconds;

            if (this.keyDownTime > 200 && this.actionType != KeyActionType.InternalAction) {
                this.clickKey();
            }
        }

        private void MouseDown_Event(object sender, MouseButtonEventArgs e) {
            if (this.actionType != KeyActionType.InternalAction) {
                keyTitle.Background = Key.hoverKeyColor;
            }

            this.clickKey();

            if (this.actionType == KeyActionType.Input || this.value == (char)KeyType.ForwardKeyTypeBackspace) {
                this.timer.Start();
            }
        }

        private void MouseUp_Event(object sender, MouseEventArgs e) {
            if (this.actionType == KeyActionType.Input || this.value == (char)KeyType.ForwardKeyTypeBackspace) {
                this.timer.Stop();
                this.keyDownTime = 0;
            }

            if (this.actionType == KeyActionType.Input) {
                keyTitle.Background = Key.defaultKeyColour;
            } else if (this.actionType == KeyActionType.ForwardAction) {
                keyTitle.Background = Key.defaultKeyColour;
            }
        }
    }

    /// <summary>
    /// An abstract base class which provides the layout and functionality
    /// of a keyboard. Language and symbol variants are extended from this class.
    /// </summary>
    public abstract partial class Keyboard : UserControl {
        protected object[,] keyMap;
        protected List<Size> keySizes;

        protected List<Key> internalKeys;
        protected List<Key> normalKeys;

        //dwFlags constants
        public const int keyUp = 0x00000002;
        public const int keyDown = 0x00000000;

        //Virtual Key Codes for keybd_event
        public const byte enterKey = 0x0D;
        public const byte backspaceKey = 0x08;
        public const byte shiftKey = 0xA0;
        public const byte controlKey = 0x11;
        public const byte altKey = 0x12;

        //pwszKLID language identifier
        public const string identifierUS = "00000409";

        protected bool _shiftEnabled;

        public bool shiftEnabled {
            get {
                return _shiftEnabled;
            }

            set {
                _shiftEnabled = value;
                this.setShiftKeyColour();
            }
        }

        protected static readonly IntPtr defaultKeyboardLayout;

        public bool isVisible;

        /// <summary>
        /// Static constructor for the Keyboard class.
        /// Loads the default keyboard layout into the read-only defaultKeyboardLayout static pointer.
        /// </summary>
        static Keyboard() {
            defaultKeyboardLayout = NativeMethods.LoadKeyboardLayout(identifierUS, 0);
        }

        /// <summary>
        /// Instantiates a new Keyboard object. This method cannot be called manually.
        /// </summary>
        public Keyboard() {
            InitializeComponent();
            
            internalKeys = new List<Key>();
            normalKeys = new List<Key>();

            keySizes = new List<Size>();

            isVisible = false;
        }

        /// <summary>
        /// Provides an overrideable function for handling internal
        /// key events. This method should not be invoked manually.
        /// </summary>
        /// <param name="keyObject">The key object to handle</param>
        public abstract void handleInternalEvent(Key keyObject);

        /// <summary>
        /// Peforms a keypress with the corresponding character
        /// </summary>
        /// <param name="keyCode">The character to print</param>
        public static void performKeyPress(char keyCode) {   
            int numericValue = (int)keyCode;
            short virtualKeyCodeState = NativeMethods.VkKeyScanEx(keyCode, Keyboard.defaultKeyboardLayout);
            
            //Extract the high- and low-bytes from the 16-bit integer returned from VkKeyScanEx
            byte virtualKeyCode = (byte)(virtualKeyCodeState & 0xFF);
            bool shiftModifier = (virtualKeyCodeState >> 8) == 0x01;

            if (shiftModifier) {
                NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyDown, IntPtr.Zero);
            }

            NativeMethods.keybd_event(virtualKeyCode, 0, Keyboard.keyDown, IntPtr.Zero);
            NativeMethods.keybd_event(virtualKeyCode, 0, Keyboard.keyUp, IntPtr.Zero);

            if (shiftModifier) {
                NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyUp, IntPtr.Zero);
            }
        }

        /// <summary>
        /// Bring this instance of the keyboard to the front of the parent Canvas
        /// </summary>
        public void bringToFront() {
            //linq is the devil you just don't know it
            int maxZIndex = ((MainWindow)Application.Current.MainWindow).KeyboardCanvas.Children.OfType<Keyboard>() //Find all of the keyboards
                                                                                                .Where(element => element != this) //that aren't the current keyboard
                                                                                                .Select(element => Canvas.GetZIndex(element)) //and get their z-indicies
                                                                                                .Max(); //then find the maximum one
            Canvas.SetZIndex(this, maxZIndex + 1);
        }

        /// <summary>
        /// Sets the colour for the two shift keys on the keyboard, if applicable.
        /// </summary>
        public void setShiftKeyColour() {
            foreach (Key key in this.internalKeys) {
                if (key.value == (char)KeyType.InternalKeyTypeShift) {
                    if (this.shiftEnabled == true) {
                        key.keyTitle.Background = Key.hoverKeyColor;
                    } else {
                        key.keyTitle.Background = Key.defaultKeyColour;
                    }
                }
            }
        }

        /// <summary>
        /// Show the hide keyboard animation and hide the parent canvas, hiding all the keyboards.
        /// </summary>
        public void hideKeyboard() {
            MainWindow mainWindowInstance = (MainWindow)Application.Current.MainWindow;
            FocusManager.SetFocusedElement(mainWindowInstance, mainWindowInstance);

            mainWindowInstance.keyboardIsVisible = false;

            Storyboard animationStoryboard = (Storyboard)mainWindowInstance.KeyboardCanvas.FindResource("HideAnimation");
            Storyboard.SetTarget(animationStoryboard, mainWindowInstance.KeyboardCanvas);
            Timeline.SetDesiredFrameRate(animationStoryboard, 60);
            
            animationStoryboard.Begin();
        }

        /// <summary>
        /// Show all the keyboards and canvas with an animation and bring the main qwerty keyboard to the front.
        /// </summary>
        public void showKeyboard() {
            MainWindow mainWindowInstance = (MainWindow)Application.Current.MainWindow;
            mainWindowInstance.keyboardIsVisible = true;
            mainWindowInstance.UserKeyboard.bringToFront();

            Storyboard animationStoryboard = (Storyboard)mainWindowInstance.KeyboardCanvas.FindResource("ShowAnimation");
            Storyboard.SetTarget(animationStoryboard, mainWindowInstance.KeyboardCanvas);
            Timeline.SetDesiredFrameRate(animationStoryboard, 60);

            animationStoryboard.Begin();
        }

        /// <summary>
        /// An overrideable function which renders the keys based on the
        /// keyMap and keySizes instance variables.
        /// 
        /// Provides a default implementation for rendering qwery-like layouts.
        /// </summary>
        protected virtual void renderKeys() {
            for (int i = 0; i < (this.keyMap.Length / 2); i++) {
                char character = (char)this.keyMap[i, 0];

                KeyActionType currentActionType = (KeyActionType)this.keyMap[i, 1];

                double xPosition = 0;
                double yPosition = 0;

                //Awful key layout code:
                if (i < 11) {
                    xPosition = (i * 93) + 6;
                    yPosition = 9;
                } else if (i >= 11 && i < 21) {
                    xPosition = ((i - 11) * 92) + 44;
                    yPosition = (9 + 76) + 9;
                } else if (i >= 21 && i < 32) {
                    xPosition = 0;

                    for (int k = i; k > 21; k--) {
                        xPosition += this.keySizes[k - 1].Width;
                    }

                    xPosition += ((i - 21) * 12) + 6;
                    yPosition = 2 * (9 + 76) + 9;
                } else {
                    xPosition = 0;

                    for (int k = i; k > 32; k--) {
                        xPosition += this.keySizes[k - 1].Width;
                    }

                    xPosition += ((i - 32) * 12) + 6;
                    yPosition = 3 * (9 + 76) + 9;
                }

                Key keyInstance = new Key(character, currentActionType, this.keySizes[i], new Point(xPosition, yPosition), this);

                if (currentActionType == KeyActionType.InternalAction || currentActionType == KeyActionType.ForwardAction) {
                    this.internalKeys.Add(keyInstance);
                } else {
                    this.normalKeys.Add(keyInstance);
                }

                KeyCanvas.Children.Add(keyInstance);
            }
        }
    }

    public class CharacterKeyboard : Keyboard {
        public CharacterKeyboard() {
            keySizes = Enumerable.Repeat(new Size(81, 76), 10).ToList();
            keySizes.Add(new Size(82, 76));
            keySizes.AddRange(Enumerable.Repeat(new Size(80, 76), 9));
            keySizes.AddRange(new Size[] { new Size(146, 76), new Size(79, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(78, 76), new Size(78, 76) });
            keySizes.AddRange(new Size[] { new Size(107, 76), new Size(260, 76), new Size(441, 76), new Size(196, 76), new Size(79, 76) });

            keyMap = new object[,]{ { 'Q', KeyActionType.Input }, { 'W', KeyActionType.Input }, { 'E', KeyActionType.Input }, { 'R', KeyActionType.Input }, { 'T', KeyActionType.Input }, 
            { 'Y', KeyActionType.Input }, { 'U', KeyActionType.Input }, { 'I', KeyActionType.Input }, { 'O', KeyActionType.Input }, { 'P', KeyActionType.Input }, { '\u232B', KeyActionType.ForwardAction }, { 'A', KeyActionType.Input }, 
            { 'S', KeyActionType.Input }, { 'D', KeyActionType.Input }, { 'F', KeyActionType.Input }, { 'G', KeyActionType.Input }, { 'H', KeyActionType.Input }, { 'J', KeyActionType.Input }, { 'K', KeyActionType.Input }, 
            { 'L', KeyActionType.Input }, { '\u21A9', KeyActionType.ForwardAction }, { '\u2191', KeyActionType.InternalAction }, { 'Z', KeyActionType.Input }, { 'X', KeyActionType.Input }, { 'C', KeyActionType.Input }, { 'V', KeyActionType.Input }, 
            { 'B', KeyActionType.Input }, { 'N', KeyActionType.Input }, { 'M', KeyActionType.Input }, { ',', KeyActionType.Input }, { '.', KeyActionType.Input }, { '\u2191', KeyActionType.InternalAction }, { '\a', KeyActionType.InternalAction }, 
            { ' ', KeyActionType.ForwardAction }, { '\a', KeyActionType.InternalAction }, { '\u2328', KeyActionType.InternalAction } };

            base.renderKeys();
            this.setShiftEnabled();
        }

        public void setShiftEnabled() {
            NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyDown, IntPtr.Zero);

            this.shiftEnabled = true;
            Keyboard.performKeyPress((char)KeyType.ForwardKeySpace);
        }

        public override void handleInternalEvent(Key keyObject) {
            if (keyObject.value == (char)KeyType.InternalKeyTypeShift) {
                NativeMethods.keybd_event(Keyboard.shiftKey, 0, Keyboard.keyDown, IntPtr.Zero);
                this.shiftEnabled = !this.shiftEnabled;
            } else if (keyObject.value == (char)KeyType.InternalKeyKeyboard) {
                this.hideKeyboard();
            } else if (keyObject.value == (char)KeyType.InternalKeyNumeric) {
                //Get the instance of the other keyboard
                ((MainWindow)Application.Current.MainWindow).UserNumericKeyboard.bringToFront();

                //Set the background colour of this key back to black
                keyObject.keyTitle.Background = Key.defaultKeyColour;
            }
        }
    }

    public class NumericKeyboard : Keyboard {
        public NumericKeyboard() : base() {
            keyMap = new object[,] { { '1', KeyActionType.Input }, { '2', KeyActionType.Input }, { '3', KeyActionType.Input }, { '4', KeyActionType.Input }, { '5', KeyActionType.Input }, 
            { '6', KeyActionType.Input }, { '7', KeyActionType.Input }, { '8', KeyActionType.Input }, { '9', KeyActionType.Input }, { '0', KeyActionType.Input }, { '\u232B', KeyActionType.ForwardAction }, { '@', KeyActionType.Input }, 
            { '#', KeyActionType.Input }, { '$', KeyActionType.Input }, { '%', KeyActionType.Input }, { '&', KeyActionType.Input }, { '-', KeyActionType.Input }, { '+', KeyActionType.Input }, { '(', KeyActionType.Input }, 
            { ')', KeyActionType.Input }, { '\u21A9', KeyActionType.ForwardAction }, { '\uE10C', KeyActionType.InternalAction }, { '.', KeyActionType.Input }, { ',', KeyActionType.Input }, { '?', KeyActionType.Input }, { '!', KeyActionType.Input }, 
            { '\'', KeyActionType.Input }, { '"', KeyActionType.Input }, { ':', KeyActionType.Input }, { '\f', KeyActionType.InternalAction }, { '\uE10C', KeyActionType.InternalAction }, { '\b', KeyActionType.InternalAction }, 
            { ' ', KeyActionType.Input }, { '\b', KeyActionType.InternalAction }, { '\u2328', KeyActionType.InternalAction } };

            keySizes = Enumerable.Repeat(new Size(81, 76), 10).ToList();
            keySizes.Add(new Size(82, 76));
            keySizes.AddRange(Enumerable.Repeat(new Size(80, 76), 9));
            keySizes.AddRange(new Size[] { new Size(146, 76), new Size(79, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(78, 76), new Size(79, 76), new Size(168, 76) });
            keySizes.AddRange(new Size[] { new Size(107, 76), new Size(260, 76), new Size(441, 76), new Size(196, 76), new Size(79, 76) });

            this.renderKeys();
        }

        protected sealed override void renderKeys() {
            for (int i = 0; i < (this.keyMap.Length / 2); i++) {
                char character = (char)this.keyMap[i, 0];

                KeyActionType currentActionType = (KeyActionType)this.keyMap[i, 1];

                double xPosition = 0;
                double yPosition = 0;
                
                //Awful key layout code:
                if (i < 11) {
                    xPosition = (i * 93) + 6;
                    yPosition = 9;
                } else if (i >= 11 && i < 21) {
                    xPosition = ((i - 11) * 92) + 44;
                    yPosition = (9 + 76) + 9;
                } else if (i >= 21 && i < 31) {
                    xPosition = 0;

                    for (int k = i; k > 21; k--) {
                        xPosition += this.keySizes[k - 1].Width;
                    }

                    xPosition += ((i - 21) * 12) + 6;
                    yPosition = 2 * (9 + 76) + 9;
                } else {
                    xPosition = 12;
                    
                    for (int k = i; k > 31; k--) {
                        xPosition += this.keySizes[k - 1].Width;
                    }

                    xPosition += ((i - 32) * 12) + 6;
                    yPosition = 3 * (9 + 76) + 9;
                }
                
                Key keyInstance = new Key(character, currentActionType, this.keySizes[i], new Point(xPosition, yPosition), this);

                if (currentActionType == KeyActionType.InternalAction || currentActionType == KeyActionType.ForwardAction) {
                    this.internalKeys.Add(keyInstance);
                } else {
                    this.normalKeys.Add(keyInstance);
                }

                KeyCanvas.Children.Add(keyInstance);
            }
        }

        public sealed override void handleInternalEvent(Key keyObject) {
            if (keyObject.value == (char)KeyType.InternalKeyKeyboard) {
                this.hideKeyboard();
            } else if (keyObject.value == (char)KeyType.InternalKeyQwerty) {
                //Get the instance of the other keyboard
                ((MainWindow)Application.Current.MainWindow).UserKeyboard.bringToFront();
                keyObject.keyTitle.Background = Key.defaultKeyColour;
            } else if (keyObject.value == (char)KeyType.InternalKeyMoreNumeric) {
                if (this.GetType() == typeof(SymbolKeyboard)) {
                    //Display the numeric keyboard
                    ((MainWindow)Application.Current.MainWindow).UserNumericKeyboard.bringToFront();
                } else {
                    //Display the symbol keyboard
                    ((MainWindow)Application.Current.MainWindow).UserSymbolKeyboard.bringToFront();
                }
            } else if (keyObject.value == (char)KeyType.InternalKeyUndo) {
                //it's cheap i know
                short virtualKeyCode = NativeMethods.VkKeyScanEx('z', Keyboard.defaultKeyboardLayout);
                byte keyCode = (byte)(virtualKeyCode & 0xFF);

                NativeMethods.keybd_event(Keyboard.controlKey, 0, Keyboard.keyDown, IntPtr.Zero);
                NativeMethods.keybd_event(keyCode, 0, Keyboard.keyDown, IntPtr.Zero);

                NativeMethods.keybd_event(Keyboard.controlKey, 0, Keyboard.keyUp, IntPtr.Zero);
                NativeMethods.keybd_event(keyCode, 0, Keyboard.keyUp, IntPtr.Zero);
            }
        }
    }

    public class SymbolKeyboard : NumericKeyboard {
        public SymbolKeyboard() : base() {
            keyMap = new object[,] { { '/', KeyActionType.Input }, { '[', KeyActionType.Input }, { ']', KeyActionType.Input }, { '{', KeyActionType.Input }, { '}', KeyActionType.Input }, 
            { '^', KeyActionType.Input }, { '*', KeyActionType.Input }, { '=', KeyActionType.Input }, { '\\', KeyActionType.Input }, { '|', KeyActionType.Input }, { '\u232B', KeyActionType.ForwardAction }, { '@', KeyActionType.Input }, 
            { '~', KeyActionType.Input }, { '<', KeyActionType.Input }, { '>', KeyActionType.Input }, { '%', KeyActionType.Input }, { ':', KeyActionType.Input }, { ';', KeyActionType.Input }, { '_', KeyActionType.Input }, { '`', KeyActionType.Input }, 
            { '\u21A9', KeyActionType.ForwardAction }, { '\uE10C', KeyActionType.InternalAction }, { '.', KeyActionType.Input }, { ',', KeyActionType.Input }, { '?', KeyActionType.Input }, { '!', KeyActionType.Input }, 
            { '\'', KeyActionType.Input }, { '"', KeyActionType.Input }, { ':', KeyActionType.Input }, { '\f', KeyActionType.InternalAction }, { '\uE10C', KeyActionType.InternalAction }, { '\b', KeyActionType.InternalAction }, 
            { ' ', KeyActionType.Input }, { '\b', KeyActionType.InternalAction }, { '\u2328', KeyActionType.InternalAction } };

            base.renderKeys();
        }
    }
}