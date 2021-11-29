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
using System.Windows;

namespace timesaoe2v1._1
{
    public partial class Form1 : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        private string[] arrayResources = { "wood", "food", "gold", "stone" };
        private int[] arrayVillsAsignationResource = { 1, 1, 1, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2 };
        private int villCounter = 0;
        private LowLevelKeyboardHook kbh;
        string[] arrayKeyPressed = { "", "" };
        private static IDictionary<string, int> resources = new Dictionary<string, int>();
        private static IDictionary<string, dynamic> hotKeysaoe = new Dictionary<string, dynamic> { };
        //private static IDictionary<string, Func<string, bool>> hotKeysaoe = new Dictionary<string, Func<string, bool>>();
        private static IDictionary<string, dynamic> dictTest = new Dictionary<string, dynamic> { };
        private bool isIdleTC = false;
        List<dynamic> queueTC = new List<dynamic>();
        private int queueTcIndex = 0;
        /* General functions controller {*/
        private void configuringHotKeys()
        {
            dynamic hc = new System.Dynamic.ExpandoObject();
            hc.secondsLastToResearch = 15000;
            hc.onStarted = new Func<bool>(() =>
            {
                notificationVillToCreateToggle(true);
                villToCreateCount.Text = (Int32.Parse(villToCreateCount.Text) + 1).ToString();
                return true;
            });
            hc.onFinished = new Func<bool>(() =>
            {
                if (arrayVillsAsignationResource.Length < villCounter)
                {
                    string g = arrayResources[arrayVillsAsignationResource[villCounter]];
                    resources[g] += 1;
                    villCounter++;
                    refreshingCountersResources();
                    villTotalPop.Text = (Int32.Parse(villTotalPop.Text) + 1).ToString();
                    if (Int32.Parse(villToCreateCount.Text) > 0)
                    {
                        villToCreateCount.Text = (Int32.Parse(villToCreateCount.Text) - 1).ToString();
                    }
                }
                return true;
            });
            hotKeysaoe["HC"] = hc;

            dynamic ht = new System.Dynamic.ExpandoObject();
            ht.secondsLastToResearch = 15000;
            ht.onStarted = new Func<bool>(() =>
            {
                notificationVillToCreateToggle(true);
                pictureBox7.BackgroundImage = Image.FromFile("./assets/loom.png");
                pictureBox7.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox7.Visible = true;
                return true;
            });
            ht.onFinished = new Func<bool>(() =>
            {
                pictureBox7.Visible = false;
                return true;
            });
            hotKeysaoe["HT"] = ht;

            dynamic hr = new System.Dynamic.ExpandoObject();
            hr.secondsLastToResearch = 77000;
            hr.onStarted = new Func<bool>(() =>
            {
                resourcesControl1.Text = "12";
                resourcesControl2.Text = "8";
                notificationVillToCreateToggle(true);
                pictureBox7.BackgroundImage = Image.FromFile("./assets/feudal.png");
                pictureBox7.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox7.Visible = true;
                return true;
            });
            hr.onFinished = new Func<bool>(() =>
            {
                pictureBox7.Visible = false;
                return true;
            });
            hotKeysaoe["HR"] = hr;

            dynamic hy = new System.Dynamic.ExpandoObject();
            hy.secondsLastToResearch = 45000;
            hy.onStarted = new Func<bool>(() =>
            {
                notificationVillToCreateToggle(true);
                pictureBox7.BackgroundImage = Image.FromFile("./assets/wheelbarrow.png");
                pictureBox7.BackgroundImageLayout = ImageLayout.Stretch;
                pictureBox7.Visible = true;
                return true;
            });
            hy.onFinished = new Func<bool>(() =>
            {
                pictureBox7.Visible = false;
                return true;
            });
            hotKeysaoe["HY"] = hy;
        }
        private void refreshingCountersResources()
        {
            resourcesControl1.Text = resources["wood"].ToString();
            resourcesControl2.Text = resources["food"].ToString();
            resourcesControl3.Text = resources["gold"].ToString();
            resourcesControl4.Text = resources["stone"].ToString();
        }
        private void kbh_OnKeyPressed(object sender, Keys e)
        {
            System.Diagnostics.Debug.WriteLine(arrayKeyPressed.ToString());
            keyPressedOrganizer(e.ToString());
            label1.Text = string.Join(", ", arrayKeyPressed);
        }
        private void keyPressedOrganizer(string letter)
        {
            int[] a = { 1, 2 };
            if (arrayKeyPressed[arrayKeyPressed.Length - 1] != letter)
            {
                arrayKeyPressed[0] = arrayKeyPressed[a[0]];
                //arrayKeyPressed[1] = arrayKeyPressed[a[1]];
                //arrayKeyPressed[2] = arrayKeyPressed[a[2]];
                //arrayKeyPressed[3] = arrayKeyPressed[a[3]];
                arrayKeyPressed[1] = letter;
            }
            keyPressedController();
        }

