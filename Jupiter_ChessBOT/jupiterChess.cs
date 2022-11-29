 using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Engine
{
    public class pathway
    {
        public int move;
        public pathway child;
    }
    public partial class jupiterChess
    {
       const int totalnumberassign = 10000;
        // int Blocks=70;
        // int blocksize = 10000;
        public int branchesfound = 0;
        ConcurrentStack<int> freeboardreadyforreset = new ConcurrentStack<int>();
        Stack<int> freeboard=new Stack<int>();
        public int depthM;
        public int bestmove;
        int depthhere = 0;
        int boardhere = 0;
        List<string> sequencemoves = new List<string>();
        zoobristhasher zb = new zoobristhasher();
        public Dictionary<long, int> repeatedpositions = new Dictionary<long, int>();
        public  Dictionary<long, int> prevpathway = new Dictionary<long, int>();
        List<string> listpath = new List<string>();
        public Dictionary<long, double[]> positionslist = new Dictionary<long, double[]>();//after evaluating a position we save it here
        public Dictionary<long,int> positionBestmoves=new Dictionary<long, int>();
        public void putrepeatedposition(long keyg)
        {
            //repeatedpositions.Add(keyg, 0);
        }
        void boardresetter()
        {
            while (true)
            {
                if (freeboardreadyforreset.Count > 0)
                {
                    int f;
                    if (freeboardreadyforreset.TryPop(out f))
                    {
                        board_resetallsquares(f);
                        check_resetme(f);
                        freeboard.Push(f);
                    }
                }
                Thread.Sleep(1);
            }
        }
        object lockobj = new object();
        void pushfreeboard(int board)
        {
            freeboardreadyforreset.Push(board);
        }
        int getfreeboard()
        {
            lock (lockobj)
            {
                int res=0;
                //while(! freeboard.TryPop(out res))
                //{

                //}
                return res;
            }
        }
        const int DrawEval = -1;
      // Dictionary<long, pathway> positionspaths = new Dictionary<long, pathway>();//after evaluating a position we save it here                                                         //    string lastmove = "";
        public bool calcWithAlphabeta=true;
        public string message1="Branches and speed here ...";
        public string message2="moves found here ...";
        public int solidboard=990;
        public double search(int side,int sidetomove,int depth,int board, bool withalphabeta, double alpha = -100000000, double beta = 10000000)
        {
            
            double getevalorpush(double evalfull,long keyfull,int depthfull)
            {
                //return evalfull;
                double[] evalanddepth = { evalfull, depthfull };
                if (!positionslist.ContainsKey(keyfull))
                {
                    positionslist.Add(keyfull, evalanddepth);
                  //  positionspaths.Add(keyfull, pth);
                }
                else 
                {
                    if (positionslist[keyfull][1] < depthfull)
                    {
                        positionslist[keyfull] = evalanddepth;
                     //   positionspaths[keyfull] = pthc;
                    }
                    else
                    {
                        return positionslist[keyfull][0];
                    }
                }
                return evalfull;
            }
            double getevalorpushlite(int boardfull,int sidefull, long keyfull, int depthfull,bool aretheremoves)
            {
                //return board_eval(boardfull, sidefull, depthfull, aretheremoves, sidetomove);
                if (!aretheremoves)
                {
                    return board_eval(boardfull, sidefull, depthfull, aretheremoves, sidetomove);
                }
                if (!positionslist.ContainsKey(keyfull))
                {
                    double[] evalanddepth = { board_eval(boardfull,sidefull,depthfull, aretheremoves, sidetomove), depthfull };
                    positionslist.Add(keyfull, evalanddepth);
                    return evalanddepth[0];
                 //   positionspaths.Add(keyfull, pthc);
                }
                else
                {
                    if (positionslist[keyfull][1] < depthfull)
                    {
                        double[] evalanddepth = { board_eval(boardfull, sidefull,depthfull, aretheremoves, sidetomove), depthfull };
                        positionslist[keyfull] = evalanddepth;
                    }
                    else
                    {
                        return positionslist[keyfull][0];
                    }
                }
                return board_eval(boardfull, sidefull, depthfull, aretheremoves, sidetomove);
            }
            if (depth == depthM)
            {

                branchesfound = 0;
                if (false)
                {
                    try
                    {
                        zb = zoobristhasher.load(zb.datapath+";;");
                    }
                    catch
                    {
                        zb.generate();
                        positionslist.Clear();
                    }
                }
                else if(zb.squares_pawn==null)
                {
                    zb.generate();
                    positionslist.Clear();
                }
                else
                {
                   // positionslist.Clear();
                }
                prevpathway.Clear();
               // positionspaths.Clear();
                for (int i = 0; i < 1000 - 1; i++)
                {
                    if (i != board && i!= solidboard)
                    {
                        board_resetallsquares(i);
                        check_resetme(i);
                        freeboard.Push(i);
                    }
                }
                Thread th = new Thread(() => boardresetter());
             //   th.Start();
            }
            depthhere = depth;
            boardhere = board;
            var key = board_getkey(board, sidetomove, side);
           
            if ((repeatedpositions.ContainsKey(key) || prevpathway.ContainsKey(key)) && depth != depthM)
            {
                freeboard.Push(board);
                branchesfound++;
                return DrawEval;
            }
            prevpathway.Add(key, 0);
            if (positionslist.ContainsKey(key) && positionslist[key][1] >= depth && depth!=depthM)
            {
                //  var cdepth = positionslist[key][1];
                //if (depth > 3)
                //{
                //    var fh = positionslist[key][1];
                //}

                freeboard.Push(board);
                branchesfound++;
                prevpathway.Remove(key);
              //  pth.child = positionspaths[key];
                return positionslist[key][0];
            }
            if (depth == 1)
            {
                freeboard.Push(board);
                branchesfound++;
                //board_getallmoves(board, sidetomove);
                //var eval = board_eval(board, side);
                board_getallchecks(board, -1);
                prevpathway.Remove(key);
               // return 0;
                return getevalorpushlite(board,side,key,depth, board_arethermoves(board,sidetomove));

            }
            
            board_getallchecks(board,-1);
          //  board_getpiecethreated(board, sidetomove);
            board_getallmoves(board, sidetomove);
            int nummoves = board_getboardfeature(board, 5);
            int bestmove1 = -1;
            if (positionBestmoves.ContainsKey(key))
            {
                bestmove1 = positionBestmoves[key];
            }
            else
            {
                positionBestmoves.Add(key, bestmove1);
            }
            int[] movesindices = new int[nummoves];
            int[] capturevalues = new int[nummoves];
         //   string[] sad = new string[nummoves];//only for debugging
            for(int i = 0; i < nummoves; i++)
            {
                movesindices[i] = i;
                capturevalues[i] = -1*move_getmove_feature(board, i, 3);
                //here we will add extra value for bestmove
                if (bestmove1 == i )
                {
                    capturevalues[i] -= 1000000;//add huge value//40-50% off
                }
            //    sad[i] = move_to_string(board, i);//only for debugging
            }
            Array.Sort(capturevalues, movesindices);
            //  Array.Sort(capturevalues, sad);

            double besteval = -1000000000;
            //  move bestmove;
            if (sidetomove != side)
            {
                besteval = 1000000000;
            }
            int topmove = 0;
            //pathway bestpth = null;
            double alpha2 = alpha;
            double beta2 = beta;
            if (sidetomove != side)
            {
                alpha2 = alpha;
                beta2 = 10000000;//removing this enhances the search by 5%
            }
            else
            {
                alpha2 = -10000000;//removing this enhances the search by 5%
                beta2 = beta;
            }
            for (int t = 0; t < nummoves; t++)
            {
                int br = movesindices[t];
               // var move = move_to_string(board, br);//only for debugging
                // bool yes = false;
                //  var ls = lastmove;
                //  //var ev = 0.0;
                //if (move == "Kb7" && lastmove== "Kd3")
                //{
                //    ev = board_eval(board, side, depth, true, sidetomove);
                //    yes = true;
                //}
                //if (depth == depthM)
                //{ sequencemoves.Clear(); }
                //h
                int newboard = freeboard.Pop();// getfreeboard();
                board_copyboard(board, newboard);
                move_aplymove(newboard, br,board);
                //pathway pth3 = new pathway();
                //pth3.move = br;
                //pth.child = pth3;
               // lastmove = br;
                var eval= search(side, 1 - sidetomove, depth - 1, newboard, withalphabeta, alpha2,beta2);
                if (eval == -12345)
                {
                    if (depth == 3)
                    {
                  //      board_tostring(board, richt1);
                    }
                    return eval;
                }
                //if (move.Contains("x") )
                //{

                //}
                //if ( yes && sidetomove==1&&ev<2)
                //{
                    
                    
                //}
                
                if (sidetomove == side)
                {
                    if (eval > besteval)
                    {
                        besteval = eval;
                        // stmain.bestmove = fmove;
                        // bestmove = br;
                        //bestpth = pth3;
                        
                        topmove = br;
                   //     if (depth % 2 == 0)
                        {
                            positionBestmoves[key] = topmove;
                        }
                    }
                }
                else
                {
                    if (eval < besteval)
                    {
                        besteval = eval;
                        //bestmove = br;
                        //bestpth = pth3;
                        topmove = br;
                        //if (depth % 2 == 0)
                        {
                            positionBestmoves[key] = topmove;
                        }
                    }
                }
                //
                if (withalphabeta)
                {
                    if (sidetomove == side)
                    {
                        //it means white to move
                        if (beta2 <= eval)
                        {
                            //stmain.eval = st.eval;
                            freeboard.Push(board);
                            setbestmove(topmove, depth);
                          //  positionBestmoves[key] = topmove;
                            branchesfound++;
                            // pathway pth2 = new pathway();
                            // pth2.move = move;
                            //  pth.child = bestpth;
                            prevpathway.Remove(key);
                            //if (beta == eval)
                            //{
                            //    return besteval;
                            //}
                            return  getevalorpush(besteval, key, depth);
                        }
                        else
                        {
                            // beta2 = st.eval;
                        }
                        if (eval > alpha2)
                        {
                            alpha2 = eval;
                        }
                    }
                    else
                    {
                        if (alpha2 >= eval)
                        {
                            //stmeval = st.eval;
                            freeboard.Push(board);
                            setbestmove(topmove, depth);
                            //positionBestmoves[key] = topmove;
                            branchesfound++;
                            // pathway pth2 = new pathway();
                            // pth2.move = move;
                            //     pth.child = bestpth;
                            prevpathway.Remove(key);
                            //if (alpha == eval)
                            //{
                            //    return besteval;
                            //}
                            return getevalorpush(besteval, key,depth);
                        }
                        else
                        {
                            //   alpha2 = st.eval;
                        }
                        if (eval < beta2)
                        {
                            beta2 = eval;
                        }
                    }
                }
            }
            
            if (nummoves == 0)
            {
                branchesfound++;
                freeboard.Push(board);
                prevpathway.Remove(key);
                return getevalorpushlite(board, side, key, depth,false); //board_eval(board, side, depth);
            }
            freeboard.Push(board);
            setbestmove(topmove, depth);
            //if (depth == depthM)
            //{
            //    try
            //    {
            //        zb.saveme();
            //    }
            //    catch { }
            //}
          //  if (depth % 2 == 0)
            {
                positionBestmoves[key] = topmove;
            }
            prevpathway.Remove(key);
            //pathway pth22 = new pathway();
            //pth22.move = move_to_string(board,bestmove);
           // pth.child = bestpth;
            return getevalorpush(besteval, key, depth);
        }
       public int sidewon = -1;
       public   double directeval = 0;
        public void SearchSide(int board,int depth,int side,List<int> Boardsc,RichTextBox richTextBox1=null)
        {
            var th = this;
            {
                Stopwatch sp = new Stopwatch();
                
                if (th.zb == null)
                {
                    
                }
                th.board_setboardfeature(board, 5, 0);
                th.board_resetallsquares(board);
                th.board_putpiecesinsquares(board);
                th.check_resetme(board);
                th.board_setboardfeature(board, 5, 0);
                th.board_resetallsquares(board);
                th.check_resetme(board);
                th.board_putpiecesinsquares(board);
                
               // pathway pth = new pathway();
                th.richt1 = richTextBox1;
                try
                {
                    th.prevpathway.Add(th.board_getkey(board, side, side), 0);
                    th.repeatedpositions.Add(th.board_getkey(board, side, side), 0);
                }
                catch { }
                double eval=0;
                th.board_copyboard(board, th.solidboard);
                th.positionBestmoves.Clear();
                th.positionslist.Clear();
                for (int i = 1 + (depth) % 2; i < depth + 2; i += 2)
                {
                    
                    th.depthM = i;// depth+1;
                    th.branchesfound = 0;
                    if (i >= depth + 1)
                    {
                        sp.Start();
                    }
                    eval = th.search(side % 2, (side) % 2, th.depthM, 0, calcWithAlphabeta);
                    th.board_copyboard(th.solidboard, board);
                }
                sp.Stop();
                th.board_setboardfeature(board, 5, 0);
                th.move_aplymove(board, th.bestmove);
                
                th.board_copyboard(board, Boardsc.Count + 1000);
                Boardsc.Add(Boardsc.Count + 1000);
                var  index = Boardsc.Count + 1000;
                th.board_setboardfeature(board, 5, 0);
                //   richTextBox1.Text = "";
                if (richTextBox1 != null) {
                    richTextBox1.Text = "";
                    th.board_tostring(board, richTextBox1);
                }
                

                th.board_setboardfeature(board, 5, 0);
                th.board_resetallsquares(board);
                th.check_resetme(board);
                th.board_putpiecesinsquares(board);
                th.board_getallchecks(board, 1 - side);
                th.board_getallmoves(board, 1 - side);
                var ev= th.board_eval(board,1-side,1,th.board_arethermoves(board,side),1-side);
                directeval = ev;
                if (Math.Abs(ev) > 1000)
                {
                    
                    sidewon = (ev > 0) ? 0 : 1;
                }
                message2 = (th.board_listallmoves(board));
               // listBox1.Items.Clear();
               // listBox1.Items.AddRange(th.board_listallmoves(0).Split('\n'));
                try
                {
                       message1 = (th.branchesfound + " branches \r\n time : " + sp.ElapsedMilliseconds + " ms \r\n average speed : " + th.branchesfound / sp.ElapsedMilliseconds + " knode/sec\r\neval : " + eval + "\r\nBestmove : " + th.bestmove);
                   // var c = pth;
                   
                }
                catch { }
                moveaplied = true;
              //  cc++;
            }
        }
        public bool moveaplied=false;
        public int selectedpiece = -1;
        public   bool noexpand;
        Dictionary<string, Bitmap> imagesforboard=new Dictionary<string, Bitmap>();
        public void loadimage(string name)
        {
            Bitmap bmp = new Bitmap(name);
            imagesforboard[name] = bmp;
        }
        public void listBoard(int boardid,Panel tbl,Color black,Color white,Color selection,bool flip)
        {
            tbl.Controls.Clear();

            var height1 = tbl.Height / 8;
            var width1 =  tbl.Width / 8;

            if (!noexpand)
            {
                tbl.Height *= 12;
                tbl.Height /= 10;
            }
            else
            {
                height1 = (tbl.Height*10/12) / 8;
            }
          //  tbl.BackColor=Color.White;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    PictureBox pb = new PictureBox();
                    bool occupied=  square_getsquare_feature(boardid, i, j, 2)==1|| square_getsquare_feature(boardid, i, j, 3) == 1;
                    pb.Visible = true;
                    pb.BackColor = ((i + j) % 2 == 0) ? black : white;
                    pb.SizeMode = PictureBoxSizeMode.Zoom;
                    pb.Height = (int)(height1*1.1);
                    pb.Width = width1;
                    int piece1 = -1;
                    if (occupied)
                    {
                        var piece = square_getsquare_feature(boardid, i, j, 4);
                        int side = piece_getpiece_feature(boardid, piece, 4);
                        int type = piece_getvalue(boardid, piece);
                        piece1 = piece;
                        string imagename = "";
                        switch (type)
                        {
                            case 4:
                                imagename = "king";
                                break;
                            case 30:
                                imagename = "knight";

                                break;
                            case 32:
                                imagename = "bishop";
                                break;
                            case 50:
                                imagename = "rook";
                                break;
                            case 90:
                                imagename = "queen";
                                break;
                            case 10:
                                imagename = "pawn";
                                break;
                        }
                        if (imagename == "")
                        {

                        }
                        if (side == 1)
                        {
                            imagename += "b";
                        }
                        else if (side == 0)
                        {
                            imagename += "w";
                        }
                        string n1 = $"pieces/{imagename}.png";
                        if (imagesforboard.ContainsKey(n1))
                        {
                            pb.Image = imagesforboard[n1];
                        }
                        else
                        {
                            loadimage(n1);
                            pb.Image = imagesforboard[n1];
                        }
                       
                    }
                    else
                    {
                       
                    }
                    pb.Click += delegate
                    {
                        try
                        {
                            var mov = (moveJC)pb.Tag;
                            board_setboardfeature(boardid, 5, 0);
                            board_getallmoves(boardid, -1);
                            move_aplymove(boardid, mov.moveindex);
                            board_resetallsquares(boardid);
                            board_putpiecesinsquares(boardid);
                            board_setboardfeature(boardid, 5, 0);
                            board_resetallsquares(boardid);
                            board_putpiecesinsquares(boardid);
                            check_resetme(boardid);
                            board_setboardfeature(boardid, 5, 0);
                            board_resetallsquares(boardid);
                            check_resetme(boardid);
                            board_putpiecesinsquares(boardid);
                            noexpand = true;
                            listBoard(boardid, tbl, black, white, selection, flip);
                            moveaplied = true;
                            return;
                        }
                        catch
                        {

                        }
                        noexpand = true;
                        listBoard(boardid, tbl, black, white, selection, flip);
                        if (piece1 == -1)
                        {
                            return;
                        }
                        board_setboardfeature(boardid, 5, 0);
                        board_getallmoves(boardid, -1);
                        board_resetallsquares(boardid);
                        board_putpiecesinsquares(boardid);
                        board_setboardfeature(boardid, 5, 0);
                        board_resetallsquares(boardid);
                        board_putpiecesinsquares(boardid);
                        check_resetme(boardid);
                        board_setboardfeature(boardid, 5, 0);
                        board_resetallsquares(boardid);
                        check_resetme(boardid);
                        board_putpiecesinsquares(boardid);
                        board_getallchecks(boardid, -1);
                        var a = lispieceMove(tbl, boardid, piece1);
                        foreach (var c in tbl.Controls)
                        {
                            foreach (var c2 in a)
                            {
                                try
                                {
                                    var p = (PictureBox)c;
                                    var pos = (Point)p.Tag;
                                    if (pos.X == c2.location1 && pos.Y == c2.location2)
                                    {
                                        p.BackColor = selection;
                                        p.Tag = c2;
                                    }
                                }
                                catch { }
                            }
                        }
                    };
                    pb.Tag = new Point(i, j);
                    if (flip)
                    {
                        
                        pb.Location = new Point(height1 * (8 - i-1 ), width1 * (j ));
                    }
                    else
                    {
                        int i2 = 8 - i;
                        int j2 = 8 - j;
                        pb.Location = new Point(height1 * (8 - i2 ), width1 * (j2-1 ));
                    }
                    tbl.Controls.Add(pb);
                }
            }
        }
        string aboutmessage="Chess Bot Created by Mohamed Yasser Attwa,+201093272159" +
            "\r\nfounder of jupiter C" +
            "\r\nsoftware developer" +
            "\r\nemail:jupiterc@outlook.com" +
            "\r\nfor more see http:\\jupiterc.com";
        public string about()
        {
            return aboutmessage;
        }
        void setbestmove(int top,int depth)
        {
            if (depth == depthM)
            {
                bestmove = top;
            }
        }
    }

}
