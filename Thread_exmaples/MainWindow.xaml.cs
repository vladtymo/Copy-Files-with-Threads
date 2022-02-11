using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using PropertyChanged;

namespace Thread_exmaples
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ObservableCollection<CopyFileProcessInfo> processes = new ObservableCollection<CopyFileProcessInfo>();
        public MainWindow()
        {
            InitializeComponent();

            processesLb.ItemsSource = processes;
        }

        private void CopyButton_Click(object sender, RoutedEventArgs e)
        {
            string fileFrom = fromTb.Text;
            string fileTo = toTb.Text;

            if (!File.Exists(fileFrom))
            {
                MessageBox.Show("File does not exists! Please, enter a valid path.", "File does not exists", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var info = new CopyFileProcessInfo()
            {
                FromPath = fileFrom,
                ToPath = fileTo,
                FileName = System.IO.Path.GetFileName(fileFrom),
                DestDirectory = fileTo,
                Percent = 0
            };

            processes.Add(info);

            Thread copyThread = new Thread(CopyFile);
            copyThread.Start(info);
        }

        private void CopyFile(object data)
        {
            var info = data as CopyFileProcessInfo;

            // Method Copy()
            //File.Copy(info.Item1, info.Item2);

            // Byte Array
            using (FileStream fileFrom = new FileStream(info.FromPath, FileMode.Open))
            {
                using (FileStream toFile = new FileStream(info.ToPath, FileMode.Create))
                {
                    long totalBytes = fileFrom.Length;
                    long copiedBytes = 0;

                    byte[] buffer = new byte[1024 * 1024 * 8]; // 8KB

                    int count = 0;
                    do
                    {
                        count = fileFrom.Read(buffer, 0, buffer.Length);
                        toFile.Write(buffer, 0, count);
                        copiedBytes += count;

                        info.Percent = (float)(copiedBytes / (totalBytes / 100M));
                    } while (count > 0);
                }
            }

            MessageBox.Show("Process completed!");
        }
    }

    [AddINotifyPropertyChangedInterface]
    public class CopyFileProcessInfo
    {
        public string FromPath { get; set; }
        public string ToPath { get; set; }
        public string FileName { get; set; }
        public string DestDirectory { get; set; }
        public float Percent { get; set; }
    }
}
