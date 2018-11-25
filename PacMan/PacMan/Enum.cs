using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public enum Directions
    {
        Up = 0,
        Down = 1,
        Right = 2,
        Left = 3,
        None
    }

    public enum States
    {
        Login,
        SkinSelection,
        Play,
        Lose,
        Win
    }
}
