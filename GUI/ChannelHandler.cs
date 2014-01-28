using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Animation;
using ImageUtils;
using System.Diagnostics;
using System.Timers;

namespace Orchestra
{
    public class ChannelHandler
    {
        public List<int[]> EventData = new List<int[]>();
        public int prevInst = 0;
        public int squareIndex = 0;
    }
}
