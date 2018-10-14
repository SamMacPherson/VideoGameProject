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
    class GameManager
    {
        //Use My_Library
        MyLibrary myLibrary = new MyLibrary();
        
        Random random = new Random();

        private float GameTime=0;

        public void SetGameTimeToZero()
        {
            GameTime = 0;
        }

        public void AddtoGameTime(float elapsedTime)
        {
            GameTime += elapsedTime;
        }
            
        //Enum type to manage the game state
        public enum GameState
        {
            Titlescreen,
            Gameplay,
            GameEnd
        }



        public GameState gameState;

        private float numberOfMissiles = 0;

        private float numberOfAliens = 0;

        public Ship ship;

        //List to hold all the game sprites
        //List<GameSprite> Sprites = new List<GameSprite>();
        public Dictionary<string, GameSprite> Sprites = new Dictionary<string, GameSprite>();


        //List to hold all moving sprites
        //public List<MovingSprite> movingSprites = new List<MovingSprite>();
        public Dictionary<string, MovingSprite> movingSprites = new Dictionary<string, MovingSprite>();

        //Dictionary to hold all aliens
        Dictionary<string, GameSprite> aliens = new Dictionary<string, GameSprite>();

        //Dictionary to hold all non moving objects
        Dictionary<string, GameSprite> stillObjects = new Dictionary<string, GameSprite>();

        //Dictionary to hold missiles
        Dictionary<string, GameSprite> missiles = new Dictionary<string, GameSprite>();

        public SpriteFont titleFont;  

        /// <summary>
        /// Adds a Gamesprite object to the list of game sprites
        /// </summary>
        public void AddtoSprites(string Key,GameSprite sprite)
        {
            Sprites.Add(Key, sprite);
        }

        public void AddtoMissiles(string Key,MovingSprite sprite)
        {
            missiles.Add(Key, sprite);
        }

        public void AddtoStillObjects(string Key, GameSprite sprite)
        {
            stillObjects.Add(Key, sprite);
        }

        public void AddtoMovingSprites(string Key,MovingSprite sprite)
        {
            movingSprites.Add(Key,sprite);
        }

        public void AddtoAliens(string Key, MovingSprite sprite)
        {
            aliens.Add(Key, sprite);
        }


        /// <summary>
        /// Updates all sprites in the game
        /// </summary>
        public void UpdateAllSprites(float elapsedTime)
        {
            foreach (KeyValuePair<string,GameSprite> kvp in Sprites)
            {
                kvp.Value.Update(elapsedTime);
            }
        }

        


        /// <summary>
        /// Draws all game sprites
        /// </summary>
        public void DrawAllSprites(SpriteBatch spritebatch)
        {
            foreach(KeyValuePair<string, GameSprite> kvp in Sprites)
            {
                kvp.Value.Draw(spritebatch);
            }
        }



        /// <summary>
        /// Resets all sprites in the game
        /// </summary>
        public void ResetAllSprites()
        {
            foreach (KeyValuePair<string, GameSprite> kvp in Sprites)
            {
                kvp.Value.Reset();
            }
        }

        float prevFireTime;
       
        public void UpdateGame(GameState GameState, float elapsedTime,Ship ship,GraphicsDevice graphicsDevice)
        {
            switch(GameState)
            {
                case GameState.Titlescreen:
                    //Titlescreen is static, so need to detect if the space bar is pressed, if it is, start the game
                    KeyboardState state = Keyboard.GetState();
                    if(state.IsKeyDown(Keys.Space))
                    {
                        gameState = GameState.Gameplay;
                    }
                    break;

                case GameState.Gameplay:
                    //If in game, update the sprites
                    UpdateAllSprites(elapsedTime);
                    AddtoGameTime(elapsedTime);
                    KeyboardState statetemp = Keyboard.GetState();
                    if(statetemp.IsKeyDown(Keys.P))
                    {
                        gameState = GameState.GameEnd;
                    }


                    
                    double chaseAlienProbability = 0.003 ;
                    if(random.NextDouble()<chaseAlienProbability)
                    {
                        string alienName = "chaseAlien" + numberOfAliens;
                        ChaseAlien newChaseAlien = new ChaseAlien(graphicsDevice, "chaseAlien.png", 0.25f, ship);
                        //Keep track of aliens
                        AddtoSprites(alienName,newChaseAlien);
                        AddtoMovingSprites(alienName,newChaseAlien);
                        AddtoAliens(alienName, newChaseAlien);


                        numberOfAliens++;
                        
                    }


                   
                    
                    if(statetemp.IsKeyDown(Keys.Space))
                    {
                        string missileName = "missile" + numberOfMissiles;
                        if (GameTime - prevFireTime > 0.5f) 
                        {
                            Missile newMissile = new Missile(graphicsDevice, "rocket.png", 0.04f, ship);
                            AddtoSprites(missileName, newMissile);
                            AddtoMovingSprites(missileName, newMissile);
                            AddtoMissiles(missileName, newMissile);
                            prevFireTime = GameTime;
                        }
                        

                        numberOfMissiles++;
                    }

                    Dictionary<string, GameSprite> aliensCopy = new Dictionary<string, GameSprite>(aliens);
                    Dictionary<string, MovingSprite> movingSpritesCopy = new Dictionary<string, MovingSprite>(movingSprites);
                    Dictionary<string, GameSprite> SpritesCopy = new Dictionary<string, GameSprite>(Sprites);
                    Dictionary<string, GameSprite> missilesCopy = new Dictionary<string, GameSprite>(missiles);


                    //foreach(KeyValuePair<string,GameSprite> pairOuter in aliens)
                    //{
                    //    foreach(KeyValuePair<string, GameSprite> pairInner in aliens)
                    //    {
                    //        if(pairInner.Key!=pairOuter.Key)
                    //        {
                    //            Console.WriteLine("Why?");
                    //            if(CircleCollision(pairInner.Value,pairOuter.Value))
                    //            {
                    //                Console.WriteLine("Alien Alien Collision");
                    //                aliensCopy.Remove(pairInner.Key);
                    //                movingSpritesCopy.Remove(pairInner.Key);
                    //                SpritesCopy.Remove(pairInner.Key);

                    //                aliensCopy.Remove(pairOuter.Key);
                    //                movingSpritesCopy.Remove(pairOuter.Key);
                    //                SpritesCopy.Remove(pairOuter.Key);
                    //            }
                    //        }
                    //    }
                    //}

                    foreach (KeyValuePair<string,GameSprite> kvp in Sprites)
                    {
                        //The sprite is the value of the dictionary
                        GameSprite sprite = kvp.Value;
                        string name = kvp.Key;

                        //Don't check for a collision with itself
                        if (name != "ship" & name.Contains("missile") == false) 
                        {
                            
                            if (CircleCollision(ship,sprite))
                            {
                                
                                gameState = GameState.GameEnd;
                            }

                        }

                        if(name.Contains("missile"))
                        {
                            foreach(KeyValuePair<string,GameSprite> j in stillObjects)
                            {
                                if(CircleCollision(sprite,j.Value))
                                {
                                    SpritesCopy.Remove(name);
                                    movingSpritesCopy.Remove(name);
                                    missilesCopy.Remove(name);
                                }
                            }
                        }
                        if (name.Contains("Alien"))
                        {
                            foreach(KeyValuePair<string,GameSprite> pear in stillObjects)
                            {
                                if(CircleCollision(sprite,pear.Value))
                                {
                                    
                                    SpritesCopy.Remove(name);
                                    movingSpritesCopy.Remove(name);
                                    aliensCopy.Remove(name);
                                    Score += 1;
                                }
                            }

                            foreach(KeyValuePair<string,GameSprite> i in missiles)
                            {
                                if(CircleCollision(i.Value,sprite))
                                {
                                    SpritesCopy.Remove(name);
                                    movingSpritesCopy.Remove(name);
                                    aliensCopy.Remove(name);

                                    SpritesCopy.Remove(i.Key);
                                    movingSprites.Remove(i.Key);
                                    missilesCopy.Remove(i.Key);
                                    Score += 1;

                                }
                            }

                            foreach (KeyValuePair<string, GameSprite> pair in aliens)
                            {
                                if (pair.Key != name)
                                {

                                    if (CircleCollision(sprite,pair.Value))
                                    {
                                        
                                        SpritesCopy.Remove(pair.Key);
                                        movingSpritesCopy.Remove(pair.Key);
                                        aliensCopy.Remove(pair.Key);

                                        SpritesCopy.Remove(name);
                                        movingSpritesCopy.Remove(name);
                                        aliensCopy.Remove(name);
                                        Score += 2;

                                        //if (spritescopy.containskey(pair.key))
                                        //{
                                        //    spritescopy.remove(pair.key);
                                        //    movingspritescopy.remove(pair.key);
                                        //    alienscopy.remove(pair.key);
                                        //}


                                        //if (spritescopy.containskey(name))
                                        //{
                                        //    spritescopy.remove(name);
                                        //    movingspritescopy.remove(name);
                                        //    alienscopy.remove(name);
                                        //}
                                    }
                                }
                            }

                        }


                    }

                    aliens = new Dictionary<string, GameSprite>(aliensCopy);
                    movingSprites =new Dictionary<string, MovingSprite>(movingSpritesCopy);
                    Sprites = new Dictionary<string, GameSprite>(SpritesCopy);
                    missiles = new Dictionary<string, GameSprite>(missilesCopy);
                   
                    break;

                case GameState.GameEnd:

                    //Replay game is spacebar is pressed
                    KeyboardState state2 = Keyboard.GetState();
                    if (state2.IsKeyDown(Keys.Space))
                    {
                        gameState = GameState.Gameplay;
                        ResetGame(graphicsDevice);
                    }
                    break;
            }
        }

        public void ResetGame(GraphicsDevice graphicsDevice)
        {
            Sprites.Clear();
            movingSprites.Clear();
            aliens.Clear();
            stillObjects.Clear();
            missiles.Clear();

            ship = new Ship(graphicsDevice,"spaceShip.png",0.6f);
            GravObject planet= new GravObject(graphicsDevice, "planet.png", 0.17f, 6000f, 0.9f, movingSprites);

            int numberOfAsteroids = random.Next(4, 9);
            for (int i = 0; i < numberOfAsteroids; i++)
            {
                string asteroidName = "asteroid" + i;
                Asteroid asteroid = new Asteroid(graphicsDevice, "planet.png", 0.05f, 0.9f);
                AddtoSprites(asteroidName, asteroid);
                AddtoStillObjects(asteroidName, asteroid);
            }

            AddtoSprites("ship", ship);
            AddtoSprites("planet", planet);

            AddtoMovingSprites("ship", ship);
            AddtoStillObjects("planet", planet);
            ResetAllSprites();

            numberOfAliens = 0;
            numberOfMissiles = 0;
            prevFireTime = 0;
            Score = 0;
            gameState = GameManager.GameState.Titlescreen;

            SetGameTimeToZero();
        }
        public void DrawGame(GameState GameState,SpriteBatch spriteBatch)
        {
            switch(GameState)
            {
                case GameState.Titlescreen:
                    spriteBatch.Begin();

                    spriteBatch.DrawString(titleFont, "Space shooter game in Space!", new Vector2(540, myLibrary.screenHeight / 2), Color.Red);
                    spriteBatch.DrawString(titleFont, "Press space to start", new Vector2(540, myLibrary.screenHeight / 3*2), Color.Red);

                    spriteBatch.End();
                    break;

                case GameState.Gameplay:
                    spriteBatch.Begin();

                    //Draw sprites
                    DrawAllSprites(spriteBatch);
                    string ScoreString = "Score: " + Score;
                    
                    spriteBatch.DrawString(titleFont, ScoreString, new Vector2(0.05f * myLibrary.screenWidth, 0.92f * myLibrary.screenHeight), Color.Red);
                    spriteBatch.End();
                    break;

                case GameState.GameEnd:
                    spriteBatch.Begin();

                    spriteBatch.DrawString(titleFont, "Game Over!", new Vector2(540, myLibrary.screenHeight / 2), Color.Red);
                    spriteBatch.DrawString(titleFont, "Press space to play again", new Vector2(540, myLibrary.screenHeight*2 / 3), Color.Red);

                    spriteBatch.End();
                    break;
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



        /// <summary>
        /// Determines whether two sprites have collided, assumes both are circular
        /// </summary>
        /// <param name="sprite1"></param>
        /// <param name="sprite2"></param>
        /// <returns>Boolean stating whether a collision has taken place</returns>
        public bool CircleCollision(GameSprite sprite1, GameSprite sprite2)
        {
            Vector2 sprite1Pos = new Vector2(sprite1.x, sprite1.y);
            Vector2 sprite2Pos = new Vector2(sprite2.x, sprite2.y);

            Vector2 seperationVector = sprite1Pos - sprite2Pos;

            if(seperationVector.Length()<(sprite1.hitRadius+sprite2.hitRadius))
            {
                return true;
            }

            else
            {
                return false;
            }
        }
    }
}
