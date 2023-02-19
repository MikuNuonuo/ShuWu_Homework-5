using System;
using System.Collections.Generic;
using System.Linq; // currently only needed if we use alternate shuffle method

namespace RaceTo21
{
    public class Deck
    {
        public List<Card> cards = new List<Card>(); //change Sting to Card
        public Dictionary<String, String> cardIdToImage = new Dictionary<string, string>(); //using dictionary to store each cards ID and it's associates image name 

        public Deck()
        {
            Console.WriteLine("*********** Building deck...");
            string[] suits = { "Spades", "Hearts", "Clubs", "Diamonds" };

            for (int cardVal = 1; cardVal <= 13; cardVal++)
            {
                foreach (string cardSuit in suits)
                {
                    string cardName;
                    string cardLongName;

                    switch (cardVal)
                    {
                        case 1:
                            cardName = "A";
                            cardLongName = "Ace";
                            break;
                        case 11:
                            cardName = "J";
                            cardLongName = "Jack";
                            break;
                        case 12:
                            cardName = "Q";
                            cardLongName = "Queen";
                            break;
                        case 13:
                            cardName = "K";
                            cardLongName = "King";
                            break;
                        default:
                            cardName = cardVal.ToString();
                            cardLongName = cardName;
                            break;
                    }
                    if (cardVal < 10 && cardVal > 1){// show the cards name
                        cardIdToImage.Add(cardName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_0" + cardName + ".png");
                    }
                    else {
                        cardIdToImage.Add(cardName + cardSuit.First<char>(), "card_" + cardSuit.ToLower() + "_" + cardName + ".png");
                    }
                    cards.Add(new Card(cardName + cardSuit.First<char>(), cardLongName + " of " + cardSuit));
                }
            }
            /*foreach (String s in carIdToImage.Values) {
                Console.Write(s + " ");
            }*/
        }

        public void Shuffle()
        {
            Console.WriteLine("Shuffling Cards...");

            Random rng = new Random();

            // one-line method that uses Linq:
            // cards = cards.OrderBy(a => rng.Next()).ToList();

            // multi-line method that uses Array notation on a list!
            // (this should be easier to understand)
            for (int i=0; i<cards.Count; i++)
            {
                Card tmp = cards[i]; // change Sting to Card
                int swapindex = rng.Next(cards.Count);
                cards[i] = cards[swapindex];
                cards[swapindex] = tmp;
            }
        }

        /* Maybe we can make a variation on this that's more useful,
         * but at the moment it's just really to confirm that our 
         * shuffling method(s) worked! And normally we want our card 
         * table to do all of the displaying, don't we?!
         */

        public void ShowAllCards()
        {
            for (int i=0; i<cards.Count; i++)
            {
                //Console.Write(i+":"+cards[i].id); // a list property can look like an Array!
                if (i < cards.Count -1)
                {
                    Console.Write(" ");
                } else
                {
                    Console.WriteLine("");
                }
            }
        }

        /*Take cards from deck and give them to player
        *Is called by DoNextTask() method of Game object
        *need paramter cardCount to decide how many cards player want to draw
        *Game object provides number of cards
        *Returns picked cards to Game object
        */
        public List<Card> DealTopCard(int cardCount) // change Sting to Card
        {
            List<Card> GetCardOneTime = new List<Card>(); // create a new list to store all cards player get in one turn
            for (int i = 0; i < cardCount; i++) { // page2, Level1 use for loop to get cardCount number of cards 
                Card card = cards[cards.Count - 1]; 
                GetCardOneTime.Add(card);
                cards.RemoveAt(cards.Count - 1);
                // Console.WriteLine("I'm giving you " + card);
            }
            return GetCardOneTime;
        }
    }
}

