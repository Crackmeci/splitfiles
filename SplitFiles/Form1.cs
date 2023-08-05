using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyNamespace;

namespace SplitFiles
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void addList(string folder)
        {
            string[] folders = Directory.GetDirectories(folder);
            string[] files = Directory.GetFiles(folder);

            listBox2.Items.Add("KLASÖRLER");
            listBox2.Items.Add("-------------------------------");
            foreach (string directory in folders)
            {
                DirectoryInfo folderInfo = new DirectoryInfo(directory);
                string folderName = folderInfo.Name;
                listBox2.Items.Add(folderName);
            }

            listBox1.Items.Add("DOSYALAR");
            listBox1.Items.Add("-------------------------------");
            foreach (string file in files)
            {
                FileInfo fileInfo = new FileInfo(file);
                string fileName = fileInfo.Name;
                listBox1.Items.Add(fileName);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            dialog.ShowDialog();

            string path = dialog.SelectedPath;

            textBox1.Text = path;

            listBox1.Items.Clear();
            listBox2.Items.Clear();

            addList(path);
            groupBox3.Enabled = true;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.
            groupBox3.Enabled = false;
            if (Properties.Settings.Default.isDefault != true && !File.Exists("settings.json"))
            {
                DialogResult result = MessageBox.Show("Merhaba, dosya uzantı ayarlarını varsayılana çevirmek ister misiniz?", "Onay", MessageBoxButtons.YesNo);
                Form2 frm2 = new Form2();

                if (result == DialogResult.Yes)
                {
                    frm2.LoadDefault();
                }
                else
                {
                    frm2.Show();
                }
            }
        }

        private void MoveFilesToTargetFolders(string folder)
        {
            string[] files = Directory.GetFiles(folder);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileName(file);

                string destination = GetDestinationFolder(folder, extension);

                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                if (destination != null)
                {
                    string path = Path.Combine(destination, fileName);
                    File.Move(file, path);
                }
            }
        }

        private string GetDestinationFolder(string folder, string extension)
        {
            string lowerExtension = extension.ToLower();

            List<string> fileTypesList = SettingsManager.GetAllFileTypes();
            foreach (var type in fileTypesList)
            {
                string fileTypeSettings = SettingsManager.GetFileTypeSettings(type.Trim());
                string[] tipayar = fileTypeSettings.Split(',');
                foreach (var tip in tipayar)
                {
                    if (lowerExtension.Trim() == tip.Trim())
                    {
                        return Path.Combine(folder, type.Trim());
                    }
                }

            }

            return Path.Combine(folder, "Diğer");
        }

        private void MoveDirectories(string folder)
        {
            string[] folders = Directory.GetDirectories(folder);

            string destination = Path.Combine(folder, "Klasörler");

            if (!Directory.Exists(destination))
            {
                Directory.CreateDirectory(destination);
            }

            List<string> banned = SettingsManager.GetAllFileTypes();
            banned.Add("Klasörler");
            banned.Add("Diğer");

            foreach (string directory in folders)
            {
                string folderName = Path.GetFileName(directory);
                if (!banned.Contains(folderName))
                {
                    string destinationPath = Path.Combine(destination, folderName);
                    try
                    {
                        Directory.Move(directory, destinationPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void moveFiles(string folder)
        {
            string[] files = Directory.GetFiles(folder);

            foreach (string file in files)
            {
                string extension = Path.GetExtension(file);
                string fileName = Path.GetFileName(file);

                string destination = Path.Combine(folder, extension.TrimStart('.'));
                string path = Path.Combine(destination, fileName);

                if (!Directory.Exists(destination))
                {
                    Directory.CreateDirectory(destination);
                }

                try
                {
                    File.Move(file, path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                if (radioButton1.Checked)
                {
                    moveFiles(textBox1.Text);
                    MessageBox.Show("Klasörler uzantıya başarıyla düzenlendi.", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MoveFilesToTargetFolders(textBox1.Text);
                    MessageBox.Show("Klasörler kategoriye başarıyla düzenlendi.", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                MoveDirectories(textBox1.Text);
                listBox1.Items.Clear();
                listBox2.Items.Clear();
                addList(textBox1.Text);
            }
            else
            {
                MessageBox.Show("İşlemi gerçekleştirmek için lütfen onay kutusunu işaretleyin.", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show("Seçilen dosyayı silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string selected = listBox1.SelectedItem.ToString();
                    string path = Path.Combine(textBox1.Text.ToString(), selected);

                    if (File.Exists(path))
                    {
                        try
                        {
                            File.Delete(path);
                            listBox1.Items.Remove(listBox1.SelectedItem);
                            MessageBox.Show("Dosya başarıyla silindi!", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Silinmek istenen dosya bulunamadı!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir dosya seçiniz!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show("Seçilen dosyanın ismini değiştirmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string selected = listBox1.SelectedItem.ToString();
                    string path = Path.Combine(textBox1.Text.ToString(), selected);

                    string newFileName = textBox2.Text;

                    if (!string.IsNullOrWhiteSpace(newFileName))
                    {
                        string newFilePath = Path.Combine(Path.GetDirectoryName(path), newFileName);

                        if (!File.Exists(newFilePath))
                        {
                            try
                            {
                                File.Move(path, newFilePath);
                                listBox1.Items[listBox1.SelectedIndex] = newFileName;
                                MessageBox.Show("Dosya ismi başarıyla güncellendi!", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Yeni dosya ismi zaten kullanımda!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Yeni dosya ismindeki karakterlerde bir hata oluştu!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen isim değiştirmek için bir dosya seçiniz!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void button5_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedItem != null)
            {
                DialogResult result = MessageBox.Show("Seçilen klasörü silmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string selected = listBox2.SelectedItem.ToString();
                    string path = Path.Combine(textBox1.Text.ToString(), selected);

                    if (Directory.Exists(path))
                    {
                        try
                        {
                            Directory.Delete(path);
                            listBox2.Items.Remove(listBox2.SelectedItem);
                            MessageBox.Show("Klasör başarıyla silindi!", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Silinmek istenen klasör bulunamadı!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen silmek için bir klasör seçiniz!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                DialogResult result = MessageBox.Show("Seçilen klasörün ismini değiştirmek istediğinize emin misiniz?", "Onay", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    string selected = listBox2.SelectedItem.ToString();
                    string path = Path.Combine(textBox1.Text, selected);

                    string newFileName = textBox3.Text;

                    if (!string.IsNullOrWhiteSpace(newFileName))
                    {
                        string newFilePath = Path.Combine(Path.GetDirectoryName(path), newFileName);

                        if (!Directory.Exists(newFilePath))
                        {
                            try
                            {
                                Directory.Move(path, newFilePath);
                                listBox2.Items[listBox2.SelectedIndex] = newFileName;
                                MessageBox.Show("Klasör ismi başarıyla güncellendi!", "Başarılı!!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show("Hata: " + ex.Message, "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                        else
                        {
                            MessageBox.Show("Yeni klasör ismi zaten kullanımda!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Yeni klasör ismindeki karakterlerde bir hata oluştu!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else
            {
                MessageBox.Show("Lütfen isim değiştirmek için bir klasör seçiniz!", "Hata!!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void uzantıAyarlarıToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 frm2 = new Form2();
            frm2.Show();
        }

        private void hakkımdaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Bu program Crackmeci tarafından geliştirilmiştir ©2023 Copyright","Hakkımda!",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }
    }
}
