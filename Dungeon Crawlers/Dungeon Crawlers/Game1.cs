﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace Dungeon_Crawlers
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont titleFont;
        Vector2 titlePosition;
        int screenWidth;
        int screenHeight;
        Player player;                  // The player object for the main character
        Hero hero;                      // The hero object for the enemy of the game
        Texture2D charTextures;         // The textures for the hero
        Texture2D squareObject;         // use only for debug
        Enemy enemy;                    // The ally object for the other monsters who help you
        Texture2D goblinTextures;       // The textures for the goblin
        Texture2D slimeTextures;        // The textures for the slime
        Texture2D wizardTextures;       // The textures for the wizard
        Texture2D tileTextures;         // The textures for the level tiles
        KeyboardState kbState;          // Tracks the current state of the keyboard
        KeyboardState prevKbState;      // Tracks the state of the keyboard from the last frame
        MouseState mState;              // use only for debug
        MouseState prevmsState;         // use only for debug
        List<Hitbox> hitBoxes;
        Tile tile1;
        Tile tile2;

        List<Item> squareCollection = new List<Item>();
        
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            screenWidth = graphics.GraphicsDevice.Viewport.Width;
            screenHeight = graphics.GraphicsDevice.Viewport.Height;

            //Sets up the positions for text
            titlePosition = new Vector2((screenWidth / 2), (screenHeight / 2));
            IsMouseVisible = true;

            // Initializes collection of hitboxes
            hitBoxes = new List<Hitbox>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //Loads the title text
            titleFont = Content.Load<SpriteFont>("fonts/titleFont");

            //Loads the hero and his textures
            charTextures = Content.Load<Texture2D>("Character-Spritesheet");
            Hitbox heroBox = new Hitbox(new Rectangle(0,0,96,96),BoxType.Hurtbox); //96x96 size because 2x scaleing, will change to 1 time (48x48) after debug
            hero = new Hero(charTextures, heroBox, screenWidth, screenHeight);

            Hitbox playerBox = new Hitbox(new Rectangle(700, 200, 36*2, 45*2), BoxType.Hitbox);
            player = new Player(charTextures, playerBox, screenWidth, screenHeight);

            squareObject = Content.Load<Texture2D>("Square");

            tileTextures = Content.Load<Texture2D>("Tile_Spritesheet");
            Hitbox tileBox1 = new Hitbox(new Rectangle (700, 400, 64, 64), BoxType.Collision);
            tile1 = new Tile(tileTextures, tileBox1, TileType.Floor);
            hitBoxes.Add(tileBox1);
            Hitbox tileBox2 = new Hitbox(new Rectangle(700, 336, 64, 64), BoxType.Collision);
            tile2 = new Tile(tileTextures, tileBox2, TileType.Floor);
            hitBoxes.Add(tileBox2);

            Hitbox enemyBox = new Hitbox(new Rectangle(800, 200, 36 * 2, 45 * 2), BoxType.Hitbox);
            enemy = new Enemy(charTextures, enemyBox, screenWidth, screenHeight);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here'
            hero.UpdateAnimation(gameTime);
            player.UpdateAnimation(gameTime);
            player.CheckCollision(hitBoxes);
            enemy.UpdateAnimation(gameTime);
            enemy.CheckCollision(hitBoxes);
            
            //Gets the current keyboard state
            kbState = Keyboard.GetState();
            mState = Mouse.GetState();
            hero.logic(player, hitBoxes);

            if (mState.LeftButton == ButtonState.Pressed && prevmsState.LeftButton == ButtonState.Released)
            {
                Rectangle mousLoc = new Rectangle(mState.X, mState.Y, 40, 40);
                Hitbox squareBox = new Hitbox(mousLoc, BoxType.Collision);
                Item square = new Item(squareObject, squareBox, screenWidth, screenHeight);

                squareCollection.Add(square);
                hitBoxes.Add(squareBox);
                
            }
            prevmsState = mState;

            
            //Checks the state and updates accordingly
            switch (StateManager.Instance.CurrentState)
            {
                //Title updates
                case GameState.Title:
                    //If enter is pressed
                    if (kbState.IsKeyDown(Keys.Enter) && kbState != prevKbState)
                    {
                        //Start the game
                        StateManager.Instance.ChangeState(GameState.Game);
                    }
                    //If I is pressed
                    if (kbState.IsKeyDown(Keys.I) && kbState != prevKbState)
                    {
                        //Show the instructions
                        StateManager.Instance.ChangeState(GameState.Instructions);
                    }
                    
                    break;

                //Instructions updates
                case GameState.Instructions:
                    //If enter is pressed
                    if (kbState.IsKeyDown(Keys.Enter) && kbState != prevKbState)
                    {
                        //Start the game
                        StateManager.Instance.ChangeState(GameState.Game);
                    }
                    //If M is pressed
                    if (kbState.IsKeyDown(Keys.M) && kbState != prevKbState)
                    {
                        //Go back to the main menu
                        StateManager.Instance.ChangeState(GameState.Title);
                    }
                    break;

                //Game updates
                case GameState.Game:
                    //If esc is pressed
                    if (kbState.IsKeyDown(Keys.P) && kbState != prevKbState)
                    {
                        //Pause the game
                        StateManager.Instance.ChangeState(GameState.Pause);
                    }
                    player.Update(gameTime);
                    enemy.Update(gameTime);
                    break;

                //Pause updates
                case GameState.Pause:
                    //If R is pressed
                    if (kbState.IsKeyDown(Keys.R) && kbState != prevKbState)
                    {
                        //Unpause the game
                        StateManager.Instance.ChangeState(GameState.Game);
                    }
                    //If H is pressed
                    if (kbState.IsKeyDown(Keys.H) && kbState != prevKbState)
                    {
                        //Display the help screen
                        StateManager.Instance.ChangeState(GameState.Help);
                    }
                    //If M is pressed
                    if (kbState.IsKeyDown(Keys.M) && kbState != prevKbState)
                    {
                        //Display the help screen
                        StateManager.Instance.ChangeState(GameState.Title);
                    }
                    break;

                //Help updates
                case GameState.Help:
                    //If P is pressed
                    if (kbState.IsKeyDown(Keys.P) && kbState != prevKbState)
                    {
                        //Go back to the pause menu
                        StateManager.Instance.ChangeState(GameState.Pause);
                    }
                    //If P is pressed
                    if (kbState.IsKeyDown(Keys.R) && kbState != prevKbState)
                    {
                        //Go back to the pause menu
                        StateManager.Instance.ChangeState(GameState.Game);
                    }
                    break;

                //Game over updates
                case GameState.GameOver:
                    //If enter is pressed
                    if (kbState.IsKeyDown(Keys.Enter) && kbState != prevKbState)
                    {
                        //Return to the title screen
                        StateManager.Instance.ChangeState(GameState.Title);
                    }
                    break;

                //Win updates
                case GameState.Win:
                    break;
            }

            //Sets the previous keyboard state to the current keyboard state now that the frame is done
            prevKbState = kbState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            // TODO: Add your drawing code here
            spriteBatch.Begin();
            //Checks the state and draws accordingly


            switch (StateManager.Instance.CurrentState)
            {
                case GameState.Title:
                    //Draws the title text
                    spriteBatch.DrawString(titleFont, "Dungeon Crawlers", titlePosition, Color.OrangeRed);

                    //Draws the instructions for starting the game
                    spriteBatch.DrawString(titleFont, "Press ENTER to Start", new Vector2(500, 700), Color.OrangeRed);

                    break;


                case GameState.Instructions:
                    spriteBatch.DrawString(titleFont, "Objective: Defeat the hero by gathering as many enemies as possible and", new Vector2(500, 300), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "then fighting the hero with those enemies. The Hero is stronger so you", new Vector2(500, 325), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "can't defeat him alone!", new Vector2(500, 350), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click Enter to Continue", new Vector2(500, 800), Color.OrangeRed);
                    break;

                case GameState.Game:
                    hero.Draw(spriteBatch);
                    player.Draw(spriteBatch);
                    enemy.Draw(spriteBatch);
                    tile1.Draw(spriteBatch, 0, 0, SpriteEffects.None, 0);
                    tile2.Draw(spriteBatch, 0, 0, SpriteEffects.None, 0);
                    for (int a = 0; a < squareCollection.Count; a++)
                    {
                        spriteBatch.Draw(squareObject, squareCollection[a].Position.Box, Color.White);
                    }
                    break;

                case GameState.Pause:
                    spriteBatch.DrawString(titleFont, "Click M to Go back To Menu", new Vector2(500, 700), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click H for Help", new Vector2(500, 750), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click R to Resume", new Vector2(500, 800), Color.OrangeRed);
                    break;

                case GameState.Help:
                    spriteBatch.DrawString(titleFont, "Click W to Jump", new Vector2(500, 300), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click A to move left", new Vector2(500, 350), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click D to move right", new Vector2(500, 400), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click Space to attack", new Vector2(500, 450), Color.OrangeRed);

                    spriteBatch.DrawString(titleFont, "Click P to Go Back", new Vector2(500, 750), Color.OrangeRed);
                    spriteBatch.DrawString(titleFont, "Click R to Resume", new Vector2(500, 800), Color.OrangeRed);
                    break;

                case GameState.GameOver:
                    spriteBatch.DrawString(titleFont, "You have lost the game!", titlePosition, Color.OrangeRed);
                    break;

                case GameState.Win:
                    spriteBatch.DrawString(titleFont, "You have won the game!", titlePosition, Color.OrangeRed);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
