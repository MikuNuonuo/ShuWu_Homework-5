using System;
using System.Collections.Generic;

namespace RaceTo21
{
    public class Game
    {
        public int numberOfPlayers {
            get;private set;
        } // number of players in current game
        List<Player> players = new List<Player>(); // list of objects containing player data
        public CardTable cardTable {
            get; private set;
        } // object in charge of displaying game information
        public Deck deck {
            get; private set;
        } // deck of cards
        private int currentPlayer = 0; // current player on list
        public Task nextTask {
            get; private set;
        } // keeps track of game state
        private bool cheating = true; // lets you cheat for testing purposes if true

        public Game(CardTable c)
        {
            cardTable = c;
            deck = new Deck();
            deck.Shuffle();
            deck.ShowAllCards();
            nextTask = Task.GetNumberOfPlayers;
        }

        /* Adds a player to the current game
         * Called by DoNextTask() method
         */
        public void AddPlayer(string n)
        {
            players.Add(new Player(n));
        }

        /* Figures out what task to do next in game
         * as represented by field nextTask
         * Calls methods required to complete task
         * then sets nextTask.
         * calls DealTopCard() method
         */
        public void DoNextTask()
        {
            Console.WriteLine("================================"); // this line should be elsewhere right?
            if (nextTask == Task.GetNumberOfPlayers)
            {
                numberOfPlayers = cardTable.GetNumberOfPlayers();
                nextTask = Task.GetNames;
            }
            else if (nextTask == Task.GetNames)
            {
                for (var count = 1; count <= numberOfPlayers; count++)
                {
                    var name = cardTable.GetPlayerName(count);
                    AddPlayer(name); // NOTE: player list will start from 0 index even though we use 1 for our count here to make the player numbering more human-friendly
                }
                nextTask = Task.IntroducePlayers;
            }
            else if (nextTask == Task.IntroducePlayers)
            {
                cardTable.ShowPlayers(players);
                nextTask = Task.PlayerTurn;
            }
            else if (nextTask == Task.PlayerTurn)
            {
                cardTable.ShowHands(players);
                Player player = players[currentPlayer];
                if (player.status == PlayerStatus.active)
                {
                    if (cardTable.OfferACard(player))
                    {
                        Console.Write("How many cards do you want?(up to 3)"); //page2, Level1 player can choose to draw up to 3 cards each turn  
                        string response = Console.ReadLine();
                        int cardCount; //count how much card play want
                        while (int.TryParse(response, out cardCount) == false || cardCount > 3 || cardCount < 1)// check whether invalid input
                        {
                            Console.WriteLine("Invalid number of cards.");
                            Console.Write("How many cards do you want?(up to 3)");
                            response = Console.ReadLine();
                        }
                        List<Card> GetCardOneTime = deck.DealTopCard(cardCount); //give player all cards they choose
                        foreach (Card card in GetCardOneTime) 
                        {
                            player.cards.Add(card);
                        }
                        player.score = ScoreHand(player);
                        if (player.score > 21)
                        {
                            player.status = PlayerStatus.bust;
                        }
                        else if (player.score == 21)
                        {
                            player.status = PlayerStatus.win;
                        }
                    }
                    else
                    {
                        player.status = PlayerStatus.stay;
                    }
                }
                cardTable.ShowHand(player);
                nextTask = Task.CheckForEnd;
            }
            else if (nextTask == Task.CheckForEnd)
            {
                if (!CheckActivePlayers())
                {
                    Player winner = DoFinalScoring();
                    cardTable.AnnounceWinner(winner); //output the winner of this round

                    //new round
                    List<Player> ContinuePlayers = new List<Player>(); //page2, level2, checke whether players want to continue games
                    foreach (Player player in players){
                        bool loop = true; //tracking whether player says Y or N
                        while (loop)
                        {   
                            Console.Write(player.name + ", do you want a new round? (Y/N)");
                            string response = Console.ReadLine();
                            if (response.ToUpper().StartsWith("Y")) //if player choose Y
                            {
                                player.cards.Clear(); //clear all hand cards
                                player.score = 0; //reset score
                                player.status = PlayerStatus.active; // player state become active
                                ContinuePlayers.Add(player); // add player into continueplayers list
                                loop = false; // Jump out of the loop
                            }
                            else if (response.ToUpper().StartsWith("N")) //if player choose N
                            {
                                numberOfPlayers--; // player number --
                                loop = false; // Jump out of the loop
                            }
                            else
                            {
                                Console.WriteLine("Please answer Y(es) or N(o)!"); //check invalid input
                            }
                        }
                    }
                    if (numberOfPlayers >= 2) //if more than 1 player continue playing game, game will begin again
                    {
                        players = ContinuePlayers;
                        deck = new Deck();//new deck to start a new game
                        deck.Shuffle();
                        currentPlayer = 0; //back to the first player
                        PlayerShuffle(); //shuffle players
                        players.Remove(winner);//remove winner from players
                        players.Add(winner);//add winner to the end of players as the dealer
                        nextTask = Task.IntroducePlayers;
                    }
                    else
                    {
                        if (numberOfPlayers == 1) { //if only 1 player continue playing game, game over and the only one player will win
                            Console.WriteLine(players[0].name + " is final winner!");
                        }
                        cardTable.RealEnd();
                        nextTask = Task.GameOver;
                    }
                }
                else
                {
                    currentPlayer++;
                    if (currentPlayer > players.Count - 1)
                    {
                        currentPlayer = 0; // back to the first player...
                    }
                    nextTask = Task.PlayerTurn;
                }
            }
            else // we shouldn't get here...
            {
                Console.WriteLine("I'm sorry, I don't know what to do now!");
                nextTask = Task.GameOver;
            }
        }

