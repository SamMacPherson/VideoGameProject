using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace My_Library
{
    class MyLibrary
    {
        public int screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
        public int screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

        Random random = new Random();

        




        /// <summary>
        /// Finds the normalised x and y components of a vector. Inputs are the x andy co-ordinates of a vector. This function passes by reference.
        /// </summary>
        public void Norm(ref float x, ref float y)
        {

            //Find length of vector using pythagoras
            double squaredTotal = x * x + y * y;
            double length = Math.Sqrt(squaredTotal);

            //Normalise the x and y co-ordinates
            x = x / Convert.ToSingle(length);
            y = y / Convert.ToSingle(length);


        }
        /// <summary>
        /// Finds the angle that a sprite needs to point in
        /// </summary>
        public float VectorToAngle(Vector2 vector)
        {
            if (vector.X < 0)
            {
                return (float)Math.Atan(vector.Y / vector.X) - (float)Math.PI / 2;
            }
            else
            {
                return (float)Math.Atan(vector.Y / vector.X) + (float)Math.PI / 2;
            }
        }

        /// <summary>
        /// Finds the angle that a sprite needs to point in
        /// </summary>
        public float VectorToAngle(float x, float y)
        {
            if (x < 0)
            {
                return (float)Math.Atan(y / x) - (float)Math.PI / 2;
            }
            else
            {
                return (float)Math.Atan(y / x) + (float)Math.PI / 2;
            }
            
        }


        /// <summary>
        /// Constructs a unit vector from an angle, pass the vector by reference
        /// </summary>
        public void AngletoVector(float angle,ref Vector2 vector)
        {
            vector.X = (float)Math.Sin(angle);
            vector.Y = -(float)Math.Cos(angle);
        }

        public void RanScreenPos(ref float x, ref float y)
        {
            //Randomly place the GravObject
            x = Convert.ToSingle(random.NextDouble()) * screenWidth;
            y = Convert.ToSingle(random.NextDouble()) * screenHeight;
        }

        public void RanScreenPos(ref float x, ref float y, float leftBound, float rightBound, float bottomBound, float topBound)
        {
            //Randomly place the GravObject
            //x = Convert.ToSingle(random.NextDouble()) * screenWidth;
            //y = Convert.ToSingle(random.NextDouble()) * screenHeight;
            RandomRange(leftBound * screenWidth, rightBound * screenWidth, ref x);
            RandomRange(topBound * screenHeight, bottomBound * screenHeight, ref y);

        }

        public void RandomRange(float bottom, float top, ref  float ranNum)
        {
            bool done = false;
            
            while(!done)
            {
                float num = Convert.ToSingle(random.NextDouble()) * top;
                if (num>bottom)
                {
                    done = true;
                    ranNum = num;
                    
                }
            }
            
        }

        public void TorusWorld(ref float x,ref float y,float height, float width)
        {
            //If ship goes past right edge of screen
            if (x - width/2 > screenWidth)
            {
                x = -width/2;
            }

            //If ship goes past left edge of screen
            if (x + width/2 < 0)
            {
                x = screenWidth + width/2;
            }

            //If ship goes past top edge of screen
            if (y + height/2 < 0)
            {
                y = screenHeight + height/2;
            }

            //If ship goes past bottom edge of screen
            if (y - height/2 > screenHeight)
            {
                y = -height/2;
            }
        }
    }      
}
