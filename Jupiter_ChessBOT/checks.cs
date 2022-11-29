using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    partial class jupiterChess
    {
        //now check
        const  int totalcheckfeatures = 9;
        int[] checks;
        public void setchecks()
        {
            //assign pieces
            checks = new int[totalnumberassign * 2 * totalcheckfeatures];
            //0=numpieces,1=positionofcheck_0,2=positionofcheck_1,3=checkpiece,4=typecheck,5=kingposition_0,6=kingposition_1,7=xmatch,8=rookmatch
        }
        /// <summary>
        /// side =0 if white king is checked and 1 if blackking
        /// </summary> 
        public void check_createcheck(int board,int side,int positionofcheck_0,int positionofcheck_1,int checkpiece,int typecheck,int kingpoistion_0, int kingpoistion_1)
        {
            int pointer = board * 2 * totalcheckfeatures+side*totalcheckfeatures;
            checks[pointer] = 1;
            checks[pointer+1] = positionofcheck_0;
            checks[pointer+2] = positionofcheck_1;
            checks[pointer+3] = checkpiece;
            checks[pointer+4] = typecheck;
            checks[pointer+5] = kingpoistion_0;
            checks[pointer+6] = kingpoistion_1;
        }
        public  void check_resetme(int board)
        {
            for (int side = 0; side < 2; side++)
            {
                int pointer = board * 2 * totalcheckfeatures + side * totalcheckfeatures;
                checks[pointer] = 0;
                checks[pointer + 1] = 0;
                checks[pointer + 2] = 0;
                checks[pointer + 3] = 0;
                checks[pointer + 4] = 0;
                checks[pointer + 5] = 0;
                checks[pointer + 6] = 0;
                checks[pointer + 7] = 0;
                checks[pointer + 8] = 0;
            }
        }
        /// <summary>
        /// typecheck 0 =piece or knight,2=bishop,1=rook
        /// </summary>
        public void checks_checkifitisking(int board,int col,int row,int positionofcheck_0,int positionofcheck_1,int checkpiece,int typecheck, int side)
        {
            var piece = square_getsquare_feature(board, col, row, 4);
            if (piece == -1)
            {
                return;
            }
            var king = piece_getpiece_feature(board, piece, 0)==5;
            if (!king)
            {
                return;
            }
            var sidedontmatch = piece_getpiece_feature(board, piece, 4)!=side;
           if(king && sidedontmatch)
            {
                int pointer = board * 2 * totalcheckfeatures + (1-side) * totalcheckfeatures;
                checks[pointer]++;
                checks[pointer + 1] = positionofcheck_0;
                checks[pointer + 2] = positionofcheck_1;
                checks[pointer + 3] = checkpiece;
                checks[pointer + 4] = typecheck;
                checks[pointer + 5] = col;//kingposition_0
                checks[pointer + 6] = row;
                if (typecheck == 1)
                {
                    checks[pointer + 8] = 1;
                    if (positionofcheck_0 == col)
                    {
                        // xmatch = true;
                        checks[pointer + 7] = 1;//xmatch
                    }
                }
            }
        }
        bool check_matchmove(int board,int side,int col,int row)
        {

            int type = check_getcheckfeature(board, side, 4);
            int positionofcheck_0=check_getcheckfeature(board,side,1);
            int positionofcheck_1=check_getcheckfeature(board,side,2);
            if((positionofcheck_0 == col && positionofcheck_1 == row))
            {
                return true;
            }
            if (type == 0)
            {
               // if(positionofcheck_0!=col || positionofcheck_1 != row)
                {
                    return false;
                }
            }
            if (type == 1)
            {
                int kingposition_0 = check_getcheckfeature(board, side, 5);
                int kingposition_1 = check_getcheckfeature(board, side, 6);
                bool xmatch = check_getcheckfeature(board, side, 7)==1;
                return checks_match_rook(col, row, xmatch, kingposition_0, kingposition_1, positionofcheck_0, positionofcheck_1);
            }
            else if (type == 2)
            {
                int kingposition_0 = check_getcheckfeature(board, side, 5);
                int kingposition_1 = check_getcheckfeature(board, side, 6);
                return checks_match_bishop(col, row, kingposition_0, kingposition_1, positionofcheck_0, positionofcheck_1);
            }
            return true;
        }
        public bool checks_match_bishop(int pos_0, int pos_1,int kingposition_0,int kingposition_1,int positionofcheck_0,int positionofcheck_1)
        {
            bool fg, fg2, fg3;
            bool fg1, fg21, fg31;
            int dx = pos_0 - kingposition_0;
            int dy = pos_1 - kingposition_1;
            if (!(dx == dy || dx == -dy))
            {
                return false;
            }
         //   switch (diagnoalright)
            {
            //    case true:
                    fg = pos_0 - kingposition_0 > 0;
                    fg1 = pos_1 - kingposition_1 > 0;
                    fg2 = positionofcheck_0 - kingposition_0 > 0;
                    fg21 = positionofcheck_1 - kingposition_1 > 0;
                    fg3 = positionofcheck_0 - pos_0 > 0;
                    fg31 = positionofcheck_1 - pos_1 > 0;
                    if (fg == fg2 && fg2 == fg3 && fg21 == fg1 && fg31 == fg21)
                    {
                        return true;
                    }
                    return false;
            }
           // return true;
        }
        public bool checks_match_rook(int pos_0, int pos_1,bool xmatch, int kingposition_0, int kingposition_1, int positionofcheck_0, int positionofcheck_1)
        {
            bool fg, fg2, fg3;
            switch (xmatch)
            {
                case true:
                    if (pos_0 != kingposition_0)
                    {
                        return false;
                    }
                    fg = pos_1 - kingposition_1 > 0;
                    fg2 = positionofcheck_1 - kingposition_1 > 0;
                    fg3 = positionofcheck_1 - pos_1 > 0;
                    if (fg == fg2 && fg2 == fg3)
                    {
                        return true;
                    }
                    return false;
                case false:
                    if (pos_1 != kingposition_1)
                    {
                        return false;
                    }
                    fg = pos_0 - kingposition_0 > 0;
                    fg2 = positionofcheck_0 - kingposition_0 > 0;
                    fg3 = positionofcheck_0 - pos_0 > 0;
                    if (fg == fg2 && fg2 == fg3)
                    {
                        return true;
                    }
                    return false;
            }
            return false;
        }
        /// <summary>
        /// 
        ///0=numpieces,1=positionofcheck_0,2=positionofcheck_1,3=checkpiece,4=typecheck,5=kingposition_0,6=kingposition_1,7=xmatch,8=rookmatch      /// </summary>
        /// <param name="board"></param>
        /// <param name="side"></param>
        /// <param name="feature"></param>
        public int check_getcheckfeature(int board,int side,int feature)
        {
            return checks[board * 2 * totalcheckfeatures + side * totalcheckfeatures + feature];
        }
        void check_setking(int board,int side,int kingcol,int kingrow)
        {
            var p = board * totalcheckfeatures *2+side*totalcheckfeatures;
            checks[p + 5] = kingcol;
            checks[p + 6] = kingrow;
        }
    }
}
