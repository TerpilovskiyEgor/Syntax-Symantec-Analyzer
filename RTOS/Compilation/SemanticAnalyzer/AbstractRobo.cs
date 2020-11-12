using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RTOS.Compilation.SemanticAnalyzer
{
    class AbstractRobo
    {
        public int X = 150, Y = 288;
        public int Direction = 1;

        public bool HasItem = false;
        public bool HasBox = false;

        public int LimXmax = 355;
        public int LimXmin = 59;
        public int LimYmax = 319;
        public int LimYmin = 77;

        public bool IfMotionForward(int step)
        {
            switch (Direction)
            {
                case 0:
                    if (Y - step * 6 > LimYmin)
                    {
                        Y -= step * 6;
                        return true;
                    }
                    return false;
                case 1:
                    if (X + step * 6 < LimXmax)
                    {
                        X += step * 6;
                        return true;
                    }
                    return false;
                case 2:
                    if (Y + step * 6 < LimYmax)
                    {
                        Y += step * 6;
                        return true;
                    }
                    return false;
                case 3:
                    if (X - step * 6 > LimXmin)
                    {
                        X -= step * 6;
                        return true;
                    }
                    return false;
            }

            return true;
        }

        public bool IfMotionBack(int step)
        {
            switch (Direction)
            {
                case 0:
                    if (Y + step * 6 < LimYmax)
                    {
                        Y += step * 6;
                        return true;
                    }
                    return false;
                case 1:
                    if (X - step * 6 > LimXmin)
                    {
                        X -= step * 6;
                        return true;
                    }
                    return false;
                case 2:
                    if (Y - step * 6 > LimYmin)
                    {
                        Y -= step * 6;
                        return true;
                    }
                    return false;
                case 3:
                    if (X + step * 6 < LimXmax)
                    {
                        X += step * 6;
                        return true;
                    }
                    return false;
            }
            return true;
        }

        public void TurnLeft()
        {
            Direction = (--Direction + 4) % 4;
        }

        public void TurnRight()
        {
            Direction = (++Direction) % 4;
        }

        public bool IfTake()
        {
            if (!HasItem && !HasBox && X==354 && Y==288)
            {
                HasItem = true;
                return true;
            }
            else
                return false;
        }

        public bool IfSeal()
        {
            if (HasItem && !HasBox && X == 150 && Y == 78)
            {
                HasItem = false;
                HasBox = true;
                return true;
            }
            else
                return false;
        }

        public bool IfPut()
        {
            if (HasBox || HasItem)
            {
                HasItem = false;
                HasBox = false;
                return true;
            }
            else
                return false;
        }


    }
}
