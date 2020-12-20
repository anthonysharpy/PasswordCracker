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

        public MainWindow()
        {
            InitializeComponent();

            passwordbox = (TextBox)FindName("password_box");
            hashbox = (TextBlock)FindName("hash_box");
            outputbox = (TextBlock)FindName("output_text");
            outputinfo = (TextBlock)FindName("output_info");

            ThreadPool.SetMaxThreads(12, 12);
            ThreadPool.SetMinThreads(12, 12);
        }

        static string computeHash(string input, SHA256 thesha)
        {
            thesha.ComputeHash(Encoding.ASCII.GetBytes(input));

            string output = "";

            for(int i = 0; i < 32; i++) // sha256 returns 32 bytes
            {
                output += (int)thesha.Hash[i];
            }

            return output;
        }

        ulong howManyPossiblePasswords(int passwordlength)
        {
            return (ulong)Math.Pow(26, passwordlength);
        }

        static int getTime()
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

            SHA256 tempsha = SHA256.Create();

            password = passwordbox.Text;
            passwordhash = computeHash(password, tempsha);
            attempts = 0;
            outputbox.Text = "";


            if(!validPassword(password))
            {
                hashbox.Text = "ERROR: password must only contain lowercase letters";
                return;
            }
            if(password.Length > 10)
            {
                hashbox.Text = "ERROR: password can be a maximum of 10 characters";
                return;
            }

            hashbox.Text = "Your SHA256 hash: " + passwordhash;

            int threads = 12; // because my CPU has 12 threads.

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

        static string incrementStringQuick(string currentstring, ulong times)
        {
            if (times == 0) return currentstring;

            char[] output = new char[10];
            int[] results = new int[10];
            ulong total = 0;

            /*
            last char = worth 1
            second to last = worth 26
            third to last = 676
            17576
            456976
            11881376
            308915776
            8031810176
            208827064576
            5429503678976

            */

            // could do this as a loop
            results[0] = (int)Math.Floor((decimal)(times / (ulong)5429503678976));
            total += 5429503678976 * (ulong)results[0];
            results[1] = (int)Math.Floor((decimal)((times-total) / (ulong)208827064576));
            total += 208827064576 * (ulong)results[1];
            results[2] = (int)Math.Floor((decimal)((times - total) / (ulong)8031810176));
            total += 8031810176 * (ulong)results[2];
            results[3] = (int)Math.Floor((decimal)((times - total) / (ulong)308915776));
            total += 308915776 * (ulong)results[3];
            results[4] = (int)Math.Floor((decimal)((times - total) / (ulong)11881376));
            total += 11881376 * (ulong)results[4];
            results[5] = (int)Math.Floor((decimal)((times - total) / (ulong)456976));
            total += 456976 * (ulong)results[5];
            results[6] = (int)Math.Floor((decimal)((times - total) / (ulong)17576));
            total += 17576 * (ulong)results[6];
            results[7] = (int)Math.Floor((decimal)((times - total) / (ulong)676));
            total += 676 * (ulong)results[7];
            results[8] = (int)Math.Floor((decimal)((times - total) / (ulong)26));
            total += 26 * (ulong)results[8];
            results[9] = (int)(times - total);

            string finalstring = "";
            bool startedstring = false;

            for(int i = 0; i < 10; i++)
            {
                if (results[i] > 0 || startedstring)
                {
                    startedstring = true;
                    finalstring += (char)('a' + results[i]); // a actually represents 0            
                }
            }

            return finalstring;
        }

        static string incrementString(string currentstring)
        {
            bool addnewchar;

            char[] chararray = currentstring.ToCharArray();
            addnewchar = false;

            int i = chararray.Length - 1;
            chararray[i] = (char)(chararray[i] + 1); // increments last character

            int x = chararray.Length - 1;

rescan:
            for (; x > -1; x--)
            {
                if (chararray[x] == '{') // { is 1 after z
                {
                    // reset this char

                    chararray[x] = 'a';

                    // increment previous char

                    if (x == 0)
                    {
                        addnewchar = true; // was first char, so we just need to add a new one on later
                        goto end; // very very slightly faster because otherwise we'd rescan the whole thing
                    }
                    else chararray[x - 1] = (char)((int)chararray[x - 1] + 1); // increment previous char

                    goto rescan; // faster than setting a variable probably ... the alternative was using a while loop with a variable that was set to true if we need to rescan, then doing while(needtorescan). setting and getting that variable adds overhead, this is faster.
                }
            }
                
end:
            currentstring = new string(chararray);
            if (addnewchar) currentstring = "a" + currentstring;

            return currentstring;
        }

        void pushToOutput(string line)
        {
            outputbox.Text += line + "\n";
            scroller.ScrollToBottom();
        }

        void doCrack(ulong startat, ulong stopat, int id)
        {
            string curstring = incrementStringQuick("a", startat);
            string endpoint = incrementStringQuick("a", stopat);

            SHA256 oursha = SHA256.Create();

            Dispatcher.Invoke(() =>
            {
                pushToOutput("Thread " + id + " scanning range "+ curstring + "-"+endpoint+" (n="+ String.Format("{0:n0}", startat)+"-"+String.Format("{0:n0}", stopat)+")");
            });

            ulong persecond = 0;
            ulong completiontime = 0;

            for (ulong i = startat; i <= stopat && !done; i++)
            {
                if (computeHash(curstring, oursha).Equals(passwordhash))
                {
                    Dispatcher.Invoke(() =>
                    {
                        foundpasswordtextbox.Text = "Found password: "+curstring;
                        pushToOutput("Thread " + id + " found password: " + curstring);
                    });

                    done = true;
                    return;
                }

                if (i % 50000 == 0)
                {
                    if (i != 0)
                    {
                        attempts += 50000;

                        if ((ulong)getTime() - (ulong)starttime > 0) persecond = attempts / ((ulong)getTime() - (ulong)starttime);
                        if (persecond > 0) completiontime = possibilities / persecond;

                        Dispatcher.Invoke(() =>
                        {
                            outputinfo.Text = "Trying hash " + String.Format("{0:n0}", attempts) + "/" + String.Format("{0:n0}", possibilities) + "     " + String.Format("{0:n0}", persecond) + " attempts/sec    Time left until guaranteed completion: " + String.Format("{0:n0}", (((ulong)starttime + completiontime) - (ulong)getTime())) + " seconds";
                            pushToOutput("Thread " + id + ": Trying " + curstring);
                        });
                    }
                }

                curstring = incrementString(curstring);
            }

            if (done)
            {
                Dispatcher.Invoke(() =>
                {
                    pushToOutput("Thread " + id + " finished (password already found)");
                });
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    pushToOutput("Thread " + id + " finished (ran out of passwords to try)");
                });
            }
        }
    }
}
