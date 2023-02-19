using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RaceTo21
{
    public class Card
    {
        public string id {
            get;private set;
        }
        public string displayName {
            get;private set;
        }

        public Card(string shortCardName, string longCardName)
        {
            id = shortCardName;
            displayName = longCardName;
        }
    }
}
