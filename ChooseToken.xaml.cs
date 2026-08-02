using System.Windows;

namespace SnakesAndLadders
{
    /// <summary>
    /// Interaction logic for ChooseToken.xaml
    /// </summary>
    public partial class ChooseToken : Window
    {
        public int ChosenToken { get; private set; }
        public ChooseToken()
        {
            InitializeComponent();
        }

        private void GreenButton_Click(object sender, RoutedEventArgs e)
        {
            ChosenToken = 1;
            DialogResult = true;
        }

        private void BlueButton_Click(object sender, RoutedEventArgs e)
        {
            ChosenToken = 2;
            DialogResult = true;
        }

        private void PurpleButton_Click(object sender, RoutedEventArgs e)
        {
            ChosenToken = 3;
            DialogResult = true;
        }

        private void GrayButton_Click(object sender, RoutedEventArgs e)
        {
            ChosenToken = 4;
            DialogResult = true;
        }
    }
}
