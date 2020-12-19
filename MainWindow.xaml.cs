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
using System.Diagnostics;

namespace PaswordCracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string password = "";
        string passwordhash = "";
        ulong possibilities = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        static string computeHash(string input)
        {
            SHA256 hash = SHA256.Create();
            hash.ComputeHash(Encoding.ASCII.GetBytes(input));

            string output = "";

            for(int i = 0; i < 32; i++) // sha256 returns 32 bytes
            {
                output += Convert.ToInt32(hash.Hash[i]);
            }

            return output;
        }

        ulong howManyPossiblePasswords(int passwordlength)
        {
            return 36 ^ (ulong)passwordlength;
        }

        private void crackButtonClick(object sender, RoutedEventArgs e)
        {
            password = ((TextBox)FindName("password_box")).Text;
            passwordhash = computeHash(password);

            ((TextBlock)FindName("hash_box")).Text = "Your SHA-1 hash: " + passwordhash;

            doCrack();
        }

        string incrementString(string currentstring)
        {
            if(currentstring.Substring(currentstring.Length-1, 1) != "z")
            {
                currentstring.
            }

            return currentstring;
        }

        void doCrack()
        {
            possibilities = howManyPossiblePasswords(password.Length);

            string curstring = "a";

            TextBlock outputbox = (TextBlock)FindName("output_text");

            for(ulong i = 0; i < possibilities; i++)
            {
                outputbox.Text += "Trying " + curstring + "\n";

                if (computeHash(curstring) == passwordhash)
                {
                    outputbox.Text += "Found password: " + curstring + "\n";
                    return;
                }

                curstring = incrementString(curstring);
            }
        }

        private void stopButtonClick(object sender, RoutedEventArgs e)
        {

        }
    }
}
