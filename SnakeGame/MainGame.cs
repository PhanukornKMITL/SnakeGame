using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace SnakeGame
{
   
    public class MainGame : Game
    {
        public const int WIDTH = 800;
        public const int HEIGHT = 600;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        List<Vector2> _snakePelletsPos; //เก็บ position ของจุดงู
        Texture2D _pellet;
        float _moveSpeed;
        float _tick;
        SpriteFont _font;
        int _score;
        enum GameState
        {
            PLAYING,GAMEOVER,PAUSE
        }
        enum Direction
        {
            LEFT,RIGHT,UP,DOWN
        };
        Vector2 _foodPos,_textPos;

        Direction _currentDirectrion;
        GameState _currentState;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
           
        }

       //ถูกอรียกเมื่อ ตัวนี้ถูกสร้าง เรียกหลัง constructor  สร้าง obj
        protected override void Initialize()
        {
            
            graphics.PreferredBackBufferWidth = WIDTH;
            graphics.PreferredBackBufferHeight = HEIGHT;
            graphics.ApplyChanges(); // ใส่เพื่อยืนยันค่า
            _score = 0;
            _currentState = GameState.PLAYING;

            _snakePelletsPos = new List<Vector2>();
            _textPos = new Vector2(50,500);
            base.Initialize();
        }

        //การโหลดภาพ เรียกหลังจาก Intitialize
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //โหลดรูปมาเก็บไว้ในตัวแปร
            _pellet = this.Content.Load<Texture2D>("Pellet");
            _font = this.Content.Load<SpriteFont>("gamefont");

            //ทำให้งูอยู่ตรงกลาง 10 คือขนาดของจุดงู i แรกเป็น 10 จึงไม่ลบ
            for(int i = 0; i < 5; i++)
            {
                _snakePelletsPos.Add(new Vector2(MainGame.WIDTH  / 2 - 10 * i, MainGame.HEIGHT  / 2));

            }
            _moveSpeed = 5;
            _tick = 0;
            _currentDirectrion = Direction.RIGHT; //setทิศทางเริ่มต้นของงู
            _foodPos = GetNewFoodPos();
            

        }

        //ถูกเรียกเมื่อเกมถูกปิด จะปล่อยค่าใน memory อออก
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        
        //ถูกเรียก 1 ครั้งต่อ 1 เฟรม
        protected override void Update(GameTime gameTime)
        {

                
            switch (_currentState)
            {
                case GameState.PLAYING:
                    checkPress();

                    //Update tick elaspgametime คือ เวลาที่ผ่านมาตั้งแต่ลูปที่แล้งถึงลูปนี้
                    _tick += gameTime.ElapsedGameTime.Ticks / (float)TimeSpan.TicksPerSecond; // ทำให้เป็นวินาที

                    if (_tick >= 1 / _moveSpeed)
                    {
                        //set ค่า tick = 0 ทุกครั้งที่มีการอัทเดท
                        _tick = 0;
                        //add head      ตำแหน่งปัจจุบันคือ 0 ในตำแหน่งX + 10 
                        switch (_currentDirectrion)
                        {
                            case Direction.LEFT:
                                _snakePelletsPos.Insert(0, new Vector2(_snakePelletsPos[0].X - 10, _snakePelletsPos[0].Y));
                                break;
                            case Direction.RIGHT:
                                _snakePelletsPos.Insert(0, new Vector2(_snakePelletsPos[0].X + 10, _snakePelletsPos[0].Y));
                                break;
                            case Direction.UP:
                                _snakePelletsPos.Insert(0, new Vector2(_snakePelletsPos[0].X, _snakePelletsPos[0].Y - 10));
                                break;
                            case Direction.DOWN:
                                _snakePelletsPos.Insert(0, new Vector2(_snakePelletsPos[0].X, _snakePelletsPos[0].Y + 10));
                                break;

                        }
                        //check eat food
                        if (_snakePelletsPos[0].Equals(_foodPos))
                        {
                            //After ate then random new food
                            _foodPos = GetNewFoodPos();
                            _score++;
                            _moveSpeed *= 1.1f;
                            //กินแล้วไม่ต้องลบหาง
                        }
                        else
                        {
                            //remove tail
                            _snakePelletsPos.RemoveAt(_snakePelletsPos.Count - 1);
                        }

                        checkGameOver();


                    }


                    break;
                case GameState.PAUSE:
                    checkPress();

                    break;
                case GameState.GAMEOVER:
                    checkPress();
                    break;
            }
            base.Update(gameTime);
        }

        // เอาไว้วาดหลังจาก อัพเดทรูปทุกรูปเสร็จแล้ว
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DimGray);
            spriteBatch.Begin();

            // TODO: Add your drawing code here
            foreach (Vector2 sp in _snakePelletsPos)
            {
               
                spriteBatch.Draw(_pellet,sp,Color.White);
            }
            spriteBatch.Draw(_pellet,_foodPos,Color.Yellow);
            spriteBatch.DrawString(_font, "SCORE: "+_score,_textPos,Color.Red);
            if (_currentState == GameState.GAMEOVER)
            {
                spriteBatch.DrawString(_font, "GAMEOVER", new Vector2(350, 300), Color.Red);
                spriteBatch.DrawString(_font, "Press Space To Restart", new Vector2(310, 330), Color.Red);
            }

            if(_currentState == GameState.PAUSE)
            {
                spriteBatch.DrawString(_font, "GAME PAUSE", new Vector2(350, 300), Color.Red);
            }

            spriteBatch.End();
            graphics.BeginDraw();

            base.Draw(gameTime);
        }
        protected Vector2 GetNewFoodPos()
        {
            Vector2 returnPoint = new Vector2();
            Random rand = new Random();
            returnPoint.X = (int) rand.Next(WIDTH / 10) * 10; //random 0 ถึง ที่กำหนดไว้
            returnPoint.Y = (int)rand.Next(HEIGHT / 10) * 10;

            //Get New Food if the food has been generate on snake Position
            for(int i = 0; i < _snakePelletsPos.Count; i++)
            {
                if(returnPoint == _snakePelletsPos[i])
                {
                    GetNewFoodPos();
                }
            }

            return returnPoint;
        }

        public void checkPress()
        {

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                if(_currentState == GameState.PLAYING)
                {
                    _currentState = GameState.PAUSE;
                }
                else
                    _currentState = GameState.PLAYING;

                //Exit();
            }
                

            if(_currentState == GameState.GAMEOVER)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space))
                {
                    _currentState = GameState.PLAYING;
                    Initialize();
                   
                }
            }


            //process input
            if (Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                switch (_currentDirectrion)
                {
                    case Direction.LEFT:
                        break;
                    case Direction.RIGHT:
                        break;
                    case Direction.UP:
                        _currentDirectrion = Direction.LEFT;
                        break;
                    case Direction.DOWN:
                        _currentDirectrion = Direction.LEFT;
                        break;
                }
            }
            if (Keyboard.GetState().IsKeyDown(Keys.Right))
            {
                switch (_currentDirectrion)
                {
                    case Direction.LEFT:

                        break;
                    case Direction.RIGHT:

                        break;
                    case Direction.UP:
                        _currentDirectrion = Direction.RIGHT;
                        break;
                    case Direction.DOWN:
                        _currentDirectrion = Direction.RIGHT;
                        break;
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Up))
            {
                switch (_currentDirectrion)
                {
                    case Direction.LEFT:
                        _currentDirectrion = Direction.UP;
                        break;
                    case Direction.RIGHT:
                        _currentDirectrion = Direction.UP;
                        break;
                    case Direction.UP:
                        break;
                    case Direction.DOWN:
                        break;
                }

            }
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                switch (_currentDirectrion)
                {
                    case Direction.LEFT:
                        _currentDirectrion = Direction.DOWN;
                        break;
                    case Direction.RIGHT:
                        _currentDirectrion = Direction.DOWN;
                        break;
                    case Direction.UP:
                        break;
                    case Direction.DOWN:
                        break;
                }

            }
        }

        public void checkGameOver()
        {

            //check if snake collise border of screen then GAMEOVER
            if (_snakePelletsPos[0].X == 0 || _snakePelletsPos[0].Y == 0 || _snakePelletsPos[0].X == 800 || _snakePelletsPos[0].Y == 600)
            {
                _currentState = GameState.GAMEOVER;

            }

            //Check if snake eat itself
            for (int i = 1; i < _snakePelletsPos.Count; i++)
            {
                if (_snakePelletsPos[0] == _snakePelletsPos[i])
                {
                    _currentState = GameState.GAMEOVER;
                }
            }
        }

    }

}
