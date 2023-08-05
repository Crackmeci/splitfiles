using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyNamespace;
using System.IO;

namespace SplitFiles
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            LoadFileTypes();
            timer1.Enabled = true;
            timer1.Interval = 4000;
            timer1.Start();
        }

        private void LoadFileTypes()
        {
            listBox1.Items.Clear();
            List<string> fileTypesList = SettingsManager.GetAllFileTypes();
            foreach (var type in fileTypesList)
            {
                listBox1.Items.Add(type.Trim());
            }
        }

        public void LoadDefault()
        {
            if (File.Exists("settings.json"))
            {
                File.Delete("settings.json");
            }

            AddOrUpdateFileType("Resimler", ".jpg,.png");
            AddOrUpdateFileType("Belgeler", ".pdf,.docx,.doc");
            AddOrUpdateFileType("Programlar", ".exe");
            AddOrUpdateFileType("Metinler", ".txt,.csv");
            AddOrUpdateFileType("Excel", ".xls,.xslx");
            AddOrUpdateFileType("Resimler", ".jpeg,.png,.gif,.jpg,.ico,.svg");
            AddOrUpdateFileType("Müzikler", ".wav,.mp3");
            AddOrUpdateFileType("Kodlar", ".h, .cpp, .java, .py, .cs, .php, .js, .rb, .swift, .html, .css, .xml, .sh, .pl, .kt, .lua, .ts, .r, .scala, .asm");
            AddOrUpdateFileType("Arşivler", ".zip, .rar");

            LoadFileTypes();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            string selectedFileType = listBox1.SelectedItem.ToString();
            string selectedFileTypeSettings = SettingsManager.GetFileTypeSettings(selectedFileType);

            string[] turler = selectedFileTypeSettings.Split(',');

            foreach (var tur in turler)
            {
                listBox2.Items.Add(tur.Trim());
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
            string selectedFileType = listBox1.SelectedItem.ToString();
            string selectedFileTypeSettings = SettingsManager.GetFileTypeSettings(selectedFileType);

            string[] turler = selectedFileTypeSettings.Split(',');

            List<string> array = new List<string>();

            foreach (var tur in turler)
            {
                array.Add(tur.Trim());
            }

            array.Add(textBox1.Text);

            foreach (var item in array)
            {
                listBox2.Items.Add(item);
            }

            string jsontext = string.Join(",", array);

            string fileType = listBox1.SelectedItem.ToString();
            string fileTypes = jsontext;
            AddOrUpdateFileType(fileType, fileTypes);
            Properties.Settings.Default.isDefault = false;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                AddOrUpdateFileType(textBox2.Text, "");

                LoadFileTypes();
                MessageBox.Show("Dosya türü başarıyla eklendi!", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Properties.Settings.Default.isDefault = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz türün adını yazın", "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (textBox2.Text != "")
            {
                RemoveFileType(textBox2.Text);
                MessageBox.Show("Dosya türü başarıyla silindi!", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                LoadFileTypes();
                Properties.Settings.Default.isDefault = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz türün adını yazın", "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                string selectedFileType = listBox1.SelectedItem.ToString();
                string selectedFileTypeSettings = SettingsManager.GetFileTypeSettings(selectedFileType);

                string[] turler = selectedFileTypeSettings.Split(',');

                List<string> array = new List<string>();

                foreach (var tur in turler)
                {
                    if (listBox2.SelectedItem.ToString().Trim() != tur.Trim())
                    {
                        array.Add(tur.Trim());
                    }
                }

                string jsontext = string.Join(",", array);

                string fileType = listBox1.SelectedItem.ToString();
                string fileTypes = jsontext;
                AddOrUpdateFileType(fileType, fileTypes);
                MessageBox.Show("Dosya uzantısı başarıyla silindi!", "Başarılı!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                listBox2.Items.Clear();
                foreach (var li in array)
                {
                    listBox2.Items.Add(li.ToString());
                }
                Properties.Settings.Default.isDefault = false;
                Properties.Settings.Default.Save();
            }
            else
            {
                MessageBox.Show("Lütfen silmek istediğiniz dosya uzantısını seçiniz!", "Hata!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            LoadDefault();
            Properties.Settings.Default.isDefault = true;
            Properties.Settings.Default.Save();
        }

        private void AddOrUpdateFileType(string fileType, string fileTypes)
        {
            SettingsManager.AddOrUpdateFileType(fileType, fileTypes);
        }

        private void RemoveFileType(string fileType)
        {
            SettingsManager.RemoveFileType(fileType);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (Properties.Settings.Default.isDefault == true)
            {
                label3.Text = "Orijinallik Durumu: Orijinal";
                label3.BackColor = Color.Green;
                label3.ForeColor = Color.White;
            }else
            {
                label3.Text = "Orijinallik Durumu: Değiştirilmiş";
                label3.BackColor = Color.Red;
                label3.ForeColor = Color.White;
            }

        }
    }
}
