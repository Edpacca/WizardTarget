using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace WizardGrenade
{
    class GameScreen
    {
        private Castle castle;
        private Player player;

        private SpriteFont _playerStatFont;

        private Targets _targets;
        const int NumberOfTargets = 5;
        private RoundTimer _rounds;
        private int _numberOfRounds = 3;
        private float _roundLength = 16;
        private float _countDownLength = 3;
        private bool _gameOver = false;
        private bool _allTargetsDead = false;

        private int Score = 0;

        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public void Initialize()
        {
            player = new Player(WizardTargetGame.SCREEN_WIDTH / 2, 348);
            castle = new Castle();
        }

        public void LoadContent (ContentManager contentManager)
        {
            _currentKeyboardState = Keyboard.GetState();

            player.LoadContent(contentManager);
            castle.LoadContent(contentManager);

            _targets = new Targets(NumberOfTargets);
            _targets.LoadContent(contentManager);

            _rounds = new RoundTimer(_numberOfRounds, _roundLength, _countDownLength);
            _rounds.LoadContent(contentManager);

            _playerStatFont = contentManager.Load<SpriteFont>("StatFont");
        }

        public void UnloadContent()
        {

        }

        public void Update(GameTime gameTime)
        {
            _currentKeyboardState = Keyboard.GetState();

            player.Update(gameTime);

            //if (_rounds.roundActive)
            //    player.ChargeGrenadeThrow(_currentKeyboardState, _previousKeyboardState, gameTime);

            _rounds.update(gameTime);

            if (_rounds.CurrentRound > _numberOfRounds)
            {
                _targets.KillTargets();
                _gameOver = true;
            }

            _targets.UpdateTargets(gameTime);

            if (_targets.ActiveTargets < 1)
            {
                _allTargetsDead = true;
                _rounds.newRound = true;
            }

            if (_rounds.newRound)
            {
                if (_allTargetsDead)
                    Score += 500;

                _rounds.newRound = false;
                _allTargetsDead = false;
                _targets.ActiveTargets = NumberOfTargets;
                _targets.ResetTargets();
            }

            foreach (var grenade in player._grenades)
            {
                if (_rounds.roundActive)
                {
                    if (_targets.UpdateTargetCollisions(grenade))
                    {
                        Score += (int)_rounds.TimeScoreMultiplier() * 5;
                        player.GrenadeCollisionResolution(grenade, gameTime);
                    }
                }
            }

            _previousKeyboardState = _currentKeyboardState;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            player.Draw(spriteBatch);
            castle.Draw(spriteBatch);

            spriteBatch.DrawString(_playerStatFont, "Score: " + Score, new Vector2(WizardTargetGame.SCREEN_WIDTH - 140, WizardTargetGame.SCREEN_HEIGHT - 80), Color.Turquoise);

            if (_gameOver)
                spriteBatch.DrawString(_playerStatFont, "FINAL SCORE: " + Score, new Vector2(WizardTargetGame.SCREEN_WIDTH / 2 - 120, WizardTargetGame.SCREEN_HEIGHT / 2), Color.HotPink);
            else
                _rounds.DrawTimer(spriteBatch);

            _targets.DrawTargets(spriteBatch);
        }
    }
}
