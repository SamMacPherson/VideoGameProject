using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Space_Game
{
    abstract class GameSprite
    {
        /// <summary>
        /// Texture of the sprite.
        /// </summary>
        Texture2D texture { get; }


        /// <summary>
        /// X position of the sprite
        /// </summary>
        ///
        protected float x { get; set; }


        /// <summary>
        /// Y position of the sprite
        /// </summary>
        protected float y { get; set; }


        /// <summary>
        /// Angle of the sprite (its orientation)
        /// </summary>
        protected float angle { get; set; }


        /// <summary>
        /// Size of the image relative to its original size, stored as a fraction
        /// </summary>
        protected float scale { get; set; }



        public void Draw(SpriteBatch spriteBatch)
        {
            //Position of the sprite
            Vector2 spritePosition = new Vector2(this.x, this.y);
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            //Draw the sprite (https://docs.microsoft.com/en-us/previous-versions/windows/xna/bb203869(v%3dxnagamestudio.41)
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, Origin, 1.0f, SpriteEffects.None, 0f);
        }

        //All sprites need the abilty to reset, and update.
        abstract public void Update(float elapsedTime);
        abstract public void Reset();

        //Constructor, set the scale and texture
        public GameSprite(GraphicsDevice graphicsDevice, string textureName, float scale)
        {
            this.scale = scale;
            if (texture == null)
            {
                using (var stream = TitleContainer.OpenStream(textureName))
                {
                    texture = Texture2D.FromStream(graphicsDevice, stream);
                }

            }
        }
    }


    class MovingSprite : GameSprite
    {

        //Constructor
        public MovingSprite(GraphicsDevice graphicsDevice, string textureName, float scale) : base(graphicsDevice, textureName, scale) { }

        protected float xSpeed { get; set; }

        protected float ySpeed { get; set; }

        protected float angleSpeed { get; set; }

        public override void Update(float elapsedTime)
        {
            this.x += this.xSpeed * elapsedTime;
            this.y += this.ySpeed * elapsedTime;
            this.angle += this.angleSpeed * elapsedTime;
        }

        public override void Reset()
        {
            //Insert Reset code 
        }
    }
}

