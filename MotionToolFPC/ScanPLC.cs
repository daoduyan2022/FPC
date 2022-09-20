using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Protocol;

namespace MotionToolFPC
{
    public class ScanPLC
    {
        private static ScanPLC plc = null;
        iQF fx5u = new iQF();
        public bool IsRead = true;
        public bool EnableScanPLC = true;
        public bool IsError = false;
        public List<dataSend> dataSends = new List<dataSend>();
        private Globals globals = Globals.GetInstance();


        public static ScanPLC GetInstance()
        {
            if(plc == null)
            {
                plc = new ScanPLC();
            }
            return plc;
        }
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
            while (true)
            {
                if (!EnableScanPLC)
                {
                    break;
                }

                if (IsRead)
                {
                    try
                    {
                        globals.D0D499 = fx5u.ReadHoldingRegister(0, 500);
                        globals.D500D999 = fx5u.ReadHoldingRegister(500, 500);
                    }
                    catch
                    {
                        IsError = true;
                    }
                    
                }
                else
                {
                    foreach(dataSend s in dataSends)
                    {
                        try
                        {
                            fx5u.WriteMultiRegister(s.startAddress, s.data.Length, s.data);
                        }
                        catch
                        {
                            IsError = true;
                            break;
                        }
                    }
                    IsRead = true;
                    dataSends.Clear();
                }
            }        
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