        private void keyPressedController()
        {
            //have to write a object were it´s name 
            string hk = string.Join("", arrayKeyPressed);
            
            if (hotKeysaoe.ContainsKey(hk))
            {
                //hotKeysaoe[hk].onCall();
                hotKeysaoe[hk].onStarted();
                queueTC.Add(hotKeysaoe[hk]);
                writeKeyPressedLog(hk + "\n");
            }
            else
            {
                writeKeyPressedLog(arrayKeyPressed[arrayKeyPressed.Length-1] + "\n");
            }
            //else label2.Text = "no";
        }
        private void notificationVillToCreateToggle(bool original)
        {
            if (villToCreateBox.BackColor == System.Drawing.Color.Black && !original)
            {
                villToCreateBox.BackColor = System.Drawing.Color.Red;
                villToCreateCount.ForeColor = System.Drawing.Color.White;
                villToCreateCount.BackColor = System.Drawing.Color.Red;
            }
            else
            {
                villToCreateBox.BackColor = System.Drawing.Color.Black;
                villToCreateCount.ForeColor = System.Drawing.Color.Red;
                villToCreateCount.BackColor = System.Drawing.Color.Black;
            }
        }

        public void writeKeyPressedLog(string text)
        {
            string path = "./assets/log.txt";

            // This text is added only once to the file.
            if (!File.Exists(path))
            {
                // Create a file to write to.
                //string createText = "Hello and Welcome" + Environment.NewLine;
                File.WriteAllText(path, text);
            }

            // This text is always added, making the file longer over time
            // if it is not deleted.
            //string appendText = "This is extra text" + Environment.NewLine;
            File.AppendAllText(path, text);

            // Open the file to read from.
            //string readText = File.ReadAllText(path);
            //Console.WriteLine(readText);
        }
        /*} General functions controller */
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;
            configuringHotKeys();
            resources["wood"] = 0;
            resources["food"] = 3;
            resources["gold"] = 0;
            resources["stone"] = 0;
            refreshingCountersResources();
            kbh = new LowLevelKeyboardHook();
            kbh.OnKeyPressed += kbh_OnKeyPressed;
            //kbh.OnKeyUnpressed += kbh_OnKeyUnPressed;
            kbh.HookKeyboard();
        }
        /*Dragging window controller {*/
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
                System.Diagnostics.Debug.WriteLine("Good evening.");
            }
        }
        /*} Dragging window controller*/
        
        private void label2_Click(object sender, EventArgs e)
        {

        }
       
        private void Form1_FormClosing(object sender, EventArgs e)
        {
            kbh.UnHookKeyboard();
        }
        private void pictureBox3_Click(object sender, EventArgs e)
        {

        }
        
        private void pictureBox2_Click(object sender, EventArgs e)
        {

        }
        private void idleTCLoop_Tick(object sender, EventArgs e)
        {
            //label3.Text = tcActionControlLoop.Enabled.ToString();
            if (!tcActionControlLoop.Enabled)
            {
                
                if (queueTC.Count > 0)
                {
                    dynamic q = queueTC[0];
                    tcActionControlLoop.Interval = q.secondsLastToResearch;
                    
                    tcActionControlLoop.Start();
                }
                else
                {
                    notificationVillToCreateToggle(false);
                    /*System.Media.SoundPlayer player = new System.Media.SoundPlayer(System.IO.Path.Combine(System.IO.Path.GetDirectoryName(Application.ExecutablePath), @"sounds\danger.wav"));
                    player.Play();*/
                }
            }
        }

        private void tcActionControlLoop_Tick(object sender, EventArgs e)
        {

            dynamic hk = queueTC[0];
            
            //label4.Text = hk.secondsLastToResearch.ToString();
            hk.onFinished();
            //queueTcIndex++;
            queueTC.RemoveAt(0);
            //label4.Text = "Me voa detener";
            tcActionControlLoop.Stop();
        }
    }
}
