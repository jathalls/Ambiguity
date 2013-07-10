using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Ambiguity
{
    public partial class Form1 : Form
    {
        string basefilename = "";
        double[] basesignal = new double[2048];
        double[,] Result=new double[10,2048];
        int row = 0;
        int size;
        int index;

        public Form1()
        {
            InitializeComponent();
        }

        private void genericFilter1_ProcessData(object Sender, Mitov.SignalLab.ProcessBlockNotifyArgs Args)
        {
            try
            {
               // wavePlayer1.Enabled = false;
                //wavePlayer1.Stop();
                if (!wavePlayer1.FileName.Contains('-'))
                {
                    index = -1;
                    basefilename = wavePlayer1.FileName;
                    size = (int)Args.InBuffer.GetSize();
                    if (size > 2048) size = 2048;
                    for (int i = 0; i < size; i++)
                    {
                        basesignal[i] = Args.InBuffer[i];
                    }
                    convolve(basesignal, Args.InBuffer);
                    wavePlayer1.FileName = wavePlayer1.FileName.Substring(0, wavePlayer1.FileName.Length - 4) + index + ".wav";
                    //
                    wavePlayer1.Enabled = true;
                    wavePlayer1.Start();
                }
                else
                {
                    convolve(basesignal, Args.InBuffer);
                    index--;
                    if (index < -9)
                    {
                        output(Result);
                        Application.Exit();
                    }
                    wavePlayer1.FileName = wavePlayer1.FileName.Substring(0, wavePlayer1.FileName.Length - 6) + index + ".wav";
                    //wavePlayer1.Start();
                    wavePlayer1.Enabled = true;
                    wavePlayer1.Start();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void output(double[,] Result)
        {
            File.Create(basefilename + ".csv");
            for (int i = 0; i < 10; i++)
            {

                string str = "";
                for (int j = 0; j < size; j++)
                {
                    str = str + Result[i, j] + ",";
                }
                str = str.Substring(0, str.Length - 1);
                File.AppendAllText(basefilename + ".csv", str + "\n");
            }
            

        }

        private void convolve(double[] basesignal, Mitov.SignalLab.BlockBuffer blockBuffer)
        {
            
            int samples=0;
            for (int offset = 0; offset < size; offset++)
            {
                Result[row, offset] = 0.0d;
                samples = 0;
                for (int i = 0; i < Math.Min(size, blockBuffer.GetSize()); i++)
                {
                    int offsetPos = i + offset - (size / 2);
                    if (offsetPos >= 0 && offsetPos < blockBuffer.GetSize())
                    {
                        Result[row, offset] += basesignal[i] * blockBuffer[offsetPos];
                    }
                    samples++;
                }
                Result[row, offset] = Result[row, offset] / samples;
            }
            row++;


        }
    }
}
