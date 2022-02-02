using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyGuru_v2
{
    public partial class Form1 : Form
    {
        private static string Source = "";
        private static string Target = "";
        private static string OneType = "*.*";
        private static bool going_task = false;
        private static SearchOption mode = SearchOption.AllDirectories;

        public Form1()
        {
            InitializeComponent();
        }

        private void Btn_browse_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog open = new FolderBrowserDialog())
            {
                open.ShowDialog();
                Source = open.SelectedPath;
                txtbox_sourcePath.Text = open.SelectedPath;
            }
        }

        private void Btn_target_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog open = new FolderBrowserDialog())
            {
                open.ShowDialog();
                Target = open.SelectedPath;
                txtbox_targetPath.Text = open.SelectedPath;
            }
        }

        private void Btn_start_stop_Click(object sender, EventArgs e)
        {

            if (Source != "")
            {
                if (Target != "")
                {
                    if (going_task == false)
                    {
                        //make a thread and start it 
                        Task copy = new Task(DoIt);
                        copy.Start();
                        btn_start_stop.Text = "Stop";
                        //disable the controll tools
                        txtbox_sourcePath.Enabled = false;
                        txtbox_targetPath.Enabled = false;
                        txtbox_type.Enabled = false;
                        chbox_copyOneType.Enabled = false;
                        chbox_delete.Enabled = false;
                        chbox_onePlace.Enabled = false;
                        btn_browse.Enabled = false;
                        btn_target.Enabled = false;
                        copyOnlyTop_chbox.Enabled = false;
                    }
                    else
                    {
                        //stop token to Task
                        going_task = false;
                        btn_start_stop.Text = "Start";
                        //Enable the controll tools
                        txtbox_sourcePath.Enabled = true;
                        txtbox_targetPath.Enabled = true;
                        txtbox_type.Enabled = true;
                        chbox_copyOneType.Enabled = true;
                        chbox_delete.Enabled = true;
                        chbox_onePlace.Enabled = true;
                        btn_target.Enabled = true;
                        btn_browse.Enabled = true;
                        copyOnlyTop_chbox.Enabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("Need target path!");
                }
            }
            else
            {
                MessageBox.Show("Need source path!");
            }
        }

        private void DoIt()
        {
            going_task = true;
            do
            {
                //if copy only the top level of directory
                if (copyOnlyTop_chbox.Checked)
                {
                    mode = SearchOption.TopDirectoryOnly;
                }
                else
                {
                    mode = SearchOption.AllDirectories;
                }

                //if we only want one type
                if (chbox_copyOneType.Checked)
                {
                    OneType = "*."+txtbox_type.Text;
                    if (txtbox_type.Text == "")
                    {
                        OneType = "*.*";
                    }
                }
                else//set it to default
                {
                    OneType = "*.*";
                }

                if (chbox_onePlace.Checked)
                {
                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(Source, OneType, mode))
                    {
                        Thread.Sleep((int)numericUpDown1.Value);
                        //copy only the not existing
                        if (File.Exists(newPath.Replace(Source, Target)))
                        {
                            //do nothing if it's exist
                        }
                        else
                        {
                            try
                            {
                                //copy it to target
                                Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        using (OpenFileDialog open = new OpenFileDialog())
                                        {
                                            open.FileName = newPath;
                                            string a = Target + "\\" + open.SafeFileName;
                                            File.Copy(newPath, a, true);
                                        }
                                        if (chbox_delete.Checked)//if checkboy is checked, then delete
                                        {
                                            File.Delete(newPath);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }

                                });
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }
                else
                {
                    //Now Create all of the directories
                    foreach (string dirPath in Directory.GetDirectories(Source, "*", mode))
                    {
                        try
                        {
                            Directory.CreateDirectory(dirPath.Replace(Source, Target));
                        }
                        catch (Exception)
                        {

                        }

                    }

                    //Copy all the files & Replaces any files with the same name
                    foreach (string newPath in Directory.GetFiles(Source, OneType, mode))
                    {
                        Thread.Sleep((int)numericUpDown1.Value);
                        //copy only the not existing
                        if (File.Exists(newPath.Replace(Source, Target)))
                        {
                            //do nothing if it's exist
                        }
                        else
                        {
                            try
                            {
                                //copy it to target
                                Task.Factory.StartNew(() =>
                                {
                                    try
                                    {
                                        File.Copy(newPath, newPath.Replace(Source, Target), true);
                                        if (chbox_delete.Checked)//if checkboy is checked, then delete
                                        {
                                            File.Delete(newPath);
                                        }
                                    }
                                    catch (Exception)
                                    {

                                    }

                                });
                            }
                            catch (Exception)
                            {

                            }

                        }
                    }
                }
            } while (going_task == true);
            MessageBox.Show("Copying Stopped!");
        }
    }
}
