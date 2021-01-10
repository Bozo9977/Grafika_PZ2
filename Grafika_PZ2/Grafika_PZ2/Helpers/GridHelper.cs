using Grafika_PZ2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Grafika_PZ2.Helpers
{
    public class GridHelper
    {
        public static void InitializeGrid(ulong[,] grid)
        {
            int x = grid.GetLength(0);
            int y = grid.GetLength(1);

            for(int i=0; i<grid.GetLength(0); i++)
            {
                for(int j =0; j < grid.GetLength(1); j++)
                {
                    grid[i, j] = 0;
                }
            }
        }

        public static void PositionElemntsWithinGrid(NetworkModel model, ulong[,] grid)
        {
            for(int i = 0; i < model.substations.Count; i++)
            {
                int currRow = (int)model.substations[i].Row, currColumn = (int)model.substations[i].Column;
                if (grid[currRow, currColumn] == 0)
                    grid[currRow, currColumn] = model.substations[i].Id;
                else
                    FindRowAndColumn(grid, currRow, currColumn, model.substations[i].Id, out currRow, out currColumn);
                model.substations[i].Row = currRow;
                model.substations[i].Column = currColumn;
            }

            for (int i = 0; i < model.switches.Count; i++)
            {
                int currRow = (int)model.switches[i].Row, currColumn = (int)model.switches[i].Column;
                if (grid[currRow, currColumn] == 0)
                {
                    grid[currRow, currColumn] = model.switches[i].Id;
                }
                else
                    FindRowAndColumn(grid, currRow, currColumn, model.switches[i].Id, out currRow, out currColumn);
                model.switches[i].Row = currRow;
                model.switches[i].Column = currColumn;
            }

            for (int i = 0; i < model.nodes.Count; i++)
            {
                int currRow = (int)model.nodes[i].Row, currColumn = (int)model.nodes[i].Column;
                if (grid[currRow, currColumn] == 0)
                    grid[currRow, currColumn] = model.nodes[i].Id;
                else
                    FindRowAndColumn(grid, currRow, currColumn, model.nodes[i].Id, out currRow, out currColumn);
                model.nodes[i].Row = currRow;
                model.nodes[i].Column = currColumn;
            }
        }

        public static void FindRowAndColumn(ulong[,] grid, int currRow, int currColumn, ulong entityId, out int row, out int column)
        {
            if (currRow > 0 && currColumn > 0 && currRow < 500 && currColumn < 500)
            {
              
                int step = 1;
                bool found = false;

                while (!found)
                {
                    if(currRow >= step && currColumn >= step)
                    {
                        if (grid[currRow - step, currColumn] == 0)
                        {
                            grid[currRow - step, currColumn] = entityId;
                            currRow -= step;
                            found = true;
                            break;
                        }
                        else if (grid[currRow, currColumn - step] == 0)
                        {
                            grid[currRow, currColumn - step] = entityId;
                            currColumn -= step;
                            found = true;
                            break;
                        }
                        else if (grid[currRow - step, currColumn - step] == 0)
                        {
                            grid[currRow - step, currColumn - step] = entityId;
                            currRow -= step;
                            currColumn -= step;
                            found = true;
                            break;
                        }
                    }
                    if(currRow + step < 500 && currColumn + step < 500)
                    {
                        if (grid[currRow, currColumn + step] == 0)
                        {
                            grid[currRow, currColumn + step] = entityId;
                            currColumn += step;
                            found = true;
                            break;
                        }
                        else if (grid[currRow + step, currColumn] == 0)
                        {
                            grid[currRow + step, currColumn] = entityId;
                            currRow += step;
                            found = true;
                            break;
                        }
                        else if (grid[currRow + step, currColumn + step] == 0)
                        {
                            grid[currRow + step, currColumn + step] = entityId;
                            currRow += step;
                            currColumn += step;
                            found = true;
                            break;
                        }
                    }
                    if(currRow >= step && currColumn + step < 500)
                    {
                        if (grid[currRow - step, currColumn + step] == 0)
                        {
                            grid[currRow - step, currColumn + step] = entityId;
                            currRow -= step;
                            currColumn += step;
                            found = true;
                            break;
                        }
                    }
                    if(currRow + step < 500 && currColumn >= step)
                    {
                        if (grid[currRow + step, currColumn - step] == 0)
                        {
                            grid[currRow + step, currColumn - step] = entityId;
                            currRow += step;
                            currColumn -= step;
                            found = true;
                            break;
                        }
                    }
                    
                    if (++step == 10)
                        break;
                    
                }
            }

            row = currRow;
            column = currColumn;
        }
    }
}
