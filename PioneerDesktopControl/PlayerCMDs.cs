using System;

namespace PioneerDesktopControl
{

/**
 * Enum for Playback Operation.
 * @readonly
 
Command.PB_OPERATION = {

	play:		"10",
	pause:		"11",
	previous:	"12",
	next:		"13",
	rev:		"14",
	fwd:		"15",
	display:	"18",
	stop:		"20",
	enter:		"30",
	return_:	"31",
	program:	"32",
	repeat:		"34",
	random:		"35",
	topmenu:	"36",
	edit:		"37",
	ipodControl:"40"
};
**/


public class PlayerCMDs
    {
        protected string PlayCMD;
        protected string StopCMD;
        protected string PauseCMD;
        protected string NextCMD;
        protected string PrevCMD;

        protected int index = 1;

        public void SetIndex(int value)
        {
            index = value;
        }

        public virtual string getPlayCMD()
        {
            return PlayCMD;
        }

        public virtual string getStopCMD()
        {
            return StopCMD;
        }

        public string getPauseCMD()
        {
            return PauseCMD;
        }

        public virtual string getNextCMD()
        {
            return NextCMD;
        }

        public virtual string getPrevCMD()
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
            /*          PlayCMD  = "00001GHP\r\n30PB";
                        StopCMD  = "20PB";
                        PauseCMD = "11PB";
                        NextCMD  = "13PB";
                        PrevCMD  = "12PB";
            */

            PlayCMD = "00001GHP\n\n30PB";

            // 20 is stop, 31 is return
            StopCMD = "20PB\r\n31PB";  // It does not work sometimes and does not work like UP button

            // StopCMD = "36PB";  // Top Menu

            PauseCMD = "11PB";
            NextCMD = "00001GGP\n\n";
            PrevCMD = "00001GGP\n\n";
        }
        public override string getStopCMD()
        {
            index = 1;  // reset menu to item 1 when going UP
            return StopCMD;
        }

        public override string getPlayCMD()
        {
            PlayCMD = index.ToString("00000") + "GHP\n\n30PB";
            return PlayCMD;
        }

        public override string getNextCMD()
        {
            index++;
            NextCMD = (index).ToString("00000") + "GGP\n\n";
            PrevCMD = (index - 1).ToString("00000") + "GGP\n\n";

            return NextCMD;
        }

        public override string getPrevCMD()
        {
            index--;
            if (index < 1)
                index = 1;

            NextCMD = (index + 1).ToString("00000") + "GGP\n\n";
            PrevCMD = (index).ToString("00000") + "GGP\n\n";

            return PrevCMD;
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

    public class RADIO_PlayerCMDs : PlayerCMDs
    {
        public RADIO_PlayerCMDs()
        {
            PlayCMD = "";
            StopCMD = "";
            PauseCMD = "";
            NextCMD = "TPI"; // Tuner preset increment, TFI = tuner frequency increment
            PrevCMD = "TPD"; // Tuner preset decrement, TFD = tuner frequency decrement
        }
    }

}