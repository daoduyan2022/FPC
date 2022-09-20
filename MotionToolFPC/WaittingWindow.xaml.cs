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
using System.Windows.Shapes;
using System.Threading;
using System.IO;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for WaittingWindow.xaml
    /// </summary>
    public partial class WaittingWindow : Window
    {
        string[] order = new string[] { "Load File Config", "Load File Model", "Connect to PLC", "Start Scan PLC", "Ending" };
        bool Kill = false;
        private System.Timers.Timer mTimer = new System.Timers.Timer(1000);
        Globals globals = Globals.GetInstance();
        
        public WaittingWindow()
        {
            InitializeComponent();
            
            
        }
        private void OntimedEvent(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (Kill)
            {
                this.Dispatcher.Invoke(() =>
                {
                    this.Close();
                });
            }
        }
        public void Init()
        {
            for (int i = 0; i < order.Length; i++)
            {
                this.Dispatcher.Invoke(() =>
                {
                    lbStatus.Content = order[i];
                });
                if (i == 0)
                {
                    globals.ReadFileConfig();
                }
                if (i == 1)
                {
                    globals.ReadFileModel();
                }
                if (i == 2)
                {
                    //Thread PLCScaner = new Thread(() =>
                    //{
                    ScanPLC PLC = new ScanPLC();
                    PLC.ConnectPLC("192.168.3.3", 9000);
                    //PLC.StartScan();
                    //});
                    //PLCScaner.IsBackground = true;
                    //Thread.Sleep(50);
                    //PLCScaner.Start();

                }
                if (i == 3)
                {

                }
                    
                if (i == 4)
                {
                    
                    Thread.Sleep(2000);
                }
            }
            this.Dispatcher.Invoke(() =>
            {
                this.Close();
            });

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Thread loadData = new Thread(new ThreadStart(Init));
            loadData.Start();
            loadData.IsBackground = true;
        }
        
    }
}
