using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Protocol;

namespace MotionToolFPC
{
    public class ScanPLC
    {
        iQF fx5u = new iQF();
        public bool IsRead = true;
        public bool EnableScanPLC = true;
        public bool IsError = false;
        public List<dataSend> dataSends = new List<dataSend>();
        private Globals globals = Globals.GetInstance();
        private System.Timers.Timer mTimer = new System.Timers.Timer(100);
        public bool ConnectPLC(string IpAddress, int port)
        {
            try
            {
                IsError = fx5u.Connect(IpAddress, port);
            }
            catch
            {
                IsError = true;
            }
            
            return IsError;
        }
        public void DisconnectPLC(string IpAddress, int port)
        {
            fx5u.Disconnect();
        }
        public void StartScan()
        {
            mTimer.Enabled = true;
            mTimer.Elapsed += Scan;
        }
        private void Scan(object sender, ElapsedEventArgs e)
        {

            mTimer.Enabled = false;
            Console.WriteLine("============================ running ==============================");
            try
            {
                if (!EnableScanPLC)
                {
                    mTimer.Enabled = false;
                }

                if (IsRead)
                {
                    globals.D0D499 = fx5u.ReadHoldingRegister(0, 500);
                    globals.D500D999 = fx5u.ReadHoldingRegister(500, 500);
                }
                else
                {
                    foreach (dataSend s in dataSends)
                    {
                        try
                        {
                            fx5u.WriteMultiRegister(s.startAddress, s.data.Length, s.data);
                        }
                        catch
                        {
                            break;
                        }
                    }
                    IsRead = true;
                    dataSends.Clear();
                }
            }
            catch
            {
                IsError=true;
            }
            mTimer.Enabled = true;
        }
    }


    public class dataSend
    {
        public int[] data { get; set; }
        public int startAddress { get; set; }
        public dataSend(int[] data, int startAddress)
        {
            this.data = data;
            this.startAddress = startAddress;
        }
    }

}
