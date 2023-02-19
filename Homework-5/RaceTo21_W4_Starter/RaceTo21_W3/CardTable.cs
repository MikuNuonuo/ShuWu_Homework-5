using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class CardTable
    {
        public CardTable()
        {
            Console.WriteLine("Setting Up Table...");
        }

        /* Shows the name of each player and introduces them by table position.
         * Is called by Game object.
         * Game object provides list of players.
         * Calls Introduce method on each player object.
         */
        public void ShowPlayers(List<Player> players)
        {
            for (int i = 0; i < players.Count; i++)
            {
                players[i].Introduce(i+1); // List is 0-indexed but user-friendly player positions would start with 1...
            }
        }

        /* Gets the user input for number of players.
         * Is called by Game object.
         * Returns number of players to Game object.
         */
        public int GetNumberOfPlayers()
        {
            Console.Write("How many players? ");
            string response = Console.ReadLine();
            int numberOfPlayers;
            while (int.TryParse(response, out numberOfPlayers) == false
                || numberOfPlayers < 2 || numberOfPlayers > 8)
            {
                Console.WriteLine("Invalid number of players.");
                Console.Write("How many players?");
                response = Console.ReadLine();
            }
            return numberOfPlayers;
        }

        /* Gets the name of a player
         * Is called by Game object
         * Game object provides player number
         * Returns name of a player to Game object
         */
        public string GetPlayerName(int playerNum)
        {
            Console.Write("What is the name of player# " + playerNum + "? ");
            string response = Console.ReadLine();
            while (response.Length < 1)
            {
                Console.WriteLine("Invalid name.");
                Console.Write("What is the name of player# " + playerNum + "? ");
                response = Console.ReadLine();
            }
            return response;
        }

        public bool OfferACard(Player player)
        {
            while (true)
            {
                Console.Write(player.name + ", do you want a card? (Y/N)");
                string response = Console.ReadLine();
                if (response.ToUpper().StartsWith("Y"))
                {
                    return true;
                }
                else if (response.ToUpper().StartsWith("N"))
                {
                    return false;
                }
                else
                {
                    Console.WriteLine("Please answer Y(es) or N(o)!");
                }
            }
        }


        /*show all cards on current players' hand*/
       public void ShowHand(Player player)
        {
            if (player.cards.Count > 0)
            {
                Console.Write(player.name + " has: ");
                //use  for loop to iterate all cards on each players' hand
                for (int i = 0; i < player.cards.Count; i++)
                {
                    if (i < player.cards.Count - 1) //if this card isn't the last one, add "," 
                    {
                        Console.Write(player.cards[i].displayName + ",");
                    }
                    else { //if it is the last card, we don't need "," in the end
                        Console.Write(player.cards[i].displayName + " ");
                    }
                    
                }
                
                Console.Write("=" + player.score + "/21 ");
                if (player.status != PlayerStatus.active)
                {
                    Console.Write("(" + player.status.ToString().ToUpper() + ")");
                }
                Console.WriteLine();
            }
        }

        /*show all cards on each players' hand*/
        public void ShowHands(List<Player> players)
        {
            foreach (Player player in players)
            {
                ShowHand(player);
            }
        }


        /*Show the name of winner 
         *Is called by Game object
         *Game object provides player's name.
         *Returns void
         */
        public void AnnounceWinner(Player player)
        {
            if (player != null)
            {
                Console.WriteLine(player.name + " wins!");
            }
            else
            {
                Console.WriteLine("Everyone busted!");
            }
            
        }

        /*Ending game
         *Is called by Game object when nobody want to play any more
         *no parameter
         *Returns void
         */
        public void RealEnd() {// page5, Level2. if nobody wants to continue playing game, it will become real end
            Console.Write("Press <Enter> to exit... ");
            while (Console.ReadKey().Key != ConsoleKey.Enter) { }
        }
    }
}