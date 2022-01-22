using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Code
{
    public class KnuthShuffleExample : MonoBehaviour
    {
        public List<CardSet> CardSets;
        public Card CardPrefab;
        public Sprite Cardback;
        public GameObject CardHolder;

        public Text TextToShow;

        public GameObject Tick;
        public Card[] cards;

        bool nextStep = false;

        // Start is called before the first frame update
        void Start()
        {
            cards = GenerateCards(CardColors.Hearth);

            SortCardsPosition(cards.ToList());

            StartCoroutine(ShuffleCouroutine(cards.ToList()));
        }


        void SortCardsPosition(List<Card> cardsToShow)
        {
            for (int i = 0; i < cardsToShow.Count; i++)
            {
                cardsToShow[i].transform.position = new Vector3(i, 0);
            }
        }

        // Update is called once per frame
        void Update()
        {

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

        public void NextStep()
        {
            nextStep = true;
        }

        IEnumerator ShuffleCouroutine(List<Card> cards)
        {
            System.Random random = new System.Random();
            int n = cards.Count - 1;
            while (n > 1)
            {
                if (!nextStep)
                {
                    yield return null;
                    continue;
                }
                nextStep = false;
                for (int i = n; i >= 0; i--)
                    cards[i].Pickable = true;

                int k = random.Next(n);
               
                TextToShow.text = (n+1).ToString() + " switched with " + (k+1).ToString();
                var gameObject = Instantiate(Tick);
                gameObject.transform.position = new Vector3(n, 0);

                cards[n].Pickable = false;
                cards[k].Pickable = false;

                Card temp = cards[n];
                cards[n] = cards[k];
                cards[k] = temp;
                n--;

                for (int i = 0; i < cards.Count; i++)
                {
                    cards[i].transform.position = new Vector3(i, 0);
                }


               //yield return new WaitForSeconds(4f);
            }

            var Temp = Instantiate(Tick);
            Temp.transform.position = new Vector3(1, 0);
            Temp = Instantiate(Tick);
            Temp.transform.position = new Vector3(0, 0);
        }

        Card[] GenerateCards(CardColors color)
        {
            Card[] cards = new Card[13];

            Sprite[] sprites = CardSets[(int)color].Sprites;

            for (int i = 0; i < 13; i++)
            {
                Card newCard = GameObject.Instantiate<Card>(CardPrefab, CardHolder.transform);

                cards[i] = newCard;
                cards[i].Value = i;
                cards[i].CardColor = color;
                cards[i].CarbackSprite = Cardback;
                cards[i].Sprite = sprites[i];

                cards[i].Fliped = true;
                cards[i].Pickable = true;
            }

            return (cards);
        }
    }
}