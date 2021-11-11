using System;

namespace PioneerDesktopControl
{

    public class PlayerCMDs
    {
        protected string PlayCMD;
        protected string StopCMD;
        protected string PauseCMD;
        protected string NextCMD;
        protected string PrevCMD;

        public string getPlayCMD()
        {
            return PlayCMD;
        }

        public string getStopCMD()
        {
            return StopCMD;
        }

        public string getPauseCMD()
        {
            return PauseCMD;
        }

        public string getNextCMD()
        {
            return NextCMD;
        }

        public string getPrevCMD()
        {
            return PrevCMD;
        }

        public PlayerCMDs()
        {
            PlayCMD = "";
            StopCMD = "";
            PauseCMD = "";
            NextCMD = "";
            PrevCMD = "";
        }
    }

    public class CD_PlayerCMDs : PlayerCMDs
    {
        public CD_PlayerCMDs()
        {
            PlayCMD  = "10CDP";
            StopCMD  = "20CDP";
            PauseCMD = "11CDP";
            NextCMD  = "13CDP";
            PrevCMD  = "12CDP";
        }
    }

    public class Internet_PlayerCMDs : PlayerCMDs
    {
        public Internet_PlayerCMDs()
        {
            PlayCMD  = "00001GHP\r\n30PB";
            StopCMD  = "20PB";
            PauseCMD = "11PB";
            NextCMD  = "13PB";
            PrevCMD  = "12PB";
        }
    }

    public class USB_PlayerCMDs : PlayerCMDs
    {
        public USB_PlayerCMDs()
        {
            PlayCMD  = "00001GHP\r\n30PB";
            StopCMD  = "20PB";
            PauseCMD = "11PB";
            NextCMD  = "13PB";
            PrevCMD  = "12PB";
        }
    }

}