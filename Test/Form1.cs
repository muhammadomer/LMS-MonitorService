using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        LMSMonitorService.LMSMonitorService objServiceLMS = new LMSMonitorService.LMSMonitorService();
        private void button1_Click(object sender, EventArgs e)
        {

            objServiceLMS.OnStartManual();
            


        }

        private void button2_Click(object sender, EventArgs e)
        {
            objServiceLMS.OnStopManual();
        }
    }
}
