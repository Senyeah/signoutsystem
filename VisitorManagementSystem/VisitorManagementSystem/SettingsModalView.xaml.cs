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
    public partial class SettingsModalView : UserControl {
        public Duration animationDuration = new Duration(new TimeSpan(0, 0, 0, 0, 0));

        public SettingsModalView() {
            InitializeComponent();

            attachEventHandlers(this.ContentGrid);
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

        }

        private void ElementGotFocus(object sender, RoutedEventArgs e) {

        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            Storyboard closeStoryboard = (Storyboard)BaseGrid.FindResource("closeAnimation");

            Grid sisterGrid = (Grid)((Grid)this.Parent).FindName("PresentationScreens");
            //Storyboard UnBlurStoryboard = (Storyboard)(sisterGrid.FindResource("UnBlurAnimation"));

            ((Grid)this.Parent).Focus();

            closeStoryboard.Completed += delegate(object s, EventArgs ev) {
                ((Grid)this.Parent).Children.Remove(this);
            };
            
            closeStoryboard.Begin();
            //UnBlurStoryboard.Begin();
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e) {
            System.Diagnostics.Process.Start("osk");
            Application.Current.Shutdown(0);
        }  
    }
}
