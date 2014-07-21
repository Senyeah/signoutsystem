using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisitorManagementSystem {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public bool keyboardIsVisible;

        private Stack<Grid> gridStack;
        private List<TextBox> UITextBoxes; 

        private int currentStackScreen;
        private EventHandler slideCompletionHandler;

        public Manager sessionManager;

        private void attachEventHandlers(Grid grid) {
            UIElementCollection children = grid.Children;

            foreach (UIElement instance in children) {
                if (instance.GetType() == typeof(Grid) && ((Grid)instance).Children.Count > 0) {
                    attachEventHandlers((Grid)instance);
                } else if (instance.GetType() == typeof(TextBox) || instance.GetType() == typeof(PasswordBox)) {
                    if (instance.GetType() == typeof(TextBox)) {
                        if (((TextBox)instance).IsReadOnly == true) {
                            continue;
                        }
                    }

                    instance.GotFocus += new RoutedEventHandler(TextBoxGotFocus);
                } else {
                    instance.GotFocus += new RoutedEventHandler(ElementGotFocus);
                }
            }
        }

        public void disableSystem() {
            ErrorModalView errorView = new ErrorModalView();
            this.MainGrid.Children.Add(errorView);

            ((App)Application.Current).isEnabled = false;
        }

        private void ApplicationExit(object sender, ExitEventArgs e) {
            if (sessionManager != null) {
                sessionManager.closeSession();
            }
        }

        public MainWindow() {
            InitializeComponent();

            //If we're on the actual screen, hide the cursor
            if (System.Windows.SystemParameters.PrimaryScreenWidth == 1024 &&
                System.Windows.SystemParameters.PrimaryScreenHeight == 768) {
                Mouse.OverrideCursor = Cursors.None;
            }

            Application.Current.Exit += ApplicationExit;

            keyboardIsVisible = false;
            currentStackScreen = 1;

            attachEventHandlers(this.MainGrid);

            gridStack = new Stack<Grid>();

            gridStack.Push(this.PrintingGrid);
            gridStack.Push(this.TimeGrid);
            gridStack.Push(this.ToSeeGrid);
            gridStack.Push(this.LocationGrid);
            gridStack.Push(this.NameGrid);
            gridStack.Push(this.WelcomeGrid);

            UITextBoxes = new List<TextBox>();
            UITextBoxes.Add(ToSeeField);
            UITextBoxes.Add(LocationField);
            UITextBoxes.Add(NameField);

            sessionManager = new Manager();
            sessionManager.connectToServer();
        }

        public void ElementGotFocus(object sender, RoutedEventArgs e) {
            //MessageBox.Show("element got focus");

            if (keyboardIsVisible) {
                UserKeyboard.hideKeyboard();
            }
        }

        public void TextBoxGotFocus(object sender, RoutedEventArgs e) {
            //MessageBox.Show("TextBox Got Focus");

            if (keyboardIsVisible == false) {
                UserKeyboard.showKeyboard();
            } else {
                UserKeyboard.bringToFront();
            }
        }

        /// <summary>
        /// Returns the main grid as the current object and resets the stack
        /// of grids to display
        /// <param name="animate">Animate or snap the initial grid back</param>
        /// <param name="clearState">Clear all user-entered fields</param>
        /// </summary>
        private void returnToMainScreen(bool animate, bool clearState = true) {
            currentStackScreen = 0;

            gridStack.Clear();

            gridStack.Push(this.PrintingGrid);
            gridStack.Push(this.TimeGrid);
            gridStack.Push(this.ToSeeGrid);
            gridStack.Push(this.LocationGrid);
            gridStack.Push(this.NameGrid);
            gridStack.Push(this.WelcomeGrid);

            //Reset the text of all the textboxes
            foreach (TextBox box in this.UITextBoxes) {
                box.Text = "";
            }

            this.WelcomeGrid.Visibility = Visibility.Visible;

            if (animate) {
                this.WelcomeGrid.Margin = new Thickness(0, 0, 0, 0);
                this.WelcomeGrid.Opacity = 0.0;

                Storyboard slideInStoryboard = (Storyboard)this.FindResource("GridSlideIn");
                Storyboard.SetTarget(slideInStoryboard, this.WelcomeGrid);

                slideInStoryboard.Begin();
            } else {
                this.WelcomeGrid.Margin = new Thickness(0, 0, 0, 0);
                this.WelcomeGrid.Opacity = 1.0;
            }

            Storyboard slideOutStoryboard = (Storyboard)this.FindResource("GridSlideOut");
            Storyboard.SetTarget(slideOutStoryboard, PrintingGrid);

            slideOutStoryboard.Begin();
        }

        private void finishSession() {
            string name = NameField.Text;
            string from = LocationField.Text;
            string toSee = ToSeeField.Text;
            DateTime leaveTime = LeaveTimePicker.chosenDateTime();

            bool authenticationResult = sessionManager.authenticateVisitor(name, from, toSee, leaveTime);

            if (authenticationResult) {
                sessionManager.printVisitorPass(name, leaveTime);
                returnToMainScreen(true);
            }
        }

        private void NextButton_Click(object sender, RoutedEventArgs e) {
            Storyboard slideOutStoryboard = (Storyboard)this.FindResource("GridSlideOut");
            Storyboard.SetTarget(slideOutStoryboard, gridStack.Pop());

            Storyboard slideInStoryboard = (Storyboard)this.FindResource("GridSlideIn");
            gridStack.Peek().Visibility = Visibility.Visible;

            this.slideCompletionHandler = delegate(object s, EventArgs eventArgs) {
                IEnumerable<TextBox> textBoxes = ((Grid)gridStack.Peek()).Children.OfType<TextBox>();

                if (textBoxes.Count() > 0) {
                    TextBox primaryTextBox = textBoxes.Where(elem => elem.TabIndex == 1).First();
                    primaryTextBox.Focus();
                }

                if (gridStack.Count() == 1) {
                    finishSession();
                }

                slideInStoryboard.Completed -= this.slideCompletionHandler;
            };

            slideInStoryboard.Completed += this.slideCompletionHandler;

            Storyboard.SetTarget(slideInStoryboard, gridStack.Peek());

            slideOutStoryboard.Begin();
            slideInStoryboard.Begin();

            currentStackScreen++;
        }

        private void Settings_Click(object sender, RoutedEventArgs e) {
            AuthenticationModalView view = new AuthenticationModalView();
            MainGrid.Children.Add(view);

            int maxZIndex = this.MainGrid.Children.OfType<Grid>().Select(element => Canvas.GetZIndex(element)).Max();
            Canvas.SetZIndex(KeyboardGrid, maxZIndex + 1);

            this.attachEventHandlers(view.BaseGrid);
        }
    }
}
