using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PacMan
{
    public class UserData
    {
        public int HighestScore;
        public int Money;
        public int PlayerID;

        public UserData(int highestScore, int money, int playerID)
        {
            HighestScore = highestScore;
            Money = money;
            PlayerID = playerID;
        }
    }
}
