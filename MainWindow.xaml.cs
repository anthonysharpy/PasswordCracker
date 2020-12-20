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
using System.Threading;

namespace PaswordCracker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool done = false;
        string password = "";
        string passwordhash = "";
        ulong possibilities = 0;
        ulong attempts = 0;

        int starttime;

        TextBox passwordbox;
        TextBlock hashbox;
        TextBlock outputbox;
        TextBlock outputinfo;

        SHA256 hashthingy;

        public MainWindow()
        {
            InitializeComponent();

            passwordbox = (TextBox)FindName("password_box");
            hashbox = (TextBlock)FindName("hash_box");
            outputbox = (TextBlock)FindName("output_text");
            outputinfo = (TextBlock)FindName("output_info");

            ThreadPool.SetMaxThreads(12, 12);
            ThreadPool.SetMinThreads(12, 12);

            hashthingy = SHA256.Create();
        }

        string computeHash(string input)
        {
            hashthingy.ComputeHash(Encoding.ASCII.GetBytes(input));

            string output = "";

            for(int i = 0; i < 32; i++) // sha256 returns 32 bytes
            {
                output += (int)hashthingy.Hash[i];
            }

            return output;
        }

        ulong howManyPossiblePasswords(int passwordlength)
        {
            return (ulong)Math.Pow(26, passwordlength);
        }

        int getTime()
        {
            return (int)DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        bool validPassword(string pass)
        {
            for(int i = 0; i < pass.Length; i++)
            {
                if (pass[i] < 97 || pass[i] > 122) return false;
            }

            return true;
        }

        void crackButtonClick(object sender, RoutedEventArgs e)
        {
            done = false;

            password = passwordbox.Text;
            passwordhash = computeHash(password);

            if(!validPassword(password))
            {
                hashbox.Text = "ERROR: password must only contain lowercase letters";
                return;
            }

            hashbox.Text = "Your SHA256 hash: " + passwordhash;

            int threads = 12;

            possibilities = howManyPossiblePasswords(password.Length);
            ulong possperthread = possibilities / (ulong)threads;
            ulong possforlastthread = possibilities - (possperthread * ((ulong)threads-1));

            starttime = getTime();

            Task task1 = Task.Factory.StartNew(() => doCrack(0, possperthread - 1, 1));
            Task task2 = Task.Factory.StartNew(() => doCrack(possperthread, (possperthread * 2) - 1, 2));
            Task task3 = Task.Factory.StartNew(() => doCrack(possperthread * 2, (possperthread * 3) - 1, 3));
            Task task4 = Task.Factory.StartNew(() => doCrack(possperthread * 3, (possperthread * 4) - 1, 4));
            Task task5 = Task.Factory.StartNew(() => doCrack(possperthread * 4, (possperthread * 5) - 1, 5));
            Task task6 = Task.Factory.StartNew(() => doCrack(possperthread * 5, (possperthread * 6) - 1, 6));
            Task task7 = Task.Factory.StartNew(() => doCrack(possperthread * 6, (possperthread * 7) - 1, 7));
            Task task8 = Task.Factory.StartNew(() => doCrack(possperthread * 7, (possperthread * 8) - 1, 8));
            Task task9 = Task.Factory.StartNew(() => doCrack(possperthread * 8, (possperthread * 9) - 1, 9));
            Task task10 = Task.Factory.StartNew(() => doCrack(possperthread * 9, (possperthread * 10) - 1, 10));
            Task task11 = Task.Factory.StartNew(() => doCrack(possperthread * 10, (possperthread * 11) - 1, 11));
            Task task12 = Task.Factory.StartNew(() => doCrack(possibilities - possforlastthread, possibilities-1, 12));
        }

        string incrementString(string currentstring, ulong times)
        {
            bool addnewchar;
            bool dirty;

            for (ulong t = 0; t < times; t++)
            {
                char[] chararray = currentstring.ToCharArray();
                addnewchar = false;

                int i = chararray.Length - 1;
                chararray[i] = (char)((int)chararray[i] + 1); // increments last character

                dirty = true; // need to analyse to make sure its been incremented along the whole string

                while (dirty)
                {
                    dirty = false;

                    for (int x = chararray.Length - 1; x > -1; x--)
                    {
                        if (chararray[x] == '{') // { is 1 after z
                        {
                            dirty = true;

                            // reset this char

                            chararray[x] = 'a';

                            // increment previous char

                            if (x == 0) addnewchar = true; // was first char, so we just need to add a new one on later
                            else chararray[x - 1] = (char)((int)chararray[x - 1] + 1); // increment previous char
                        }
                    }
                }

                currentstring = new string(chararray);
                if (addnewchar) currentstring = "a" + currentstring;
            }

            return currentstring;
        }

        void pushToOutput(string line)
        {
            outputbox.Text += line + "\n";
            scroller.ScrollToBottom();
        }

        void doCrack(ulong startat, ulong stopat, int id)
        {
            string curstring = incrementString("a", startat);
            string endpoint = incrementString("a", stopat);

            Dispatcher.Invoke(() =>
            {
                pushToOutput("Thread " + id + " scanning range "+curstring+"-"+endpoint+" (n="+startat+"-"+stopat+")");
            });

            ulong persecond = 0;

            for (ulong i = startat; i <= stopat && !done; i++)
            {
                attempts++;

                if (i % 1 == 0)
                {
                    if (!done)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            if ((ulong)getTime() - (ulong)starttime > 0) persecond = attempts / ((ulong)getTime() - (ulong)starttime);
                            outputinfo.Text = "Trying hash " + attempts + "/" + possibilities + "     " + persecond + " attempts/sec";
                            pushToOutput("Thread " + id + ": Trying " + curstring);
                        });
                    }
                }

                if (computeHash(curstring) == passwordhash)
                {
                    Dispatcher.Invoke(() =>
                    {
                        pushToOutput("Thread "+id+" found password: " + curstring);
                        done = true;
                    });
                }

                curstring = incrementString(curstring, 1);
            }

            //Dispatcher.Invoke(() =>
            //{
             //   if ((ulong)getTime() - (ulong)starttime > 0) persecond = attempts / ((ulong)getTime() - (ulong)starttime);
             //   outputinfo.Text = "Trying hash " + attempts + "/" + possibilities + "     " + persecond + " attempts/sec";
             //   pushToOutput("Thread "+id+" finished");
            //});
        }
    }
}
