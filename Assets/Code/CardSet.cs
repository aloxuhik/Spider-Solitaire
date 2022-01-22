using System;
using UnityEngine;

namespace Assets.Code
{
    [Serializable]
    public class CardSet
    {
        /// <summary>
        /// Just for info in Inspector
        /// </summary>
        public CardColors CardsColor;

        /// <summary>
        /// Must be ordered as values in deck 0-Ace, 13-King
        /// </summary>
        public Sprite[] Sprites;
    }
}
