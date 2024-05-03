using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace atomicSoko
{
    class Model
    {
        public string[,] BackGrid { get; set; }


        public Model(string compresedModel, int width, int height)
        {
            BackGrid = new string[width, height];
            
            int y = 0;
            string[] lignes = compresedModel.Split('\n');
            foreach (string lign in lignes)
            {
                int x = 0;
                string[] cells = lign.Split(';');
                foreach (string cell in cells)
                {
                    if (x >= width || y >= height) continue;
                    BackGrid[x, y] = cell;
                    x++;
                }
                y++;
            }

        }
    }
    
}
