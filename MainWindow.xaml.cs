using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Security.Cryptography;

namespace PaswordCracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string password = "";

        public MainWindow()
        {
            InitializeComponent();
        }

        string computeHash(string input)
        {
            SHA256 hash = SHA256.Create();
            hash.ComputeHash(Encoding.ASCII.GetBytes(password));
            return Encoding.ASCII.GetString(hash.Hash);
        }

        private void crack_button_MouseUp(object sender, MouseButtonEventArgs e)
        {
            password = ((TextBox)FindName("password_box")).Text;

            ((TextBlock)FindName("hash_box")).Text = "Your SHA-1 hash: " + computeHash(password);
        }

        private void stop_button_MouseUp(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
