using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jupiter_ChessBOT
{
    public partial class ChessBoard : Form
    {
        public ChessBoard()
        {
            InitializeComponent();
        }
        int boardid = 0;
        Engine.jupiterChess jc;
        enum Player { human,computer}
        Player player1;
        Player player2;
        int player1timems=90;
        int player2timems=90;
        private void ChessBoard_Load(object sender, EventArgs e)
        {
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 1;
        }
        bool done=false;
        public bool humanplayed = false;
        bool working = false;
        int depth =Properties.Settings.Default.depth;
        List<int> boards = new List<int>();
        public void ThreadCalc(int side)
        {
            working = true;
            //jc.board_getpiecethreated(boardid, 1 - side);
            jc.SearchSide(boardid, depth, side, boards);
           
            Thread.Sleep(2000);
            working = false;
            done = true;
        }
        Thread th ;
        int toggle = -1;
        void playerPlay() { 
            if(side%2 == 0)
            {
                
                toggle = 0;
                if (player1 == Player.human)
                {
                   
                    return;
                }
                working = true;
                jc.moveaplied = false;
                if (player1 == Player.computer)
                {
                    th = new Thread(() => ThreadCalc(0));
                    th.Start();
                }
                 
            }
            else
            {
                
                toggle = 1;
                if (player2 == Player.human)
                {
                    return;
                }
                working = true;
                jc.moveaplied = false;
                if (player2 == Player.computer)
                {
                    th = new Thread(() => ThreadCalc(1));
                    th.Start();
                }
            }
            
        }
        int side = 0;
        string show_time(int tms)
        {
            tms = (tms > 0) ? tms : 0;
            int mins = tms / 60000;
            int secs = (tms - (mins * 60000)) / 1000;
            int ms = (tms - (mins * 60000) - secs * 1000)/10;
            if (mins == 0 && secs < 30)
            {
                return $"{mins.ToString("00")}:{secs.ToString("00")}.{ms.ToString("00")}";
            }
            return $"{mins.ToString("00")}:{secs.ToString("00")}";
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            label6.Text = jc.piecethreatened+"";
            label7.Text = jc.piecethreatenedValue+"";
            label4.Text = Math.Round( jc.directeval,2)+"";
            if (jc.sidewon != -1)
            {

                label5.Text = (jc.sidewon == 0) ? "White won" : "Black won";
            }
            if (jc.moveaplied)
            {
                jc.noexpand = true;
                jc.listBoard(boardid, panel1, Color.Brown, Color.Wheat, Color.Gray, flip);

            }
            if (jc.moveaplied  )
            {
                side++;
                jc.moveaplied = false;
            }
            if (!working)
            {
               playerPlay();
            }
            textBox1.Text = show_time(player1timems);
            textBox2.Text = show_time(player2timems);
            richTextBox1.Text = jc.message1;// +"\r\n"+jc.message2;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            player1 = (Player)comboBox1.SelectedIndex;
            player2 = (Player)comboBox2.SelectedIndex;
        }
        bool firsttime = true;
        private void button4_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            timer2.Enabled = timer1.Enabled;
            if (timer2.Enabled)
            {
                jc = new Engine.jupiterChess();
                jc.Board_init_board(boardid);
                if (!firsttime)
                {
                    jc.noexpand = true;
                }
                firsttime = false;
                jc.listBoard(boardid, panel1, Color.Brown, Color.Wheat, Color.Gray, flip);
                try
                {
                    player1timems = Convert.ToInt32(Convert.ToDouble(textBox1.Text) * 60000);
                    player2timems = Convert.ToInt32(Convert.ToDouble(textBox2.Text) * 60000);
                }
                catch { }
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (toggle == 0)
            {
                player1timems -= timer2.Interval;
            }
            else if (toggle == 1)
            {
                player2timems -= timer2.Interval;
            }
        }

        private void ChessBoard_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                th.Abort();
            }
            catch { }
        }
        bool flip;
        private void button5_Click(object sender, EventArgs e)
        {
            flip = !flip;
            jc.noexpand = true;
            jc.listBoard(boardid, panel1, Color.Brown, Color.Wheat, Color.Gray, flip);
        }
       
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            depth = (int)(numericUpDown1.Value);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            working = true;
            jc.moveaplied = false;
            if (player1 == Player.computer)
            {
                th = new Thread(() => ThreadCalc(0));
                th.Start();
            }
        }
    }
}
