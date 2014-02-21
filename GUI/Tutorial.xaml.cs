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
using System.Windows.Shapes;

namespace Orchestra
{
    /// <summary>
    /// Interaction logic for Tutorial.xaml
    /// </summary>
    /// 

    
    public partial class Tutorial : Window
    {
        private string instruction1 = "Start gesture: \n\nDo this thing and this thing and this thing to make a start gesture!";
        private string instruction2 = "Tempo gesture: \n\nDo this thing and this thing and this thing to make a tempo gesture!";
        private string instruction3 = "Volume gesture: \n\nDo this thing and this thing and this thing to make a volume gesture!";
        private string instruction4 = "Stop gesture: \n\nDo this thing and this thing and this thing to make a stop gesture!";
        //When you want to update the color of the TargetBox to be green if they're in the right spot, just put:
        // TargetBox.Stroke = "Green";


        public Tutorial()
        {
            InitializeComponent();
            Instructions.Text = instruction1;
            //Put rectangle (TargetBox) where it needs to be:
            //TargetBox.Margin = ?
            //TargetBox.Height = ?
            //TargetBox.Width = ?

        }

        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            //set Instructions.Text = next instruction
            //
        }
    }
}
