using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MouseJester
{
    public static class Constants
    {
        internal const int GESTURE_INPUT_ID = 1;
        internal const double MATCH_THRESHOLD = 1;
        public const double GESTURE_WIDTH = 1000;
        public const double GESTURE_HEIGHT = 1000;
        public const int GESTURE_POINTS = 250;
        internal const int MIN_LINE_LEN = 5;

        internal const int WM_HOTKEY = 0x0312;

        //xml tags
        internal const string GESTURES_TAG = "GESTURES";
        internal const string GESTURE_TAG = "GESTURE";
        internal const string NAME_TAG = "NAME";
        internal const string DATA_TAG = "DATA";
        internal const string ACTIONS_TAG = "ACTIONS";
        internal const string ACTION_TAG = "ACTION";

        public const string GESTURE_FILE_NAME = "Gestures.mjg";
        internal const string APP_GUID = "2beefff3-041d-4715-8c44-012699128f27";
    }
}
