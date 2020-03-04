using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Permissions;
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
using System.ComponentModel;
using System.Threading;
using System.ComponentModel;

namespace VideoPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        public static string[] urls;
        public static Uri[] videos;
        public static int counter=0;
        BackgroundWorker bw = new BackgroundWorker();


        public MainWindow()
        {
            InitializeComponent();
            urls = System.IO.File.ReadAllLines(@"C:\Users\csoria\source\repos\VideoPlayer\VideoPlayer\urls.txt");
            bw.DoWork += worker_Dowork;
            bw.RunWorkerCompleted += bw_RunworkerCompleted;
            //Create a string array with the lines of urls.txt
        }

        private delegate void mydelegate(int i);

        private void display(int i)
        {
            if (i == 10) {
                progress.Content = "Downloading Complete";
                progress.Visibility = Visibility.Collapsed;
                btn.Visibility = Visibility.Collapsed;
            }
            else
                progress.Content = "Downloading: " + i.ToString() + "/" + (urls.Length-1);
        }

        private void worker_Dowork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(2000);
            mydelegate deli = new mydelegate(display);
            WebClient web = new WebClient();
            //loop through the urls and download each file
            for (int i = 0; i < urls.Length - 1; i++)
            {
                progress.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, deli, i);
                //progress.Content = "Downloading " + i + "/" + urls.Length;
                string[] fileName = urls[i].Split('/');
                string path = @"C:\Users\csoria\source\repos\VideoPlayer\VideoPlayer\videos\video " + fileName[fileName.Length - 1];
                if (!File.Exists(path))
                    web.DownloadFile(urls[i], path);

            }
            progress.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Normal, deli, 10);
        }


        private void bw_RunworkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            PlayVideos();
        }

        public void PlayVideos()
        {
            //Convert each string to a Uri (which is the appropriate type for Media Element)
            videos = new Uri[urls.Length - 1];
            int counter = 0;
            for (int i = 0; i < videos.Length; i++)
            {
                string[] fileName = urls[i].Split('/');
                videos[i] = new Uri(@"C:\Users\csoria\source\repos\VideoPlayer\VideoPlayer\videos\video " + fileName[fileName.Length - 1]);
            }

            //Instead of solution above, process all files in the directory already to make sure you're getting all instead of just from the url txt


            VideoPlayer.Source = videos[counter];

        }

        public void Play_Next(object sender, RoutedEventArgs e)
        {
            if (videos.Length-2 < counter)
            {
                counter = 0;
                VideoPlayer.Source = videos[counter];
            }
            else
            {
                counter++;
                VideoPlayer.Source = videos[counter];
            }
                
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bw.RunWorkerAsync();
        }
    }
}
