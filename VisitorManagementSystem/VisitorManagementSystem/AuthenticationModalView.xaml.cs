using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for ModalView.xaml
    /// </summary>
    public partial class AuthenticationModalView : UserControl {
        public Duration animationDuration = new Duration(new TimeSpan(0, 0, 0, 0, 0));

        public AuthenticationModalView() {
            InitializeComponent();

            attachEventHandlers(this.ContentGrid);

            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Input,
                new Action(delegate() {
                    this.UsernameField.Focus();
                    System.Windows.Input.Keyboard.Focus(this.UsernameField);
            }));
        }

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
                    instance.LostFocus += new RoutedEventHandler(ElementGotFocus);
                } else {
                    instance.GotFocus += new RoutedEventHandler(ElementGotFocus);
                }
            }
        }

        private void TextBoxGotFocus(object sender, RoutedEventArgs e) {
            Storyboard focusStoryboard = (Storyboard)BaseGrid.FindResource("focusAnimation");
            ThicknessAnimation marginAnimation = (ThicknessAnimation)focusStoryboard.Children[0];

            double viewHeight = 416; //768 - keyboard height
            double contentHeight = this.ContentGrid.Height;

            double yCoordinate = (viewHeight / 2) - (contentHeight / 2);
            double bottom = (768 - contentHeight) - yCoordinate;

            marginAnimation.To = new Thickness(0, yCoordinate, 0, bottom);

            focusStoryboard.Begin();
        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {
            Storyboard blurStoryboard = (Storyboard)BaseGrid.FindResource("blurAnimation");
            blurStoryboard.Begin();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            SettingsModalView settingsView = new SettingsModalView();

            ((Grid)this.Parent).Children.Add(settingsView);
            ((Grid)this.Parent).Children.Remove(this);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Storyboard closeStoryboard = (Storyboard)BaseGrid.FindResource("closeAnimation");

            Grid sisterGrid = (Grid)((Grid)this.Parent).FindName("PresentationScreens");

            ((Grid)this.Parent).Focus();

            closeStoryboard.Completed += delegate(object s, EventArgs ev) {
                ((Grid)this.Parent).Children.Remove(this);
            };
            
            closeStoryboard.Begin();
        }  
    }
}
