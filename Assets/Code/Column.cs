using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    [System.Serializable]
    public class Column
    {
        public void RefreshRenderOrder()
        {
            for (int i = 0; i < Cards.Count; i++)
            {
                Cards[i].SetZOrder(i);
            }
        }

        public bool CanBeDroped(Card card)
        {
            if (Cards.Count == 0)
                return true;

            return Cards[Cards.Count - 1].Value == card.Value + 1;
        }

        public List<Card> Cards;
        public float XOrigin;

        //Move it somewhere as it is same for all columns
        public float YCardOffset;

        public Column()
        {
            Cards = new List<Card>();
        }

        public void AddCard(Card card)
        {
            card.transform.position = new Vector3(XOrigin, Cards.Count * YCardOffset);
            card.ParentColumn = this;
            Cards.Add(card);
            card.SetZOrder(Cards.Count);
        }

        public void AddCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                AddCard(card);
            }
        }

        public void RemoveCard(Card card)
        {
            Cards.Remove(card);
            if (Cards.Count > 0)
                if (!Cards[Cards.Count - 1].Fliped)
                {
                    Cards[Cards.Count - 1].Fliped = true;
                    Cards[Cards.Count - 1].Pickable = true;
                }
        }

        public void RemoveCards(List<Card> cards)
        {
            foreach (Card card in cards)
            {
                RemoveCard(card);
            }
        }

        public List<Card> GetChildrenCards(Card card)
        {
            int cardIndex = Cards.IndexOf(card);

            if (cardIndex == Cards.Count - 1)
                return (null);

            cardIndex++;

            return Cards.GetRange(cardIndex, Cards.Count - cardIndex);
        }

        


        public void RefreshPickable()
        {
            if (Cards.Count == 0)
                return;

            for (int i = 0; i < Cards.Count; i++)
                Cards[i].Pickable = false;

            Cards[Cards.Count - 1].Pickable = true;
            for (int i = Cards.Count - 2; i >= 0; i--)
            {
                if (Cards[i].Value == Cards[i + 1].Value + 1 && Cards[i].Fliped &&
                    Cards[i].CardColor == Cards[i+1].CardColor)
                    Cards[i].Pickable = true;
                else
                    break;
            }
        }

        public void CheckFinishedSequence()
        {
            int value = 0;
            CardColors cardColor = Cards[Cards.Count - 1].CardColor;

            //Reverse loop, becuase of card stacking
            for (int i = Cards.Count - 1; i >= 0; i--)
            {                
                if (Cards[i].Value != value || Cards[i].CardColor != cardColor)
                    break;

                if(Cards[i].Value == 12)
                {
                    var doneCards = Cards.GetRange(i, (Cards.Count )-i);

                    RemoveCards(doneCards);
                    RefreshPickable();
                    //Move cards from table.
                    for(int u = 0; u < doneCards.Count; u++)
                    {
                        doneCards[u].Pickable = false;
                        doneCards[u].transform.position = new Vector3(-1, 0);
                    }
                }
                value++;
            }
        }
    }
}
