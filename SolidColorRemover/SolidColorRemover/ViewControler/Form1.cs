using System.Diagnostics;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.VisualBasic.ApplicationServices;
using SolidColorRemover.model;

namespace SolidColorRemover
{
    public partial class Form1 : Form
    {
        private Point mouse_offset;
        private Color selectedColor;
        private Image resutl;
        private List<string> cs_bench_results = new List<string>();
        private List<string> asm_bench_results = new List<string>();
        public Form1()
        {
            InitializeComponent();
            this.trackBar1.Value = Environment.ProcessorCount;
        }

        //the  Event of MouseDown, record the offset of the mouse
        private void bar_MouseDown(object sender, MouseEventArgs e)
        {
            mouse_offset = new Point(-e.X, -e.Y);
        }
        //the Event of MouseMove, move the form if user click the left button of the mouse
        private void bar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                Point mousePos = Control.MousePosition;
                mousePos.Offset(mouse_offset.X, mouse_offset.Y);
                this.Location = mousePos; //move the form to the desired location
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void toolStripContainer1_TopToolStripPanel_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {

        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {

        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            pictureBox1.Image = Image.FromFile(openFileDialog1.FileName);
            pictureBox2.Image = Image.FromFile(openFileDialog1.FileName);
            button3.Enabled = true;
            button5.Enabled = true;
            button6.Enabled = true;
        }

        private void radioButton1_CheckedChanged_1(object sender, EventArgs e)
        {

        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (radioButton1.Checked == true)
            {
                ThreadManagerCS benchCS = new ThreadManagerCS(trackBar1.Value, new Bitmap(Image.FromFile(openFileDialog1.FileName)), colorDialog1.Color, 85);
                pictureBox2.Image = benchCS.Bitmap;
                label1.Text = benchCS.TimeElapsed.TotalMilliseconds.ToString() + " ms";
                resutl = benchCS.Bitmap;
            }
            else if (radioButton2.Checked == true)
            {
                ThreadManagerASM benchASM = new ThreadManagerASM(trackBar1.Value, new Bitmap(Image.FromFile(openFileDialog1.FileName)), colorDialog1.Color, 85);
                pictureBox2.Image = benchASM.Bitmap;
                label1.Text = benchASM.TimeElapsed.TotalMilliseconds.ToString() + " ms";
                resutl = benchASM.Bitmap;
            }
            
            button8.Enabled = true;

        }

        private void button4_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            panel4.BackColor = colorDialog1.Color;

        }

        private void trackBar1_Scroll_1(object sender, EventArgs e)
        {

        }

        private void button6_Click(object sender, EventArgs e)
        {
            for (int threads = 1; threads <= 64; threads++)
            {
                    double time_result = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        ThreadManagerASM benchASM = new ThreadManagerASM(threads, new Bitmap(Image.FromFile(openFileDialog1.FileName)), colorDialog1.Color, 85);
                        time_result += benchASM.TimeElapsed.TotalMilliseconds;
                    }

                    time_result = time_result / 5;
                    asm_bench_results.Add(time_result.ToString());
            }


            for (int threads = 1; threads <= 64; threads++)
            {
                    double time_result = 0;
                    for (int i = 0; i < 5; i++)
                    {
                        ThreadManagerCS benchCS = new ThreadManagerCS(threads, new Bitmap(Image.FromFile(openFileDialog1.FileName)), colorDialog1.Color, 85);
                        time_result += benchCS.TimeElapsed.TotalMilliseconds;
                    }
                    time_result = time_result / 5;
                    cs_bench_results.Add(time_result.ToString());
            }

            button7.Enabled = true;
        }

        private void button8_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
            
        }

        private void saveFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            resutl.Save(saveFileDialog1.FileName, ImageFormat.Png);
            button8.Enabled = false;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            saveFileDialog2.ShowDialog();
        }

        private void saveFileDialog2_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            String separator = ";";
            StringBuilder output = new StringBuilder();
            String[] headings = { "Threads", "C#", "ASM" }; 
            output.AppendLine(string.Join(separator, headings));
            for (int threads = 1; threads <= 64; threads++)
            {
                String[] newLine = { threads.ToString(), cs_bench_results[threads - 1], asm_bench_results[threads - 1] };
                output.AppendLine(string.Join(separator, newLine));
            }
            try
            {
                File.AppendAllText(saveFileDialog2.FileName, output.ToString());
            }
            catch (Exception ex)
            {
                label10.Text = ex.Message;
            }

            asm_bench_results = new List<string>();
            cs_bench_results = new List<string>();
            button7.Enabled = false;
        }
    }
}