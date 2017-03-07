using System.Windows.Forms;
using System.Threading;

namespace _86ME_ver2
{
    public partial class Progress : Form
    {
        public Progress()
        {
            InitializeComponent();
        }

        public bool Increase(int nValue)
        {
            if (nValue > 0)
            {
                if (progressBar1.Value + nValue < progressBar1.Maximum)
                {
                    progressBar1.Value += nValue;
                    return true;
                }
                else
                {
                    progressBar1.Maximum = 101;
                    progressBar1.Value = 101;
                    progressBar1.Maximum = 100;
                    progressBar1.Value = 100;
                    Thread.Sleep(900);
                    this.Close();
                    return false;
                }
            }
            return false;
        }
    }
}
