using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    partial class jupiterChess
    {
        //now situations
        const  int totalsituationfeatures = 6;
        int[] situations;
        public void setsituations()
        {
            //assign pieces
            situations = new int[totalnumberassign * 1 * totalsituationfeatures];
            //boardindex,parentindex,eval
        }
    }
}
