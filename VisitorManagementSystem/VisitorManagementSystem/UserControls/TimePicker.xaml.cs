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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VisitorManagementSystem {
    /// <summary>
    /// Interaction logic for TimePicker.xaml
    /// </summary>
    public partial class TimePicker : UserControl {
        public int hour;
        public int minute;
        public bool isAM;

        public DateTime chosenDateTime() {
            int newHour = -1;

            if (this.isAM || (!this.isAM && this.hour == 12)) {
                newHour = this.hour;
            } else {
                newHour = this.hour + 12;
            }

            return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, newHour, this.minute, 0);
        }
        
        public TimePicker() {
            InitializeComponent();

            DateTime now = DateTime.Now.AddHours(1);
            this.hour = now.Hour;
            this.minute = now.Minute - (now.Minute % 15);
            this.isAM = now.ToString("t").Contains("a");

            if (!this.isAM && this.hour != 12) {
                this.hour -= 12;
            }

            HourLabel.Content = this.hour.ToString();
            MinuteLabel.Content = this.minute.ToString("00");
            AMPMLabel.Content = (this.isAM) ? "am" : "pm";
        }

        private void Label_PreviewMouseDown(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.White;
            ((Label)sender).Background = new SolidColorBrush(Color.FromArgb(128, 0, 128, 255));
        }

        private void HourIncrementUp(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.Black;
            ((Label)sender).Background = Brushes.Transparent;

            if (this.hour + 1 > 12) {
                this.hour = 1;
            } else {
                this.hour += 1;
            }
            
            HourLabel.Content = this.hour.ToString();
        }

        private void HourDecrementUp(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.Black;
            ((Label)sender).Background = Brushes.Transparent;

            if (this.hour - 1 < 1) {
                this.hour = 12;
            } else {
                this.hour -= 1;
            }

            HourLabel.Content = this.hour.ToString();
        }

        private void MinuteIncrementUp(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.Black;
            ((Label)sender).Background = Brushes.Transparent;

            if (this.minute + 15 == 60) {
                this.minute = 0;
            } else {
                this.minute += 15;
            }

            MinuteLabel.Content = this.minute.ToString("00");
        }

        private void MinuteDecrementUp(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.Black;
            ((Label)sender).Background = Brushes.Transparent;

            if (this.minute - 15 < 0) {
                this.minute = 45;
            } else {
                this.minute -= 15;
            }

            MinuteLabel.Content = this.minute.ToString("00");
        }

        private void AMPMIncrementUp(object sender, MouseButtonEventArgs e) {
            ((Label)sender).Foreground = Brushes.Black;
            ((Label)sender).Background = Brushes.Transparent;

            this.isAM = !this.isAM;
            AMPMLabel.Content = (this.isAM) ? "am" : "pm";
        }
    }
}
