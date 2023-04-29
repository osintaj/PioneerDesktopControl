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
        protected string IRPlayAndInSubCMD; 
        protected string IRDirUpCMD;
        protected string IRGoUpCMD; 
        protected string IRGoDwCMD; 
        protected string IR20UpCMD; 
        protected string IR20DwCMD;

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
        public virtual string getIRPlayAndInSubCMD()
        {
            return IRPlayAndInSubCMD;
        }
        public virtual string getIRDirUpCMD()
        {
            return IRDirUpCMD;
        }
        public virtual string getIRGoUpCMD()
        {
            return IRGoUpCMD;
        }
        public virtual string getIRGoDwCMD()
        {
            return IRGoDwCMD;
        }
        public virtual string getIR20UpCMD()
        {
            return IR20UpCMD;
        }
        public virtual string getIR20DwCMD()
        {
            return IR20DwCMD;
        }

        public PlayerCMDs()
        {
            PlayCMD = "";
            StopCMD = "";
            PauseCMD = "";
            NextCMD = "";
            PrevCMD = "";
            IRPlayAndInSubCMD = "";
            IRDirUpCMD = "";
            IRGoUpCMD = ""; 
            IRGoDwCMD = "";
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
            NextCMD = "00001GHP\n\n30PB";
            PrevCMD = "20PB\r\n31PB"; 
            IRPlayAndInSubCMD = "00001GHP\n\n30PB";
            IRDirUpCMD = "20PB\r\n31PB";
            IRGoUpCMD = "00001GGP\n\n";
            IRGoDwCMD = "00001GGP\n\n";
                    }
        public override string getIRDirUpCMD()
        {
            index = 1;  // reset menu to item 1 when going UP
            return IRDirUpCMD;
        }

        public override string getIRPlayAndInSubCMD()
        {
            IRPlayAndInSubCMD = "0" + index.ToString("0000") + "GHP\n\n30PB";
            return IRPlayAndInSubCMD;
        }

        public override string getIRGoDwCMD()
        {
            index++;
            
            IRGoDwCMD = "0" + index.ToString("0000") + "GGP\n\n";
            return IRGoDwCMD;
        }

        public override string getIRGoUpCMD()
        {
            index--;
            if (index < 1)
                index = 0;

            IRGoUpCMD = "0" + index.ToString("0000") + "GGP\n\n";
            
            return IRGoUpCMD;
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