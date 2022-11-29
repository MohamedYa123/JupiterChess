using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Engine
{
    public class moveJC 
    {
        public int piece;
        public int location1;
        public int location2;
        public int moveindex;
    }

    [Serializable]
    public  class zoobristhasher
    {
        public  long[,,] squares_pawn;
        public  long[,,] squares_Queen;
        public  long[,,] squares_Rook;
        public  long[,,] squares_Bishop;
        public  long[,,] squares_Knight;
        public  long[,,] squares_king;
        public string datapath="data.MYA";
       static long LongRandom(long min, long max, Random rand)
        {
            long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
            result = (result << 32);
            result = result | (long)rand.Next((Int32)min, (Int32)max);
            return result;
        }
        public  void generate()
        {
            squares_pawn = new long[9, 9, 2];
            squares_Queen = new long[9, 9, 2];
            squares_Rook = new long[9, 9, 2];
            squares_Bishop = new long[9, 9, 2];
            squares_Knight = new long[9, 9, 2];
            squares_king = new long[9, 9, 2];
            Random rd = new Random();
            for (int i = 0; i < 9; i++)
            {
                for (int i2 = 0; i2 < 9; i2++)
                {
                    for (int i3 = 0; i3 < 2; i3++)
                    {
                        squares_pawn[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //  Thread.Sleep(1);
                        squares_Queen[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //Thread.Sleep(1);
                        squares_Rook[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //Thread.Sleep(1);
                        squares_Knight[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //Thread.Sleep(1);
                        squares_Bishop[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //Thread.Sleep(1);
                        squares_king[i, i2, i3] = LongRandom(int.MinValue, int.MaxValue, rd);
                        //Thread.Sleep(1);
                    }
                }

            }
        }
        public static zoobristhasher load(string datapath)
        {
            zoobristhasher tc;
            FileStream readerFileStream = new FileStream(datapath,
        FileMode.Open, FileAccess.Read);
            BinaryFormatter formatter = new BinaryFormatter();
            // Reconstruct information of our friends from file.
            tc = (zoobristhasher)formatter.Deserialize(readerFileStream);
            // Close the readerFileStream when we are done
            readerFileStream.Close();
            return tc;
        }
        public void saveme()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream writerFileStream =
                 new FileStream(datapath, FileMode.Create, FileAccess.Write);
            // Save our dictionary of friends to file
            formatter.Serialize(writerFileStream, this);
            // Close the writerFileStream when we are done.
            writerFileStream.Close();
        }
    }
}
