using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_Game
{
    class GameManager
    {
        //List to hold all the game sprites
        List<GameSprite> Sprites = new List<GameSprite>();



        /// <summary>
        /// Updates all sprites in the game
        /// </summary>
        public void UpdateAllSprites(float elapsedTime)
        {
            foreach (GameSprite sprite in Sprites)
            {
                sprite.Update(elapsedTime);
            }
        }

        /// <summary>
        /// Resets all sprites in the game
        /// </summary>
        public void ResetAllSprites()
        {
            foreach (GameSprite sprite in Sprites)
            {
                sprite.Reset();
            }
        }

        /// <summary>
        /// Game score
        /// </summary>
        float Score
        {
            get;
            set;
        }
    }



}
