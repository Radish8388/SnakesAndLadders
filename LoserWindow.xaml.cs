using System.Windows;

namespace SnakesAndLadders
{
    /// <summary>
    /// Interaction logic for LoserWindow.xaml
    /// </summary>
    public partial class LoserWindow : Window
    {
        public LoserWindow()
        {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
