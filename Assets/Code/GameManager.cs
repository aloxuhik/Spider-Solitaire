using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        public DealSettings DealSettings;

        public Text DealButtonText;

        public Sprite Cardback;
        public Card CardPrefab;

        public List<Card> Cards;

        public GameObject CardHolder;

        public Column[] Columns;

        /// <summary>
        /// Cards sets must be ordered same as enum CardColors
        /// 0 - Club, 1 - Hearts ...
        /// </summary>
        public List<CardSet> CardSets;

        public int DealtCardsIndex = 0;
        public int StockDealt = 0;

        // Start is called before the first frame update
        void Start()
        {
            Instance = this;
            DealButtonText.text = "Deal Another - " + 5;

            Columns = new Column[10];

            GenerateCards(DealSettings);
            
            for (int i = 0; i < 10; i++)
            {
                Columns[i] = new Column();
                Columns[i].YCardOffset = -0.3f;
                Columns[i].XOrigin = i * 1.1f;
            }

            Shuffle(Cards);
            DealCards();
        }

        private void GenerateCards(DealSettings dealSettings)
        {
            if(dealSettings == DealSettings.OneSuit)
            {
                for (int i = 0; i < 8; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Club));
                }
            }
            else if(dealSettings == DealSettings.TwoSuit)
            {
                for (int i = 0; i < 4; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Club));
                }

                for (int i = 0; i < 4; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Hearth));
                }
            }
            else if(dealSettings == DealSettings.FourSuit)
            {
                for (int i = 0; i < 2; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Club));
                }

                for (int i = 0; i < 2; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Hearth));
                }
                for (int i = 0; i < 2; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Diamond));
                }

                for (int i = 0; i < 2; i++)
                {
                    Cards.AddRange(GenerateCards(CardColors.Spade));
                }
            }
        }

        public void DealFromStock()
        {
            if (StockDealt == 5)
                return;

            for (int i = 0; i < 10; i++)
            {
                Columns[i].AddCard(Cards[DealtCardsIndex]);

                Cards[DealtCardsIndex].Fliped = true;
                Cards[DealtCardsIndex].Pickable = true;

                Columns[i].RefreshPickable();
                DealtCardsIndex++;
            }
            StockDealt++;

            DealButtonText.text = "Deal Another - " + (5 - StockDealt);
        }

        void DealCards()
        {
            for (int i = 0; i < 10; i++)
            {
                int unFlipedCards = 5;
                if (i > 3)
                {
                    unFlipedCards = 4;
                }

                Columns[i].AddCards(Cards.GetRange(DealtCardsIndex, unFlipedCards));
                DealtCardsIndex += unFlipedCards;

                Cards[DealtCardsIndex].Fliped = true;
                Cards[DealtCardsIndex].Pickable = true;
                Columns[i].AddCard(Cards[DealtCardsIndex]);
                DealtCardsIndex++;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        Card[] GenerateCards(CardColors color)
        {
            Card[] cards = new Card[13];

            Sprite[] sprites = CardSets[(int)color].Sprites;

            for (int i = 0; i < 13; i++)
            {
                Card newCard = GameObject.Instantiate<Card>(CardPrefab, 
                    CardHolder.transform);

                cards[i] = newCard;
                cards[i].Value = i;
                cards[i].CardColor = color;
                cards[i].CarbackSprite = Cardback;
                cards[i].Sprite = sprites[i];

                cards[i].Fliped = false;
            }

            return (cards);
        }

        /// <summary>
        /// Knuth Shuffle
        /// </summary>
        /// <param name="cards"></param>
        void Shuffle(List<Card> cards)
        {
            System.Random random = new System.Random();
            int n = cards.Count - 1;
            while (n > 1)
            {
                int k = random.Next(n);
                Card temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
                n--;
            }
        }

        public void SortDropedCard(Card card)
        {
            var colX = card.transform.position.x / 1.1f;

            int colNum = Mathf.RoundToInt(colX);

            if (Columns[colNum].CanBeDroped(card))
            {
                if(card.Children!=null)
                card.ParentColumn.RemoveCards(card.Children);
                card.ParentColumn.RemoveCard(card);
                card.ParentColumn.RefreshPickable();

                Columns[colNum].AddCard(card);
                if (card.Children != null)
                    Columns[colNum].AddCards(card.Children);

                Columns[colNum].RefreshPickable();
                Columns[colNum].CheckFinishedSequence();

            }
            else
            {
                card.ReturnToOriginalPosition();
            }
        }
    }
}
