using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    partial class jupiterChess
    {
        //now pieces
        const int totalPiecesFeatures = 7;
        //type,col,row,exists,side,nevermoved,longmovefirst
        //type 0 pawn , 1 rook,2 knight,3 bishop , 4 queen , 5 king
        int[] pieces;
        int piecesperboard = 32;
        public void setpieces()
        {
            //assign pieces
            pieces = new int[totalnumberassign* piecesperboard * totalPiecesFeatures];
            //type,col,row,exists,side,nevermoved,longmovefirst
        }
        public int piece_getvalue(int board,int piece)
        {
            switch (piece_getpiece_feature(board, piece, 0))
            {
                case 0:
                    return 10;// +piece_getpiece_feature(board,piece,1);
                case 1:
                    return 50;
                case 2:
                    return 30;
                case 3:
                    return 32;
                case 4:
                    return 90;
                case 5:
                    return 4;
            }
            return 0;
        }
        void piece_create_piece(int board,int piece,int type,int col,int row,int side)
        {
            var a = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            pieces[a] = type;
            pieces[a  + 1] = col;
            pieces[a +  2] = row;
            pieces[a +  3] = 1;
            pieces[a + 4] = side;
            pieces[a + 5] = 1;
            pieces[a +  6] = 0;
            squaresetoccupy(board, piece, col, row);
        }
        void piece_copytonewpiece(int board,int newboard,int piece)
        {
            var a = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            var a2 = newboard * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            if (pieces[a + 3] == -1)
            {
                pieces[a2 + 3] = -1;
                return;
            }
            else
            {
                pieces[a2] = pieces[a];
                pieces[a2 + 1] = pieces[a+1];
                pieces[a2 + 2] = pieces[a + 2];
                pieces[a2 + 3] = pieces[a + 3];
                pieces[a2 + 4] = pieces[a + 4];
                pieces[a2 + 5] = pieces[a + 5];
                pieces[a2 + 6] = 0;
                piece_putpieceinsquare(newboard, piece);
            }
        }
        void piece_putpieceinsquare(int board,int piece)
        {
            var a = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            var col = pieces[a + 1];
            var row = pieces[a + 2];
            int side = pieces[a + 4];
            if (pieces[a] == 5)
            {
                check_setking(board,side, col, row);
            }
            squaresetoccupy(board, piece, col, row);
        }
        void piece_removepiece(int board,int piece)
        {
            var part = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            pieces[part+ 3] = -1;
            var col = pieces[part + 1];
            var row = pieces[part + 2];
            square_freesquare(board, col, row);
        }
        public string piece_todraw(int board, int piece)
        {
            switch (piece_getpiece_feature(board, piece, 0))
            {
                case 0:
                    return "♙";
                case 1:
                    return "♖";
                case 2:
                    return "♘";
                case 3:
                    return "♗";
                case 4:
                    return "♕";
                case 5:
                    return "♔"; }
            return "";
        }
        public string piece_to_string(int board,int piece)
        {
            switch (piece_getpiece_feature(board, piece, 0))
            {
                case 0:
                    return " P ";
                case 1:
                    return " R ";
                case 2:
                    return " N ";
                case 3:
                    return " B ";
                case 4:
                    return " Q ";
                case 5:
                    return " K ";
            }
            /* case 0:
                    return " ♙ ";
                case 1:
                    return " ♖ ";
                case 2:
                    return " ♘ ";
                case 3:
                    return " ♗ ";
                case 4:
                    return " ♕ ";
                case 5:
                    return " ♔ ";*/
            return "";

        }
        ///<summary>
        ///features
        ///0=type,1=col,2=row,3=exists,4=side,5=nevermoved,6=longmovefirst
        ///</summary>
        public int piece_getpiece_feature(int board,int piece,int feature)
        {
            return pieces[board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures + feature];
        }
        ///<summary>
        ///features
        ///0=type,1=col,2=row,3=exists,4=side,5=nevermoved,6=longmovefirst
        ///</summary>
        void piece_setpiece_feature(int board,int piece,int feature,int value)
        {
             pieces[board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures + feature]=value;
        }
        public void piece_getchecks(int board,int piece)
        {
            var type = piece_getpiece_feature(board, piece, 0);
            switch (type)
            {
                case 0:
                    piece_getpawnchecks(board, piece);
                    break;
                case 1:
                    piece_getrookchecks(board, piece,true);
                    
                    break;
                case 2:
                    piece_getknightchecks(board, piece);
                    break;
                case 3:
                    piece_getbishopchecks(board, piece,true);
                    break;
                case 4:
                    //queen
                    int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
                    queencol = pieces[pointerP + 1];  //col
                    queenrow = pieces[pointerP + 2];  //row
                    piece_getrookchecks(board, piece,false);
                    piece_getbishopchecks(board, piece,false);
                    break;
                case 5:
                    piece_getkingchecks(board, piece);
                    break;
            }
        }
        void pieceChangePosition(int board,int piece,int col,int row)
        {
            var part1 = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            var col1 = pieces[part1 + 1];
            var row1 = pieces[part1 + 2];
            square_freesquare(board, col1, row1);
            var p = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            pieces[p+1] = col;
            pieces[p+2] = row;
            pieces[p+5] = 0;
            squaresetoccupy(board, piece, col, row);
        }

        void piece_get_move_king(int board,int piece)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
            bool nevermoved = pieces[pointerP + 5]==1;
            //get check
            int check_numpieces = check_getcheckfeature(board, side, 0);
            //
            bool white_o_o = board_getboardfeature(board, 1)==1;
            bool white_o_o_o = board_getboardfeature(board, 2)==1;
            bool black_o_o = board_getboardfeature(board, 3)==1;
            bool black_o_o_o = board_getboardfeature(board, 4)==1;
            //
            int pointer = board_getboardfeature(board, 5);

            for (int i = -1; i < 2; i++)
            {
                for (int i2 = -1; i2 < 2; i2++)
                {
                    var xv = x + i;
                    var yv = y + i2;
                    if (xv < 0 || yv < 0 || yv > 7 || xv > 7)
                    {
                        continue;
                    }
                    //  move mv = new move();
                    // mv.part1 = this;
                    // int[] n = { xv, yv };
                    // mv.position1 = n;
                    //   mv.position1_0 = xv;
                    //  mv.position1_1 = yv;
                    
                    int part = square_getpart(board, xv, yv);
                    bool ocupiedwhite = square_litefeature_extractor(part, 2)==1;
                    bool checkedblack = square_litefeature_extractor(part, 1)==1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3)==1;
                    bool checkedwhite = square_litefeature_extractor(part, 0)==1;
                 //   int occupyingpiece = square_litefeature_extractor(part, 4);
                    bool fok = true;
                    if (check_numpieces != 0)
                    {
                        // if (!))
                        {
                           // fok = mv.match_bishop() && mv.match_rook();
                        }
                    }
                    if ((side == 0 && fok && (!ocupiedwhite && !checkedblack)) || (side == 1 && fok && !ocupiedblack && !checkedwhite))
                    {
                      //  if (ocupiedblack||ocupiedwhite)
                        {
                            //mv.part2 = occupyingpiece;
                            move_createmove(board, pointer, -1, -1, -1, xv, yv, -1, -1,piece,-1, true,false, ref pointer,side,5,x,y);

                        }
                       // pushAndCalcForking(moves, mv);
                    }
                }

            }
            if (check_numpieces != 0 || !nevermoved)
            {
                return ;
            }
            
            //if (side == 0)
            {
                if ((side == 0&&white_o_o)|| (side == 1 && black_o_o))
                {
                    bool cs = true;
                    for (int i = 1; i < 3; i++)
                    {
                        int tx = x + i;
                        int part = square_getpart(board, tx, y);
                        bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                        bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                        bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                        bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
                        if (tx >6 || (side==0&& checkedblack)||(side==1 &&checkedwhite) || ocupiedblack||ocupiedwhite )
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x + 3;
                    int exists = square_getsquare_feature(board, tx2, y, 4);
                    if (exists!=-1) {
                        bool rooktype = piece_getpiece_feature(board, exists, 0) == 1;
                        bool sameside = piece_getpiece_feature(board, exists, 4) == side;
                        bool nevermovedrook = piece_getpiece_feature(board, exists, 5) == 1;
                        if (cs && rooktype && nevermovedrook && sameside)
                        {
                            move_createmove(board, pointer, 0+side*2, -1, -1, x + 2, y, tx2 - 2, y,piece, exists, true, false, ref pointer,side,5,x,y);
                            // pushAndCalcForking(moves, m);
                        } }
                }
                if ((side == 0 && white_o_o_o) || (side == 1 && black_o_o_o))
                {
                    bool cs = true;
                    for (int i = 1; i < 4; i++)
                    {
                        int tx = x - i;
                        int part = square_getpart(board, tx, y);
                        bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                        bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                        bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                        bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
                        if (tx <1 || (side == 0 && checkedblack) || (side == 1 && checkedwhite) || ocupiedblack || ocupiedwhite)
                        {
                            cs = false;
                            break;
                        }
                    }
                    int tx2 = x - 4;
                    int exists = square_getsquare_feature(board, tx2, y, 4);
                    if (exists != -1)
                    {
                        bool rooktype = piece_getpiece_feature(board, exists, 0) == 1;
                        bool nevermovedrook = piece_getpiece_feature(board, exists, 5) == 1;
                        bool sameside = piece_getpiece_feature(board, exists, 4) == side;
                        if (cs && rooktype && nevermovedrook && sameside)
                        {
                            
                            move_createmove(board, pointer, 1 + side * 2, -1, -1, x - 2, y, tx2+3, y,piece, exists, true, false, ref pointer,side,5,x,y);
                            //  pushAndCalcForking(moves, m);
                        }
                    }
                }
            }
          
        }
        void piece_getmove_knight(int board, int piece)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
          //  bool nevermoved = pieces[pointerP + 5] == 1;
            //get check
          //  int check_numpieces = check_getcheckfeature(board, side, 0);
            //
            //
            int pointer = board_getboardfeature(board, 5);
            ////

            var rawalow = piece_rowcheck(board, side, x, y);
            var collow = piece_colcheck(board, side, x, y); // !colcheck();
            var diaglow_R = piece_diagonalcheckright(board, side, x, y);// !diagonal_right_check();
            var diaglow_L = piece_diagonalcheckleft(board, side, x, y);// !diagonal_left_check();
            for (int i = 1; i < 9; i++)
            {
                int adx = -1;
                int ady = -1;
                switch (i)
                {
                    case 1:
                        adx = -1;
                        ady = 2;
                        break;
                    case 2:
                        adx = -1;
                        ady = -2;
                        break;
                    case 3:
                        adx = 1;
                        ady = -2;
                        break;
                    case 4:
                        adx = 1;
                        ady = 2;
                        break;
                    case 5:
                        ady = 1;
                        adx = 2;
                        break;
                    case 6:
                        ady = 1;
                        adx = -2;
                        break;
                    case 7:
                        ady = -1;
                        adx = 2;
                        break;
                    case 8:
                        ady = -1;
                        adx = -2;
                        break;
                }

                //  int[] n = { x + adx, y + ady };
                int ax = x + adx;
                int ay = y + ady;
                if (ax > 7 || ax < 0 || ay > 7 || ay < 0)
                {
                    continue;
                }
                int part = square_getpart(board, ax, ay);
                bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
              //  bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
              //  bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
               // int occupyingpiece = square_litefeature_extractor(part, 4);
               // move mv = new move();
             //   mv.part1 = this;

                //  mv.position1 = n;
             //   mv.position1_0 = ax;
             //   mv.position1_1 = ay;
                switch (side)
                {
                    case 0:
                        if (!ocupiedwhite && rawalow && collow && diaglow_L && diaglow_R)
                        {
                          
                            move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece,-1, false, false, ref pointer,side,2,x,y);
                            //  moves.Push(mv);
                            //pushifAccept(moves, mv);
                        }
                        break;
                    case 1:
                        if (!ocupiedblack && rawalow && collow && diaglow_L && diaglow_R)
                        {
                            move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece,-1, false, false, ref pointer,side,2,x,y);
                            // pushifAccept(moves, mv);
                        }
                        break;
                }

            }
        }
        void piece_getmove_rook(int board, int piece,int queentypeorrook)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
          //  bool nevermoved = pieces[pointerP + 5] == 1;
            //get check
         //   int check_numpieces = check_getcheckfeature(board, side, 0);
            //
            //
            int pointer = board_getboardfeature(board, 5);
            ////

            var rawalow = piece_rowcheck(board, side, x, y);
            var collow = piece_colcheck(board, side, x, y); // !colcheck();
            var diaglow_R = piece_diagonalcheckright(board, side, x, y);// !diagonal_right_check();
            var diaglow_L = piece_diagonalcheckleft(board, side, x, y);// !diagonal_left_check();
            bool allow = collow && diaglow_L && diaglow_R;
            int aa = 1;
            int bb = 0;
            //
            int a1 = -1;
            int a2 = 8;
            int b1 = -1;
            int b2 = 9;
            int ttx = x + 1;
            int tty = y;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = 0;
                        //
                        ttx = x - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        break;
                    case 2:
                        aa = 0;
                        bb = 1;
                        //
                        ttx = x;
                        tty = y + 1;
                        //
                        a1 = -1;
                        a2 = 9;
                        b1 = -1;
                        b2 = 8;
                        allow = rawalow && diaglow_L && diaglow_R;
                        break;
                    case 3:
                        aa = 0;
                        bb = -1;
                        //
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 9;
                        b1 = -1;
                        b2 = 8;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    if (!allow)
                    {
                        break;
                    }
                    int part = square_getpart(board, i, i2);
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                 //   bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                 //   bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
                 //   int occupyingpiece = square_litefeature_extractor(part, 4);
                   
                    if (ocupiedblack||ocupiedwhite)
                    {
                        //mv.part2 = nb.squares[i, i2].pin;
                        if (side == 0 && ocupiedblack)
                        {

                            move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                        }
                        else if (side == 1 && ocupiedwhite)
                        {
                            //nb.squares[i, i2].activatecheck(this);
                            move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                        }
                        break;
                    }
                    move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                    // pushIfacceptdv(mv);
                }
            }
        }
        void piece_getmove_bishop(int board,int piece,int queentypeorrook)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
         //   bool nevermoved = pieces[pointerP + 5] == 1;
            //get check
         //   int check_numpieces = check_getcheckfeature(board, side, 0);
            //
            //
            int pointer = board_getboardfeature(board, 5);
            ////

            var rawalow = piece_rowcheck(board, side, x, y);
            var collow = piece_colcheck(board, side, x, y); // !colcheck();
            var diaglow_L = piece_diagonalcheckright(board, side, x, y);// !diagonal_right_check();
            var diaglow_R = piece_diagonalcheckleft(board, side, x, y);// !diagonal_left_check();
            int kingrow = check_getcheckfeature(board, side, 6);
            int kingcol = check_getcheckfeature(board, side, 5);
            if (kingrow - y > 0)
            {
                var r = diaglow_R;
                diaglow_R = diaglow_L;
                diaglow_L = r;
            }
            bool allow = diaglow_L && rawalow && collow;
            int aa = 1;
            int bb = 1;
            //
            int a1 = -1;
            int a2 = 8;
            int b1 = -1;
            int b2 = 8;
            int ttx = x + 1;
            int tty = y+1;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = -1;
                        //
                        ttx = x - 1;
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        allow = diaglow_L && rawalow && collow;
                        break;
                    case 2:
                        aa = -1;
                        bb = 1;
                        //
                        ttx = x - 1;
                        tty = y + 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        allow = diaglow_R && rawalow && collow;
                        break;
                    case 3:
                        aa = 1;
                        bb = -1;
                        //
                        ttx = x + 1;
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        allow = diaglow_R && rawalow && collow;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                    if (!allow)
                    {
                        break;
                    }
                    int part = square_getpart(board, i, i2);
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                 //   bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                 //   bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
                //    int occupyingpiece = square_litefeature_extractor(part, 4);

                    if (ocupiedblack || ocupiedwhite)
                    {
                        //mv.part2 = nb.squares[i, i2].pin;
                        if (side == 0 && ocupiedblack)
                        {

                            move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                        }
                        else if (side == 1 && ocupiedwhite)
                        {
                            //nb.squares[i, i2].activatecheck(this);
                            move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                        }
                        break;
                    }
                    move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece,-1, false, false, ref pointer,side, queentypeorrook,x,y);
                    // pushIfacceptdv(mv);
                }
            }
        }
        //final one pawn move
        void piece_getmove_pawn(int board,int piece)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
            bool nevermoved = pieces[pointerP + 5] == 1;
            //get check
            int check_numpieces = check_getcheckfeature(board, side, 0);
            //
            //
            int pointer = board_getboardfeature(board, 5);
            ////

            var rawalow = piece_rowcheck(board, side, x, y);
            var collow = piece_colcheck(board, side, x, y); // !colcheck();
            var diaglow_L = piece_diagonalcheckright(board, side, x, y) ;// !diagonal_right_check();
            var diaglow_R = piece_diagonalcheckleft(board, side, x, y);// !diagonal_left_check();
            bool con = true;
            int ay = 0;
            int kingrow = check_getcheckfeature(board, side, 6);
            int kingcol = check_getcheckfeature(board, side, 5);
            if (kingrow - y > 0)
            {
                var r = diaglow_R;
                diaglow_R = diaglow_L;
                diaglow_L = r;
            }
            if (nevermoved)
            {
                // move m = new move();
                // m.part1 = this;
                
                if (side == 0)
                {
                    //int[] mt = { x, y + 2 };
                    //m.position1 = mt;
                    int part = square_getpart(board, x, y+2);
                    ay = y + 2;
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    int part2 = square_getpart(board, x, y + 1);
                    bool ocupiedwhite2 = square_litefeature_extractor(part2, 2) == 1;
                    bool ocupiedblack2 = square_litefeature_extractor(part2, 3) == 1;
                    if (ocupiedwhite|| ocupiedblack || ocupiedwhite2 || ocupiedblack2)
                    {
                        con = false;
                    }
                }
                else
                {
                    int part = square_getpart(board, x, y - 2);
                    ay = y - 2;
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    int part2 = square_getpart(board, x, y - 1);
                    bool ocupiedwhite2 = square_litefeature_extractor(part2, 2) == 1;
                    bool ocupiedblack2 = square_litefeature_extractor(part2, 3) == 1;
                    if (ocupiedwhite || ocupiedblack || ocupiedwhite2 || ocupiedblack2)
                    {
                        con = false;
                    }
                }
                //I need to check if it's safe // change in row number requires rowcheck ##
                if (con && rawalow && diaglow_L && diaglow_R )
                {
                    // m.longmove = true;
                    // moves.Push(m);
                    move_createmove(board, pointer, -1, 1, -1, x, ay, -1, -1, piece,-1, false, true, ref pointer,side,0,x,y);
                }
            }
           // move mm = new move();
            if (side == 0)
            {
                ay = y + 1;
            }
            else
            {
                ay = y - 1;
            }
            if (true)
            {
                int part = square_getpart(board, x,ay);
                bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                //I need to check if it's safe // change in row number requires rowcheck ##
                if (rawalow && diaglow_L && diaglow_R && !(ocupiedwhite||ocupiedblack))
                {
                    move_createmove(board, pointer, -1, -1, -1, x, ay, -1, -1, piece,-1, false, true, ref pointer, side,0,x,y);
                }
            }
           // takes
            bool addme = false;
            int ax = 0;
            if (side == 0)
            {
                // int[] mt = { x + 1, y + 1 };
                // mf.position1 = mt;
                ax = x + 1;
                ay = y + 1;
                if ((x < 7 && y < 7))
                {
                    int part = square_getpart(board, ax, ay);
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    if (ocupiedblack)
                    {
                        addme = true;
                    }
                }

            }
            else
            {
                //  int[] mt = { x - 1, y - 1 };
                //mf.position1 = mt;
                // mf.position1_0 = x - 1;
                //  mf.position1_1 = y - 1;
                ax = x - 1;
                ay = y - 1;
                if ((x > 0 && y > 0))
                {
                    int part = square_getpart(board, ax, ay);
                    bool occupiedwhite = square_litefeature_extractor(part, 2) == 1;
                    if (occupiedwhite)
                    {
                        addme = true;
                    }
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && collow && addme)
            {
                move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece,-1, false, true, ref pointer, side,0,x,y);
            }
            addme = false;
            if (side == 0&& x > 0 && y < 7)
            {
                //   int[] mt = { x - 1, y + 1 };
                //  mff.position1 = mt;
                ax = x - 1;
                ay = y + 1;
                int part = square_getpart(board, ax, ay);
                bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                if (( ocupiedblack))
                {
                    addme = true;

                   // mff.part2 = nb.squares[x - 1, y + 1].pin;
                }
            }
            else if(side==1 && x < 7 && y > 0)
            {
                //int[] mt = { x + 1, y - 1 };
                //mff.position1 = mt;
                ax = x + 1;
                ay = y - 1;
                int part = square_getpart(board, ax, ay);
                bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                if (( ocupiedwhite))
                {
                    addme = true;

                  //  mff.part2 = nb.squares[x + 1, y - 1].pin;
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_R && collow && addme)
            {
                move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece,-1, false, true, ref pointer, side,0,x,y);
            }
            //on pasent
           // move mffk = new move();
           // mffk.part1 = this;
            addme = false;
            int part_2 = -1;
            if (side == 0 && x > 0 && y < 7 )
            {
                //   int[] mt = { x - 1, y + 1 };
                // mffk.position1 = mt;
                ax= x - 1;
                ay= y + 1;
                
               // if (ocupiedblack)
                {
                    int part = square_getpart(board, x-1, y);
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    bool nopiece = square_getsquare_feature(board, ax, ay,4) ==-1;
                    if (ocupiedblack && nopiece)
                    {
                        int pp = square_litefeature_extractor(part, 4);
                        bool pawntype = piece_getpiece_feature(board, pp, 0) == 0;
                        bool longmovefirst = piece_getpiece_feature(board, pp, 6) == 1;
                        part_2 = pp;
                        if ((pawntype && longmovefirst) )
                        {
                            addme = true;
                            //mffk.part2 = nb.squares[x - 1, y].pin;
                        }
                    }
                }
            }
            else if(side==1&& x < 7 && y > 0 )
            {
                //  int[] mt = { x + 1, y - 1 };
                //mffk.position1 = mt;
                ax = x + 1;
                ay = y - 1;
                int part = square_getpart(board, x + 1, y);
                bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                bool nopiece = square_getsquare_feature(board, ax, ay, 4) == -1;
                if (ocupiedwhite && nopiece){
                    int pp = square_litefeature_extractor(part, 4);
                    bool pawntype = piece_getpiece_feature(board, pp, 0) == 0;
                    bool longmovefirst = piece_getpiece_feature(board, pp, 6) == 1;
                    part_2 = pp;
                    if (  (pawntype && longmovefirst))
                    {
                        addme = true;
                       // mffk.part2 = nb.squares[x + 1, y].pin;
                    }
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_R && collow && addme)
            {
                move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece, part_2, false, true, ref pointer, side,0,x,y);
            }
            //on pasent
           // move mffl = new move();
           // mffl.part1 = this;
            addme = false;
            if (side == 0&& x < 7 && y < 7)
            {
                //  int[] mt = { x + 1, y + 1 };
                //mffl.position1 = mt;
                ax = x + 1;
                ay = y + 1;
                int part = square_getpart(board, x + 1, y);
                bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                bool nopiece = square_getsquare_feature(board, ax, ay, 4) == -1;
                if (ocupiedblack && nopiece)
                {
                    int pp = square_litefeature_extractor(part, 4);
                    bool pawntype = piece_getpiece_feature(board, pp, 0) == 0;
                    bool longmovefirst = piece_getpiece_feature(board, pp, 6) == 1;
                    part_2 = pp;
                    if (  (pawntype && longmovefirst))
                    {
                        addme = true;
                        //mffk.part2 = nb.squares[x - 1, y].pin;
                    }
                }
            }
            else if(side==1&& x > 0 && y > 0)
            {
                //int[] mt = { x - 1, y - 1 };
                //mffl.position1 = mt;
                ax= x - 1;
                ay = y - 1;
                int part = square_getpart(board, x - 1, y);
                bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                bool nopiece = square_getsquare_feature(board, ax, ay, 4) == -1;
                if (ocupiedwhite&& nopiece)
                {
                    int pp = square_litefeature_extractor(part, 4);
                    bool pawntype = piece_getpiece_feature(board, pp, 0) == 0;
                    bool longmovefirst = piece_getpiece_feature(board, pp, 6) == 1;
                    part_2 = pp;
                    if (  (pawntype && longmovefirst))
                    {
                        addme = true;
                        //mffk.part2 = nb.squares[x - 1, y].pin;
                    }
                }
            }
            //I need to check if it's safe // change in row number requires rowcheck ##
            if (rawalow && diaglow_L && collow && addme)
            {
                move_createmove(board, pointer, -1, -1, -1, ax, ay, -1, -1, piece, part_2, false, true, ref pointer, side,0,x,y);
            }
        }
        void piece_getmove(int board,int piece)
        {
            var type = piece_getpiece_feature(board, piece, 0);
            switch (type)
            {
                case 0:
                    piece_getmove_pawn(board,piece);
                    break;
                case 1:
                    piece_getmove_rook(board, piece,1);
                    break;
                case 2:
                    piece_getmove_knight(board, piece);
                    break;
                case 3:
                    piece_getmove_bishop(board, piece,3);
                    break;
                case 4:
                    //queen
                    piece_getmove_rook(board, piece,4);
                    piece_getmove_bishop(board, piece,4);
                    break;
                case 5:
                    piece_get_move_king(board,piece);
                    break;
            }
        }
        int piece_getprefarredscore(int board,int piece,int col,int row,bool noqueen)
        {
            int type = piece_getpiece_feature(board, piece, 0);
            int prefscore = 0;
            int side = piece_getpiece_feature(board, piece, 4);
            int nevermoved=piece_getpiece_feature(board,piece,5);
            if (nevermoved == 1)
            {
                prefscore -= 3;
            }
            else
            {
                prefscore += 3;
            }
            switch (piece_getpiece_feature(board, piece, 0))
            {
                case 0://pawn
                    if(col<5&& col > 3 && row < 5 && row > 3)
                    {
                        prefscore += 10;
                    }
                    else
                    {
                        prefscore -= 10;
                    }
                    return prefscore;// +piece_getpiece_feature(board,piece,1);
                case 1://rook
                    if (row == (6 * (1-side)+side))
                    {
                        prefscore += 10;
                    }
                    else
                    {
                        prefscore -= 10;
                    }
                    return prefscore;
                case 2://knight
                    if (col == 2 || col == 5)
                    {
                        prefscore += 10;
                    }
                    else if(col<6&& col> 2 && row<6&& row>2)
                    {
                        prefscore += 20;
                    }
                    else
                    {
                        prefscore -= 10;
                    }
                    return prefscore;
                case 3://bishop
                    if ((col == 1||col==6)&& (row==1||row==6))
                    {
                        prefscore += 10;
                    }
                    else
                    {
                        prefscore -= 10;
                    }
                    return prefscore;
                case 4://queen
                    if(row!=7&& row != 0)
                    {
                        prefscore -= 10;
                    }
                    else
                    {
                        prefscore += 10;
                    }
                    return prefscore;
                case 5://king
                    if (col >= 6 || col <= 2)
                    {
                        if (row == 7||row==0)
                        {
                            prefscore += 15;
                            if (col == 7||col==1||col==0)
                            {
                                prefscore += 20;
                            }
                        }
                        else
                        {
                            prefscore -= 10;
                        }
                    }
                    if (noqueen)
                    {
                        if (col > 2 && col < 6 && row > 2 && row < 6)
                        {
                            if (col > 3 && row > 3 && col < 5 && row < 5)
                            {
                                prefscore += 30;
                            }
                            prefscore += 10;
                        }
                        else
                        {
                            prefscore -= 30;
                        }
                    }
                    return prefscore;
            }
            return prefscore;
        }
        void piece_getpawnchecks(int board,int piece)
        {
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
            switch (side)
            {
                case 0:
                    if (y < 7)
                    {
                        if (x < 7)
                        {
                            square_setsquare_feature(board, x + 1, y + 1, 0+side, 1);
                            checks_checkifitisking(board, x + 1, y + 1, x, y, piece,  0, side);
                            square_addcontroller(board, x + 1, y + 1, side, 10);

                        }
                        if (x > 0)
                        {
                            square_setsquare_feature(board, x - 1, y + 1, 0 + side, 1);
                            checks_checkifitisking(board, x - 1, y + 1, x, y, piece, 0, side);
                            square_addcontroller(board, x - 1, y + 1, side, 10);
                        }
                    }
                    break;
                case 1:
                    if (y > 0)
                    {
                        if (x < 7)
                        {
                            square_setsquare_feature(board, x + 1, y - 1, 0 + side, 1);
                            checks_checkifitisking(board, x + 1, y - 1, x, y, piece, 0, side);
                            square_addcontroller(board, x + 1, y - 1, side, 10);
                        }
                        if (x > 0)
                        {
                            square_setsquare_feature(board, x - 1, y - 1, 0 + side, 1);
                            checks_checkifitisking(board, x - 1, y - 1, x, y, piece, 0, side);
                            square_addcontroller(board, x - 1, y - 1, side, 10);
                        }
                    }
                    break;
            }
        }
        void piece_getknightchecks(int board,int piece)
        {
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int side = pieces[pointerP + 4];
            for (int i = 1; i < 9; i++)
            {
                int adx = -1;
                int ady = -1;
                switch (i)
                {
                    case 1:
                        adx = -1;
                        ady = 2;
                        break;
                    case 2:
                        adx = -1;
                        ady = -2;
                        break;
                    case 3:
                        adx = 1;
                        ady = -2;
                        break;
                    case 4:
                        adx = 1;
                        ady = 2;
                        break;
                    case 5:
                        ady = 1;
                        adx = 2;
                        break;
                    case 6:
                        ady = 1;
                        adx = -2;
                        break;
                    case 7:
                        ady = -1;
                        adx = 2;
                        break;
                    case 8:
                        ady = -1;
                        adx = -2;
                        break;
                }

                // int[] n = { x + adx, y + ady };
                int n_0 = x + adx;
                int n_1 = y + ady;
                if (n_0 > 7 || n_0 < 0 || n_1 > 7 || n_1 < 0)
                {
                    continue;
                }
                switch (side)
                {
                    case 0:
                        square_setsquare_feature(board, n_0, n_1, 0, 1);
                        checks_checkifitisking(board, n_0, n_1, x, y, piece, 0, side);
                        //    nb.squares[n_0, n_1].n_white++;
                        //     nb.squares[n_0, n_1].value_white += 30;
                        break;
                    case 1:
                        square_setsquare_feature(board, n_0, n_1, 1, 1);
                        checks_checkifitisking(board, n_0, n_1, x, y, piece, 0, side);
                        break;
                }
                square_addcontroller(board, n_0, n_1, side, 30);
            }
        }
        void piece_getrookchecks(int board,int piece,bool notqueen)
        {
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int valm = 90;
            if (notqueen)
            {
                valm = 50;
            }
            if (rook1col == -1&& notqueen)
            {
                rook1col = x;
                rook1row = y;
            }
            else if (notqueen)
            {
                rook2col = x;
                rook2row = y;
            }
            int side = pieces[pointerP + 4];
            int aa = 1;
            int bb = 0;
            //
            int a1 = -1;
            int a2 = 8;
            int b1 = -1;
            int b2 = 9;
            int ttx = x + 1;
            int tty = y;
            int feature2=5;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = 0;
                        //
                        ttx = x - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 6;
                        break;
                    case 2:
                        aa = 0;
                        bb = 1;
                        //
                        ttx = x;
                        tty = y + 1;
                        //
                        a1 = -1;
                        a2 = 9;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 9;
                        break;
                    case 3:
                        aa = 0;
                        bb = -1;
                        //
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 9;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 10;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                  //  int p = 0;
                    int part = square_getpart(board, i, i2);
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                   // bool checkedblack = square_litefeature_extractor(part, 1) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    //   bool checkedwhite = square_litefeature_extractor(part, 0) == 1;
                    //   int occupyingpiece = square_litefeature_extractor(part, 4);
                 //   p = square_getsquare_feature(board, x, y, 4);
                    square_litefeature_assign(part, side, 1);
                 //    p = square_getsquare_feature(board, x, y, 4);
                    square_litefeature_assign(part,feature2+ side*2, 1);
                    square_addcontroller(board, i, i2, side, valm);
                    //  p = square_getsquare_feature(board, x, y, 4);
                    if (ocupiedblack || ocupiedwhite)
                    {
                        var occup = square_litefeature_extractor(part, 4);
                        var pieceking = piece_getpiece_feature(board, occup, 0)==5;
                        var sidek = piece_getpiece_feature(board, occup, 4);
                        if ((pieceking) && sidek != side)
                        {
                            checks_checkifitisking(board, i, i2, x, y, piece, 1, side);
                        }
                        else
                        {
                            break;
                        }
                    }
                  //  move_createmove(board, pointer, -1, -1, -1, i, i2, -1, -1, piece, -1, false, false, ref pointer);
                    // pushIfacceptdv(mv);
                }
            }
        }
        void piece_getbishopchecks(int board,int piece,bool notqueen)
        {
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            int valm = 90;
            if (notqueen)
            {
                valm = 32;
            }
            int side = pieces[pointerP + 4];
            int aa = 1;
            int bb = 1;
            //
            int a1 = -1;
            int a2 = 8;
            int b1 = -1;
            int b2 = 8;
            int ttx = x + 1;
            int tty = y + 1;
            int feature2 = 13;
            for (int ix = 0; ix < 4; ix++)
            {
                switch (ix)
                {
                    case 1:
                        aa = -1;
                        bb = -1;
                        //
                        ttx = x - 1;
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 18;
                        break;
                    case 2:
                        aa = -1;
                        bb = 1;
                        //
                        ttx = x - 1;
                        tty = y + 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 17;
                        break;
                    case 3:
                        aa = 1;
                        bb = -1;
                        //
                        ttx = x + 1;
                        tty = y - 1;
                        //
                        a1 = -1;
                        a2 = 8;
                        b1 = -1;
                        b2 = 8;
                        feature2 = 14;
                        break;
                }
                for (int i = ttx, i2 = tty; i > a1 && i < a2 && i2 < b2 && i2 > b1; i += aa, i2 += bb)
                {
                   
                    int part = square_getpart(board, i, i2);
                    bool ocupiedwhite = square_litefeature_extractor(part, 2) == 1;
                    bool ocupiedblack = square_litefeature_extractor(part, 3) == 1;
                    square_litefeature_assign(part, side, 1);
                    square_litefeature_assign(part, feature2 + side * 2, 1);
                    square_addcontroller(board, i, i2, side, valm);
                    if (ocupiedblack || ocupiedwhite)
                    {
                        var occup = square_litefeature_extractor(part, 4);
                        var pieceking = piece_getpiece_feature(board, occup, 0) == 5;
                        var sidek = piece_getpiece_feature(board, occup, 4);
                        if ((pieceking) && sidek != side)
                        {
                            checks_checkifitisking(board, i, i2, x, y, piece, 2, side);
                        }
                        else
                        {
                            break;
                        }
                    }
                    // pushIfacceptdv(mv);
                }
            }
        }
        void piece_getkingchecks(int board,int piece)
        {
            //first extractfeatures board and piece
            //piece features
            int pointerP = board * piecesperboard * totalPiecesFeatures + piece * totalPiecesFeatures;
            int x = pieces[pointerP + 1];  //col
            int y = pieces[pointerP + 2];  //row
            kingcolp = x;
            kingrowp = y;
            int side = pieces[pointerP + 4];
            for (int i = -1; i < 2; i++)
            {
                for (int i2 = -1; i2 < 2; i2++)
                {
                    var xv = x + i;
                    var yv = y + i2;
                    if (xv < 0 || yv < 0 || yv > 7 || xv > 7)
                    {
                        continue;
                    }
                    square_setsquare_feature(board, xv, yv, side, 1);
                    square_addcontroller(board, xv, yv, side, 200);
                }

            }
        }
        //checks
        bool piece_rowcheck(int board,int side,int col,int row)
        {
            int kingrow = check_getcheckfeature(board, side, 6);

            if (kingrow != row)
            {
                return true;//can never check from a row
            }
            int kingcol = check_getcheckfeature(board, side, 5);
            side = 1 - side;
            if ( kingcol < col && col<7)
            {
                var part = square_getpart(board, col+1, row );
                var rowleft = square_litefeature_extractor(part, 6 + side * 2)==1;
                var piece = square_litefeature_extractor(part, 4);
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if (( typep == 1 || typep == 4)&& side2 == side)
                    {
                        rowleft = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (rowleft)
                {
                    for (int i = col-1, i2 = row; i > kingcol; i--)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else if ( kingcol > col && col>0)
            {
                var part = square_getpart(board, col-1, row );
                var rowright = square_litefeature_extractor(part, 5 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if (  (typep == 1 || typep == 4) && side2 == side)
                    {
                        rowright = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (rowright)
                {
                    for (int i = col+1, i2 = row; i < kingcol; i++)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return true;
        }
        bool piece_colcheck(int board,int side,int col,int row)
        {
            int kingcol = check_getcheckfeature(board, side, 5);
            if (kingcol != col)
            {
                return true;//can never check from a row
            }
            int kingrow = check_getcheckfeature(board, side, 6);
            side = 1 - side;
            if ( kingrow < row && row<7)
            {
             //   var coldown = square_getsquare_feature(board, col+1, row, 10 + side * 2)==1;
                var part = square_getpart(board, col , row+1);
                var coldown = square_litefeature_extractor(part, 10 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if ( (typep == 1 || typep == 4) && side2 == side)
                    {
                        coldown = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (coldown)
                {
                    for (int i = col, i2 = row-1; i2 > kingrow; i2--)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else if (  kingrow > row && row>0)
            {
                var part = square_getpart(board, col, row-1);
                var colup = square_litefeature_extractor(part, 9 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if ( (typep == 1 || typep == 4)&& side2==side)
                    {
                        colup = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (colup)
                {
                    for (int i = col, i2 = row + 1; i2 < kingrow; i2++)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            return true;
        }
        bool piece_diagonalcheckleft(int board, int side, int col, int row)
        {
            int kingrow = check_getcheckfeature(board, side, 6);
            int kingcol = check_getcheckfeature(board, side, 5);
            side = 1 - side;
            if (kingcol - col == kingrow - row&&kingcol<col&&col<7&&row<7)
            {
                var part = square_getpart(board, col + 1, row+1);
                var diagleftup = square_litefeature_extractor(part, 18 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if ( (typep == 3 || typep == 4)&& side2 == side)
                    {
                        diagleftup = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (diagleftup)
                {
                    for (int i = col-1, i2 = row-1; i > kingcol&&i2>kingrow; i--,i2--)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else if(kingcol - col == row-kingrow && kingcol < col&&col<7&&row>0)
            {
                var part = square_getpart(board, col + 1, row - 1);
                var diagleftdown = square_litefeature_extractor(part, 17 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if ((typep == 3 || typep == 4)&& side2 == side)
                    {
                        diagleftdown = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (diagleftdown)
                {
                    for (int i = col-1, i2 = row+1; i > kingcol && i2 < kingrow; i--, i2++)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
                return true;
        }
        bool piece_diagonalcheckright(int board, int side, int col, int row)
        {
            int kingrow = check_getcheckfeature(board, side, 6);
            int kingcol = check_getcheckfeature(board, side, 5);
            side = 1 - side;
            if (kingcol - col == kingrow - row && kingcol > col &&col>0&&row>0)
            {
                var part = square_getpart(board, col - 1, row - 1);
                var diagrightup = square_litefeature_extractor(part, 13 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
               
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if ((typep == 3 || typep == 4)&& side2 == side)
                    {
                        diagrightup = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (diagrightup)
                {
                    for (int i = col+1, i2 = row+1; i < kingcol && i2 < kingrow; i++, i2++)
                    {
                        if (square_getsquare_feature(board, i, i2, 4) != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
            else if(kingcol - col == row-kingrow && kingcol > col && col>0 &&row<7)
            {
                var part = square_getpart(board, col - 1, row + 1);
                var diagrightdown = square_litefeature_extractor(part, 14 + side * 2) == 1;
                var piece = square_litefeature_extractor(part, 4);
                if (piece != -1)
                {
                    var typep = piece_getpiece_feature(board, piece, 0);
                    var side2 = piece_getpiece_feature(board, piece, 4);
                    if  ((typep == 3 || typep == 4)&& side2 == side)
                    {
                        diagrightdown = true;
                    }
                    else
                    {
                        return true;
                    }
                }
                if (diagrightdown)
                {
                    for (int i = col+1, i2 = row-1; i < kingcol && i2 > kingrow; i++, i2--)
                    {
                        var x = square_getsquare_feature(board, i, i2, 4);
                        if (x != -1)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }
                return true;
        }
    }
}
