using System;
using System.Collections.Generic;
using System.Numerics;

namespace Game10003
{
    public class Game
    {
        // Variables
        float radius = 25;
        float speed = 100;
        Vector2 playerPosition = new Vector2(300, 550);
        Vector2 input = Vector2.Zero;

        // Bullet variables
        bool bulletFired = false;
        Vector2 bulletPosition;
        float bulletSpeed = 200;

        // Pillar info (static pillars)
        Vector2[] pillarPositions = new Vector2[]
        {
            new Vector2(400, 450), // 1st pillar
            new Vector2(230, 450), // 2nd pillar
            new Vector2(600, 450), // 3rd pillar
            new Vector2(50, 450)   // 4th pillar
        };
        float pillarSize = 70; // Each pillar is a 70x70 square

        // Moving target info
        List<Vector2> targetPositions = new List<Vector2>()
        {
            new Vector2(0, 300), // First moving target
            new Vector2(0, 200), // Second moving target
            new Vector2(0, 100)  // Third moving target
        };
        bool[] targetActive = { true, true, true }; // Track active state of each target

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
            void drawTargets()
            {
                Draw.FillColor = Color.White;
                Draw.LineColor = Color.Black;

                for (int i = 0; i < targetPositions.Count; i++)
                {
                    // Update target position with wrap-around
                    targetPositions[i] = new Vector2(targetPositions[i].X + Time.DeltaTime * speed, targetPositions[i].Y);

                    // Wrap around if the target moves off the screen horizontally
                    if (targetPositions[i].X > Window.Width + radius)
                    {
                        targetPositions[i] = new Vector2(-radius, targetPositions[i].Y);
                    }

                    // Draw target only if it is active
                    if (targetActive[i])
                    {
                        Draw.Circle(targetPositions[i].X, targetPositions[i].Y, radius);
                    }
                }
            }

            drawTargets();

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
            if (playerPosition.X > Window.Width - 30) // Adjust for player width (30 pixels)
                playerPosition.X = Window.Width - 30;

            // Draw player (square)
            void drawPlayer()
            {
                Draw.FillColor = Color.Black;
                Draw.Square(playerPosition, 30);
            }

            drawPlayer();

            // Draw pillars (static)
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

                    // Check collision with each moving target
                    for (int i = 0; i < targetPositions.Count; i++)
                    {
                        if (targetActive[i] && CheckCollision(bulletPosition, targetPositions[i], radius))
                        {
                            bulletFired = false; // Reset bullet if it collides with a target
                            targetActive[i] = false; // Deactivate target if hit
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

        // Check collision between bullet and a circular target
        private bool CheckCollision(Vector2 bulletPosition, Vector2 targetPosition, float targetRadius)
        {
            return Vector2.Distance(bulletPosition, targetPosition) <= targetRadius;
        }
    }
}
