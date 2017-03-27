using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Globalization;

namespace DJI_to_FMV
{
  public partial class Form1 : Form
    {
        string csv_load,csv_save;
        int horizontalFOV;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog diag = new OpenFileDialog();
            diag.Filter = "Csv Files|*.Csv";
            diag.ShowDialog();
            if (diag.CheckFileExists)
            {
                textBox_load.Text = diag.FileName;
                this.csv_load = diag.FileName;             
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

            SaveFileDialog saved = new SaveFileDialog();
            saved.Filter = "Csv Files|*.Csv";
            saved.DefaultExt = ".csv";
            saved.ShowDialog();
            if (!saved.CheckFileExists)
            {
                textBox_save.Text = saved.FileName;
                this.csv_save = saved.FileName;
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            var fileRead = File.OpenRead(csv_load);
            var fileSave = File.OpenWrite(csv_save);
            var reader = new StreamReader(fileRead);
            var writer = new StreamWriter(fileSave);
            string newLine;
            string timestamp = "";
            string longFormats = "yyyy/MM/dd HH:mm:ss.fff";
            DateTime myDateL,Ldate;
            DateTime myDateS,Sdate;
            List<string> list1 = new List<string>();
            int once = 1;
            while (!reader.EndOfStream)
            {        
                var line = reader.ReadLine();
                var values = line.Split(',');
                if (values[2] == "Recording")
                {
                    if (once == 2)
                    {
                        if (DateTime.TryParseExact(values[0],"yyyy/MM/dd HH:mm:ss", CultureInfo.InvariantCulture,DateTimeStyles.None , out Sdate))
                        {
                            double unixDate = DateTimeToUnixTimestamp(Sdate) * 100000;
                            timestamp = unixDate.ToString().Replace(".", "") + "0000";
                        }
                        else
                        {
                            myDateL = DateTime.ParseExact(values[0], longFormats, CultureInfo.InvariantCulture);
                            double unixDate = DateTimeToUnixTimestamp(myDateL) * 100000;
                            timestamp = unixDate.ToString().Replace(".","")+"0";
                        }
                       
                    }
                    newLine = String.Format(
                    "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                    timestamp, values[3], values[9], values[10], values[12], values[16],
                    values[17], values[18], values[65], values[66], values[67], textBox_FOV.Text);
                    if (once == 1)
                    {
                        newLine = String.Format(
                        "{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11}",
                        "TimeStamp", "PlatformTrueAirSpeed", "SensorLatitude", "SensorLongitude", "SensorAltitude", "PlatformPitch",
                        "PlatformRoll", "PlatformHeading", "SensorRelativeElevation",
                        "SensorRelativeRoll", "SensorRelativeAzimuth", "HorizontalFOV");
                    }

                    once = 2;
                    writer.WriteLine(newLine);
                }
            }

            writer.Close();
            reader.Close();
            fileRead.Close(); // ter que dar close no arquivo
            fileSave.Close(); // ter que dar close no arquivo
        }
        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            return (TimeZoneInfo.ConvertTimeToUtc(dateTime) -
                   new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;
        }
    }
}

