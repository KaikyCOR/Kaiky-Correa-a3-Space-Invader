using System;
using System.Numerics;

namespace Game10003
{
    public class Game
    {
        // Variables
        float radius = 25;
        float speed = 100;
        float x;
        Vector2 position = new Vector2(300, 550);
        Vector2 playerPosition = new Vector2(300, 550);
        Vector2 input = Vector2.Zero;

        // Bullet variables
        bool bulletFired = false;
        Vector2 bulletPosition;
        float bulletSpeed = 200;

        // Pillar info
        Vector2[] pillarPositions = new Vector2[]
        {
            new Vector2(400, 450), // 1st pillar
            new Vector2(230, 450), // 2nd pillar
            new Vector2(600, 450), // 3rd pillar
            new Vector2(50, 450)   // 4th pillar
        };
        float pillarSize = 70; // Each pillar is a 70x70 square

        public void Setup()
        {
            Window.SetTitle("--");
            Window.SetSize(800, 600);
            Window.TargetFPS = 30;
        }

        public void Update()
        {
            Window.ClearBackground(Color.OffWhite);

            // Draw moving target circles
            void drawTarget()
            {
                Draw.FillColor = Color.White;
                Draw.LineColor = Color.Black;
                x += Time.DeltaTime * speed;
                Draw.Circle(x, Window.Height / 2, radius);
                Draw.Circle(x, Window.Height / 3, radius);
                Draw.Circle(x, Window.Height / 5, radius);
            }

            drawTarget();

            // Player movement
            if (Input.IsKeyboardKeyDown(KeyboardInput.Right))
                input.X += 1;
            if (Input.IsKeyboardKeyDown(KeyboardInput.Left))
                input.X -= 1;
            if (Input.IsKeyboardKeyPressed(KeyboardInput.Space))
                input.X = 0;

            playerPosition += input * 50 * Time.DeltaTime;

            // Boundary checks for player movement
            if (playerPosition.X < 0)
                playerPosition.X = 0;
            if (playerPosition.X > Window.Width)
                playerPosition.X = Window.Width;

            // Draw player (square)
            void drawPlayer()
            {
                Draw.FillColor = Color.Black;
                Draw.Square(playerPosition, 30);
            }

            drawPlayer();

            // Draw pillars
            void drawPillars()
            {
                Draw.LineColor = Color.Black;
                Draw.FillColor = Color.White;
                foreach (var pillarPosition in pillarPositions)
                {
                    Draw.Square(pillarPosition, pillarSize);
                }
            }

            drawPillars();

            // Bullet movement and collision logic
            void PewPew()
            {
                // Start bullet from player's position if Z or J key is pressed
                if ((Input.IsKeyboardKeyPressed(KeyboardInput.Z) || Input.IsKeyboardKeyPressed(KeyboardInput.J)) && !bulletFired)
                {
                    bulletPosition = new Vector2(playerPosition.X, playerPosition.Y - 15); // Start bullet at player's top-center
                    bulletFired = true;
                }

                // Move bullet if fired
                if (bulletFired)
                {
                    bulletPosition.Y -= bulletSpeed * Time.DeltaTime;

                    // Check collision with each pillar
                    foreach (var pillarPosition in pillarPositions)
                    {
                        if (CheckCollision(bulletPosition, pillarPosition, pillarSize))
                        {
                            bulletFired = false; // Reset bullet if it collides with a pillar
                            break;
                        }
                    }

                    // Draw bullet if it hasn't been reset
                    if (bulletFired)
                    {
                        Draw.LineSize = 3;
                        Draw.Line(bulletPosition, new Vector2(bulletPosition.X, bulletPosition.Y - 15));
                    }

                    // Reset bullet if it goes off-screen
                    if (bulletPosition.Y < 0)
                    {
                        bulletFired = false;
                    }
                }
            }

            PewPew();
        }

        // Check collision between bullet and a square pillar
        private bool CheckCollision(Vector2 bulletPosition, Vector2 pillarPosition, float pillarSize)
        {
            return bulletPosition.X >= pillarPosition.X - pillarSize / 2 &&
                   bulletPosition.X <= pillarPosition.X + pillarSize / 2 &&
                   bulletPosition.Y >= pillarPosition.Y - pillarSize / 2 &&
                   bulletPosition.Y <= pillarPosition.Y + pillarSize / 2;
        }
    }
}
