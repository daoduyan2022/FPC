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
using Protocol;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Timers;

namespace MotionToolFPC
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class MotionTool : UserControl, INotifyPropertyChanged
    {
        private System.Timers.Timer TimerUpdateUI = new System.Timers.Timer(10);
        public Globals globals { get; set; } = null;
        public ObservableCollection<Parameter> Parameters { get; set; }
        public ObservableCollection<DataPoint> DataPoints { get; set; }

        public ScanPLC PLC = null;

        public event PropertyChangedEventHandler PropertyChanged;
        public SolidColorBrush StatusConnect { get; set; } = Brushes.Red;
        public void OnPropertyChanged(string Name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(Name));
        }
        public int _targetPoint { get; set; } = 1;
        public int _offsetX1 { get; set; } = 0;
        public int _offsetX2 { get; set; } = 0;
        public int _offsetY { get; set; } = 0;
        public int _offsetR { get; set; } = 0;
        public int TargetPoint
        {
            get { return _targetPoint; }
            set
            {
                if(value <= DataPoints.Count && value > 0)
                {
                    _targetPoint = value;
                }
                else
                {
                    _targetPoint = 1;
                }
                OnPropertyChanged();
            }
        }
        public int OffsetX1
        {
            get { return _offsetX1; }
            set { _offsetX1 = value; OnPropertyChanged(); }
        }
        public  int OffsetX2
        {
            get { return _offsetX2; }
            set { _offsetX2 = value; OnPropertyChanged(); }
        }
        public int OffsetY
        {
            get { return _offsetY; }
            set { _offsetY = value; OnPropertyChanged(); }
        }
        public int OffsetR
        {
            get { return _offsetR; }
            set { _offsetR = value; OnPropertyChanged(); }
        }



        public MotionTool()
        {
            WaittingWindow waittingWindow = new WaittingWindow();
            waittingWindow.ShowDialog();
            globals = Globals.GetInstance();
            PLC = ScanPLC.GetInstance();
            InitParameters();
            InitDataPoints();
            InitializeComponent();
            TimerUpdateUI.Elapsed += OntimedEvent;
            TimerUpdateUI.Enabled = true;
            this.DataContext = this;
        }

        private void OntimedEvent(object sender, ElapsedEventArgs e)
        {
            TimerUpdateUI.Enabled = false;
            if(globals.D0D499[499] == 1)
            {
                StatusConnect = Brushes.Green;
            }
            else
            {
                StatusConnect = Brushes.Red;
            }
            TimerUpdateUI.Enabled = true;

            OnPropertyChanged();
        }

        public void InitParameters()
        {
            if(Parameters != null)
            {
                Parameters.Clear();
            }
            string[] Axis = { "X1", "X2", "Y", "R" };
            Parameters = new ObservableCollection<Parameter>();
            for(int i=0; i<4; i++)
            {
                Parameters.Add(new Parameter(Axis[i], globals.Parameter[i], globals.Parameter[i+4],globals.Parameter[i+8],globals.Parameter[i+12],globals.Parameter[i+16],globals.Parameter[i+20], globals.Parameter[i+24]));
            }
        }
        public void InitDataPoints()
        {
            if (DataPoints != null)
            {
                DataPoints.Clear();
            }
            DataPoints = new ObservableCollection<DataPoint>();
            for(int i=0; i<globals.DataPointX1.Length; i++)
            {
                DataPoints.Add(new DataPoint(i+1, globals.DataPointX1[i], globals.DataPointX2[i], globals.DataPointY[i], globals.DataPointR[i], globals.DataSpeedX1[i], globals.DataSpeedX2[i], globals.DataSpeedY[i], globals.DataSpeedR[i]));
            }
        }

        private void dgvParameter_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            try
            {
                OnPropertyChanged();
                //int column = e.Column.DisplayIndex;
                int row = e.Row.GetIndex();
                globals.Parameter[row] = Parameters[row].HomeSpeed;
                globals.Parameter[row+8] = Parameters[row].CreepSpeed;
                globals.Parameter[row+12] = Parameters[row].JogIncreSpeed;
                globals.Parameter[row+14] = Parameters[row].JogContinuosSpeed;
                globals.Parameter[row+16] = Parameters[row].JogIncreSize;
                globals.Parameter[row+20] = Parameters[row].StratPoint;
                globals.Parameter[row+24] = Parameters[row].StartPointSpeed;
            }
            catch { }
        }

        private void PauseHome_Click(object sender, RoutedEventArgs e)
        {
            if (globals.D0D499[48] == 0)
            {
                PLC.dataSends.Add(new dataSend(new int[] { 1 }, 48));
                PLC.IsRead = false;
            }
            else
            {
                PLC.dataSends.Add(new dataSend(new int[] { 0 }, 48));
                PLC.IsRead = false;
            }
            
        }

        private void HomeAll_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 0));
            PLC.IsRead = false;
        }

        private void HomeX1X2_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 2));
            PLC.IsRead = false;
        }

        private void HomeY_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 3));
            PLC.IsRead = false;
        }

        private void HomeR_Click(object sender, RoutedEventArgs e)
        {
            PLC.dataSends.Add(new dataSend(new int[] { 1 }, 1));
            PLC.IsRead = false;
        }

        private void btnExcuteOffsetX1_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointX1.Length; i++)
            {
                globals.DataPointX1[i] += OffsetX1;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetX2_Click(object sender, RoutedEventArgs e)
        {
            int value = Convert.ToInt32(OffsetX2);
            for (int i = 0; i < globals.DataPointX2.Length; i++)
            {
                globals.DataPointX2[i] += OffsetX2;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetY_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointY.Length; i++)
            {
                globals.DataPointY[i] += OffsetY;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnExcuteOffsetR_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < globals.DataPointR.Length; i++)
            {
                globals.DataPointY[i] += OffsetR;
            }
            InitDataPoints();
            OnPropertyChanged();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            if(TargetPoint>0)
            TargetPoint--;
        }

        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            if(TargetPoint < DataPoints.Count)
            TargetPoint++;
        }

        private void btnGo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnSaveWiteDataPointToPLC_Click(object sender, RoutedEventArgs e)
        {
            globals.SaveFileModel();
        }

        private void txtTargetPoint_TextChanged(object sender, TextChangedEventArgs e)
        {
            
        }

        private void btnReadParam_Click(object sender, RoutedEventArgs e)
        {
            globals.ReadFileConfig();
            InitParameters();
            OnPropertyChanged();
        }

        private void btnWriteParam_Click(object sender, RoutedEventArgs e)
        {
            globals.SaveFileConfig();
        }
    }
}
