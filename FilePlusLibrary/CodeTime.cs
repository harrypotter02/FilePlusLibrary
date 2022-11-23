using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilePlusLibrary
{
    public class CodeTime //3.22_9
    {
        //private string tag = "";
        private DateTime t1 = DateTime.Now;
        private DateTime t2 = DateTime.Now;
        private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);//unix時間

        //設定程式開始計算時間起始點
        public void SetTime1()
        {
            t1 = DateTime.Now;
        }

        //設定程式開始計算時間結束點
        //取得時間差(ms),回傳值string
        public string GetTimeDiffStr()
        {
            t2 = DateTime.Now;
            TimeSpan deltT = t2 - t1;
            string ret = string.Format("{0:F3}sec", deltT.TotalSeconds);
            return ret;
        }

        //設定程式開始計算時間結束點
        //取得時間差(ms),回傳值double
        public double GetTimeDiff()//3.11_17
        {
            t2 = DateTime.Now;
            TimeSpan deltT = t2 - t1;

            return deltT.TotalSeconds;
        }

        public double GetTimeDiffMs()//3.22_10
        {
            t2 = DateTime.Now;
            TimeSpan deltT = t2 - t1;

            return deltT.TotalMilliseconds;
        }

        //取得unix時間開始到現在的時間差(ms)
        public long GetTickMs()//3.22_9
        {
            return (long)(DateTime.UtcNow - UnixEpoch).TotalMilliseconds;
        }
        
    }

}