        public int ScoreHand(Player player)
        {
            int score = 0;
            if (cheating == true && player.status == PlayerStatus.active)
            {
                string response = null;
                while (int.TryParse(response, out score) == false)
                {
                    Console.Write("OK, what should player " + player.name + "'s score be?");
                    response = Console.ReadLine();
                }
                return score;
            }
            else
            {
                foreach (Card card in player.cards)
                {
                    string faceValue = card.id.Remove(card.id.Length - 1);
                    switch (faceValue)
                    {
                        case "K":
                        case "Q":
                        case "J":
                            score = score + 10;
                            break;
                        case "A":
                            score = score + 1;
                            break;
                        default:
                            score = score + int.Parse(faceValue);
                            break;
                    }
                }
            }
            return score;
        }

        public bool CheckActivePlayers()
        {
            int bustNumber = 0; //count number of bust players 
            int stayNumber = 0; // count number of stay players 
            foreach (var player in players)
            {
                if (player.status == PlayerStatus.win)
                {
                    return false; //if anyone is won, stop checking
                }
                if (player.status == PlayerStatus.bust)
                {
                    bustNumber++; //count bust players
                }
                if (player.status == PlayerStatus.stay)
                {
                    stayNumber++; //count stay players
                }

            }
            if (bustNumber == numberOfPlayers - 1)
            {
                return false; //Just one remaining players
            }
            if (stayNumber == numberOfPlayers - bustNumber) {
                return false; //still alive player select stay
            }
            return true; //nobody win or at least 2 remaining players 
        }


        public Player DoFinalScoring()
        {
            int highScore = 0;
            foreach (var player in players)
            {
                cardTable.ShowHand(player);
                if (player.status == PlayerStatus.win) // someone hit 21
                {
                    return player;
                }
                if (player.status == PlayerStatus.active)
                {
                    return player; //when just one player is active means he/she is the last remaining player and is the winner.
                }
                if (player.status == PlayerStatus.stay) // still could win...
                {
                    if (player.score > highScore)
                    {
                        highScore = player.score;
                    }
                }
                
                // if busted don't bother checking!
            }
            if (highScore > 0) // someone scored, anyway!
            {
                // find the FIRST player in list who meets win condition
                return players.Find(player => player.score == highScore);
            }
            return null; // everyone must have busted because nobody won!
        }

        /* Shuffle players list
         * Is called by DoNextTask() in Game and when CheckActivePlayers is false and start a new game
         * no parameter
         * no return
         */
        public void PlayerShuffle()// page2, level2
        {
            Console.WriteLine("Shuffling players...");

            Random rng = new Random();

            for (int i = 0; i < players.Count; i++)
            {
                Player tmp = players[i];
                int swapindex = rng.Next(players.Count);
                players[i] = players[swapindex];
                players[swapindex] = tmp;
            }
        }
    }
}
