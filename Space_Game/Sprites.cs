using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using My_Library;

namespace Space_Game
{

    abstract class GameSprite
    {

        //Use My_Library
        protected MyLibrary myLibrary = new MyLibrary();

        /// <summary>
        /// Texture of the sprite.
        /// </summary>
        public Texture2D texture { get; }

        public float width;


        public float height;

        /// <summary>
        /// X position of the sprite
        /// </summary>
        ///
        public float x { get; set; }

        

        /// <summary>
        /// Y position of the sprite
        /// </summary>
        public float y { get; set; }


        /// <summary>
        /// Angle of the sprite (its orientation)
        /// </summary>
        public float angle { get; set; }


        /// <summary>
        /// Size of the image relative to its original size, stored as a fraction
        /// </summary>
        public float scale { get; set; }

        public float hitRadius;

        public void Draw(SpriteBatch spriteBatch)
        {
            //Position of the sprite
            Vector2 spritePosition = new Vector2(this.x, this.y);
            Vector2 Origin = new Vector2(texture.Width / 2, texture.Height / 2);

            //Draw the sprite (https://docs.microsoft.com/en-us/previous-versions/windows/xna/bb203869(v%3dxnagamestudio.41)
            spriteBatch.Draw(texture, spritePosition, null, Color.White, this.angle, Origin, this.scale, SpriteEffects.None, 0f);
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
                    width = texture.Width * scale;
                    height = texture.Height * scale;
                }
            }
        }
    }


    class MovingSprite : GameSprite
    {

        //Constructor
        public MovingSprite(GraphicsDevice graphicsDevice, string textureName, float scale) : base(graphicsDevice, textureName, scale) { }

        public float xSpeed { get; set; }

        public float ySpeed { get; set; }

        public float xAcc { get; set; }

        public float yAcc { get; set; }

        public float angleSpeed { get; set; }

        public override void Update(float elapsedTime)
        {
            this.xSpeed += this.xAcc * elapsedTime;
            this.ySpeed += this.yAcc * elapsedTime;

            this.x +=this.xSpeed * elapsedTime;
            this.y +=this.ySpeed * elapsedTime;
            this.angle += this.angleSpeed * elapsedTime;
            
        }

        public override void Reset()
        {
            //Add reset code here.
        }
    }

    class Ship : MovingSprite
    {       
        public Ship(GraphicsDevice graphicsDevice, string textureName, float scale) : base(graphicsDevice, textureName, scale)
        {
            Reset();
            float Hitsensitivity = 0.9f;
            this.hitRadius = (this.width/2)*Hitsensitivity;

        }

        //Holds the previous keyboard state
        private KeyboardState lastState;

        //Sets how fast the ship will accelerate
        private float acceleration = 60;

        //Sets the maximum speed in any direction
        private float maxSpeed = 110;

        
        public override void Update(float elapsedTime)
        {
            

            //Get keyboard state
            KeyboardState newState = Keyboard.GetState();

            
            //Find the direction vector from the angle
            Vector2 dirVec = new Vector2();
            myLibrary.AngletoVector(this.angle, ref dirVec);

            

          

            this.xAcc = 0;
            this.yAcc = 0;
            if (newState.IsKeyDown(Keys.W) || newState.IsKeyDown(Keys.Up))
            {
               
                
                this.xAcc = dirVec.X * acceleration;
                this.yAcc = dirVec.Y * acceleration;
                
                
                
            }

            if (newState.IsKeyDown(Keys.S) || newState.IsKeyDown(Keys.Down))
            {
                

                this.xAcc = -dirVec.X * acceleration;
                this.yAcc = -dirVec.Y * acceleration;
            }
                
            if (newState.IsKeyDown(Keys.A) || newState.IsKeyDown(Keys.Left))
            {
                this.angle -= this.angleSpeed * elapsedTime;
            }

            if (newState.IsKeyDown(Keys.D) || newState.IsKeyDown(Keys.Right))
            {
                this.angle += this.angleSpeed * elapsedTime;
            }

            
            if (newState.IsKeyDown(Keys.Z)) { this.xSpeed = 0; this.ySpeed = 0; }
            
            

            //Update speed
            this.xSpeed += this.xAcc * elapsedTime;
            this.ySpeed += this.yAcc * elapsedTime;

            //Compose new velocity vector
            Vector2 newVelocity = new Vector2(this.xSpeed, this.ySpeed);

            if(newVelocity.Length()>maxSpeed)
            {
                //Normalise velocity vector
                newVelocity.Normalize();

                this.xSpeed = newVelocity.X * maxSpeed;
                this.ySpeed = newVelocity.Y * maxSpeed;
            }

            //Update position
            this.x += this.xSpeed * elapsedTime;
            this.y += this.ySpeed * elapsedTime;

            //Handle the ship when it leaves the edge of the map
            float xPos = this.x;
            float yPos = this.y;
            myLibrary.TorusWorld(ref xPos , ref yPos, this.height,this.width);
            this.x = xPos;
            this.y = yPos;
            
            lastState = newState;
        }

        public override void Reset()
        {
            this.x = (int)myLibrary.screenWidth / 2;
            this.y = (int)myLibrary.screenHeight / 2;
            this.ySpeed = 0.1f;
            this.xSpeed = 0.1f;
            this.xAcc = 0;
            this.yAcc = 0;
            this.angleSpeed = 2;
        }




    }

    class Asteroid:GameSprite
    {
        public Asteroid(GraphicsDevice graphicsDevice, string textureName, float scale, float hitsensitivity) : base(graphicsDevice, textureName, scale)
        {
            
            Reset();

            float Hitsensitivity = hitsensitivity;
            this.hitRadius = (this.width / 2) * Hitsensitivity;
        }
        public override void Update(float elapsedTime)
        {
            
        }

        public override void Reset()
        {
            //Randomly place the GravObject
            float ranX = 0;
            float ranY = 0;
            myLibrary.RanScreenPos(ref ranX, ref ranY, 0.1f, 0.9f, 0.9f, 0.1f);

            this.x = ranX;
            this.y = ranY;
        }
    }

    class GravObject : GameSprite
    {
        public GravObject(GraphicsDevice graphicsDevice, string textureName, float scale, float GravityStrength, float hitsensitivity, Dictionary<string,MovingSprite> MovingSprites) : base(graphicsDevice, textureName, scale)
        {
            gravityStrength = GravityStrength;
            movingSprites = MovingSprites;
            Reset();

            float Hitsensitivity = hitsensitivity;
            this.hitRadius = (this.width/2) * Hitsensitivity;
        }

        public float gravityStrength;
        Dictionary<string, MovingSprite> movingSprites;

        public override void Reset()
        {
            //Randomly place the GravObject
            float ranX=0;
            float ranY=0;
            myLibrary.RanScreenPos(ref ranX,ref ranY,0.1f,0.9f,0.9f,0.1f);

            this.x = ranX;
            this.y = ranY;
        }

        public override void Update(float elapsedTime)
        {

            //Pull all moving sprites towards the star
            foreach(KeyValuePair<string,MovingSprite> kvp in movingSprites)
            {
                MovingSprite sprite = kvp.Value;

                //Get sprite and gravObject positions
                Vector2 gravObjectPos = new Vector2(this.x, this.y);
                Vector2 spritePos = new Vector2(sprite.x, sprite.y);

                //Calculate direction vector between GravObject and sprite
                Vector2 dirVec = gravObjectPos - spritePos;

                //Find the distance between ship and Gravobject
                float distance = dirVec.Length();

                //Normalize the direction vector
                dirVec.Normalize();

                //Accelerate the ship toward the star
                float spritexAcc = dirVec.X * gravityStrength / distance;
                float spriteyAcc = dirVec.Y * gravityStrength / distance;

                //Update ship speed
                sprite.xSpeed += spritexAcc * elapsedTime;
                sprite.ySpeed += spriteyAcc * elapsedTime;

                //Update ship position
                sprite.x += sprite.xSpeed * elapsedTime;
                sprite.y += sprite.ySpeed * elapsedTime;
            }
        }
    }

    class ChaseAlien:MovingSprite
    {
        public ChaseAlien(GraphicsDevice graphicsDevice, string textureName, float scale, Ship Ship) : base(graphicsDevice, textureName, scale)
        {
            ship =Ship;
            Reset();

            float Hitsensitivity = 0.9f;
            this.hitRadius = (this.width/2) * Hitsensitivity;
        }

        Ship ship;
        
        float speed = 140;

        public override void Update(float elapsedTime)
        {
            //Get ship and alien positions
            Vector2 alienPos = new Vector2(this.x, this.y);
            Vector2 shipPos = new Vector2(ship.x, ship.y);

            //Calculate direction vector between GravObject and sprite
            Vector2 dirVec = shipPos-alienPos;

            //Find the distance between ship and Gravobject
            float distance = dirVec.Length();

            //Normalize the direction vector
            dirVec.Normalize();

            //Update alien position
            this.x += speed * dirVec.X * elapsedTime;
            this.y += speed * dirVec.Y * elapsedTime;
            this.angle = myLibrary.VectorToAngle(dirVec);

            float xPos = this.x;
            float yPos = this.y;
            myLibrary.TorusWorld(ref xPos, ref yPos, this.height,this.width);
            this.x = xPos;
            this.y = yPos;
        }

        public override void Reset()
        {
            //Randomly place the Alien
            float ranX = 0;
            float ranY = 0;
            myLibrary.RanScreenPos(ref ranX, ref ranY);

            this.x = ranX;
            this.y = ranY;
        }

    }

    class Missile:MovingSprite
    {
        private float speed=150;
        public Missile(GraphicsDevice graphicsDevice, string textureName, float scale,Ship ship):base(graphicsDevice,textureName,scale)
        {
            //Obtain velocity vector
            Vector2 dirVec = new Vector2();
            this.angle = ship.angle;
            myLibrary.AngletoVector(this.angle, ref dirVec);
            this.xSpeed = ship.xSpeed + dirVec.X * speed;
            this.ySpeed = ship.ySpeed + dirVec.Y * speed;

            this.x = ship.x;
            this.y = ship.y;

            float Hitsensitivity = 0.9f;
            this.hitRadius = (this.width / 2) * Hitsensitivity;
        }
    }
}






