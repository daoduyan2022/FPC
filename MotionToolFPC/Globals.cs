using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MotionToolFPC
{
    public class Globals
    {
        public string PathConfig = @"D:\TSB_Project\FPC.Machine\FPC.Machine\bin\Debug\Config\config.txt";
        public string PathModel = @"D:\TSB_Project\FPC.Machine\FPC.Machine\bin\Debug\Config\model2108.txt";
        private static Globals globals = null;
        public static Globals GetInstance()
        
        {
            if (globals == null)
            {
                globals = new Globals();
            }
            return globals;
        }
        public  int[] D0D499 { get; set; } = new int[500];
        public  int[] D500D999 { get; set; } = new int[500];

        public  string[] strConfigData { get; set; } = new string[] { };
        public  string[] strModelData { get; set; } = new string[] { };
        public  int[] Parameter { get; set; } = new int[] { };
        public  int[] ModelData { get; set; } = new int[] { };

        public  int[] Function = new int[] { };

        public  int[] DataPointX1 { get; set; } = new int[] { };
        public  int[] DataPointX2 { get; set; } = new int[] { };
        public  int[] DataPointY { get; set; } = new int[] { };
        public  int[] DataPointR { get; set; } = new int[] { };

        public  int[] DataSpeedX1 { get; set; } = new int[] { };
        public  int[] DataSpeedX2 { get; set; } = new int[] { };
        public  int[] DataSpeedY { get; set; } = new int[] { };
        public  int[] DataSpeedR { get; set; } = new int[] { };

        public  int OffsetX1 { get; set; } = 0;
        public  int OffsetX2 { get; set; } = 0;
        public  int OffsetY { get; set; } = 0;

        public void ReadFileConfig()
        {
            try
            {
                strConfigData = File.ReadAllLines(PathConfig);
                string[] param = new string[strConfigData.Length - 2];
                Array.Copy(strConfigData, 2, param, 0, strConfigData.Length - 2);
                Parameter = param.Select(int.Parse).ToArray();
            }   
            catch {}
        }

        public void ReadFileModel()
        {
            try
            {
                strModelData = File.ReadAllLines(PathModel);
                Function = strModelData[1].Split(',').Select(int.Parse).ToArray();
                DataPointX1 = strModelData[3].Split(',').Select(int.Parse).ToArray();
                DataPointX2 = strModelData[5].Split(',').Select(int.Parse).ToArray();
                DataPointY = strModelData[7].Split(',').Select(int.Parse).ToArray();
                DataPointR = strModelData[9].Split(',').Select(int.Parse).ToArray();

                DataSpeedX1 = strModelData[17].Split(',').Select(int.Parse).ToArray();
                DataSpeedX2 = strModelData[19].Split(',').Select(int.Parse).ToArray();
                DataSpeedY = strModelData[21].Split(',').Select(int.Parse).ToArray();
                DataSpeedR = strModelData[23].Split(',').Select(int.Parse).ToArray();

                OffsetX1 = strModelData[11].Split(',').Select(int.Parse).ToArray()[0];
                OffsetX2 = strModelData[13].Split(',').Select(int.Parse).ToArray()[0];
                OffsetY = strModelData[15].Split(',').Select(int.Parse).ToArray()[0];

            }
            catch {}
        }

        public void SaveFileConfig()
        {
            string[] param = Parameter.Select(i=> i.ToString()).ToArray();
            Array.Copy(param, 0, strConfigData, 2, param.Length);
            File.WriteAllLines(PathConfig, strConfigData);
        }
        public void SaveFileModel()
        {
            strModelData[3] = String.Join(",", DataPointX1.Select(i => i.ToString()).ToArray());
            strModelData[5] = String.Join(",", DataPointX2.Select(i => i.ToString()).ToArray());
            strModelData[7] = String.Join(",", DataPointY.Select(i => i.ToString()).ToArray());
            strModelData[9] = String.Join(",", DataPointR.Select(i => i.ToString()).ToArray());

            strModelData[17] = String.Join(",", DataSpeedX1.Select(i => i.ToString()).ToArray());
            strModelData[19] = String.Join(",", DataSpeedX2.Select(i => i.ToString()).ToArray());
            strModelData[21] = String.Join(",", DataSpeedY.Select(i => i.ToString()).ToArray());
            strModelData[23] = String.Join(",", DataSpeedR.Select(i => i.ToString()).ToArray());

            File.WriteAllLines(PathModel, strModelData);

        }
    }
}
