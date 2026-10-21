using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriUCLibrary.Views.SpaceInvaders.GameObjects
{
    public class SpaceInvadersGameEngine
    {
        public const double Width = 900;
        public const double Height = 620;
        public const double PlayerY = 564;
        public const double PlayerWidth = 42;
        public const double PlayerHeight = 26;
        private readonly Random random = new();
        private double direction = 1;
        private SpaceInvadersGameState resumeState;
        private double fireDelay;
        private double enemyDelay;
        private double bonusDelay;
        private double waveDelay;
        private int nextExtraLife = 1500;

        public SpaceInvadersGameState State { get; private set; } = SpaceInvadersGameState.Ready;
        public List<SpaceInvadersGamePiece> Aliens { get; } = new();
        public List<SpaceInvadersGamePiece> Shots { get; } = new();
        public List<SpaceInvadersGamePiece> Shields { get; } = new();
        public SpaceInvadersGamePiece? BonusShip { get; private set; }
        public double PlayerX { get; private set; }
        public double InvulnerableTime { get; private set; }
        public double Time { get; private set; }
        public int Score { get; private set; }
        public int BestScore { get; private set; }
        public int Lives { get; private set; }
        public int Wave { get; private set; }
        public bool Left { get; set; }
        public bool Right { get; set; }
        public bool Fire { get; set; }

        public void Start()
        {
            Score = 0;
            Lives = 3;
            Wave = 1;
            Time = 0;
            nextExtraLife = 1500;
            ClearInput();
            BeginWave();
        }

        private void BeginWave()
        {
            Aliens.Clear();
            Shots.Clear();
            Shields.Clear();
            BonusShip = null;
            PlayerX = (Width - PlayerWidth) / 2;
            InvulnerableTime = 2;
            direction = 1;
            fireDelay = 0;
            enemyDelay = 1.2;
            bonusDelay = 15 + random.NextDouble() * 8;
            for (int row = 0; row < 5; row++)
            {
                for (int column = 0; column < 11; column++)
                {
                    Aliens.Add(new SpaceInvadersGamePiece
                    {
                        X = 115 + column * 55,
                        Y = 82 + row * 45 + Math.Min(Wave - 1, 5) * 8,
                        Width = 34,
                        Height = 26,
                        Type = row == 0 ? 3 : row < 3 ? 2 : 1
                    });
                }
            }
            for (int bunker = 0; bunker < 4; bunker++)
            {
                for (int row = 0; row < 5; row++)
                {
                    for (int column = 0; column < 9; column++)
                    {
                        if ((row == 0 && (column < 2 || column > 6)) || (row >= 3 && column >= 3 && column <= 5))
                        {
                            continue;
                        }
                        Shields.Add(new SpaceInvadersGamePiece
                        {
                            X = 128 + bunker * 190 + column * 10,
                            Y = 475 + row * 10,
                            Width = 10,
                            Height = 10,
                            Health = 2
                        });
                    }
                }
            }
            State = SpaceInvadersGameState.Playing;
        }

        public void ClearInput()
        {
            Left = false;
            Right = false;
            Fire = false;
        }

        public void TogglePause()
        {
            if (State is SpaceInvadersGameState.Playing or SpaceInvadersGameState.WaveCleared)
            {
                resumeState = State;
                State = SpaceInvadersGameState.Paused;
            }
            else if (State == SpaceInvadersGameState.Paused)
            {
                State = resumeState;
            }
            ClearInput();
        }

        public void Update(double elapsed)
        {
            double dt = Math.Clamp(elapsed, 0, 0.05);
            if (State == SpaceInvadersGameState.WaveCleared)
            {
                waveDelay -= dt;
                if (waveDelay <= 0)
                {
                    Wave++;
                    BeginWave();
                }
                return;
            }
            if (State != SpaceInvadersGameState.Playing)
            {
                return;
            }
            Time += dt;
            InvulnerableTime = Math.Max(0, InvulnerableTime - dt);
            PlayerX = Math.Clamp(PlayerX + ((Right ? 1 : 0) - (Left ? 1 : 0)) * 355 * dt, 18, Width - PlayerWidth - 18);
            fireDelay -= dt;
            if (Fire && fireDelay <= 0 && Shots.Count(shot => shot.Type == 0) < 2)
            {
                Shots.Add(new SpaceInvadersGamePiece { X = PlayerX + 19, Y = PlayerY - 12, Width = 4, Height = 14, Type = 0, Velocity = -590 });
                fireDelay = 0.25;
            }
            MoveAliens(dt);
            if (State != SpaceInvadersGameState.Playing)
            {
                return;
            }
            UpdateBonus(dt);
            UpdateShots(dt);
            if (State == SpaceInvadersGameState.Playing && Aliens.Count == 0)
            {
                Shots.Clear();
                State = SpaceInvadersGameState.WaveCleared;
                waveDelay = 2;
                ClearInput();
            }
        }

        private void MoveAliens(double dt)
        {
            if (Aliens.Count == 0)
            {
                return;
            }
            double speed = 19 + Wave * 5 + (55 - Aliens.Count) * 1.7;
            double movement = speed * direction * dt;
            bool edge = Aliens.Any(alien => alien.X + movement < 18 || alien.X + alien.Width + movement > Width - 18);
            if (edge)
            {
                direction *= -1;
            }
            foreach (var alien in Aliens)
            {
                if (edge)
                {
                    alien.Y += 21;
                }
                else
                {
                    alien.X += movement;
                }
                Shields.RemoveAll(block => block.Intersects(alien.X, alien.Y, alien.Width, alien.Height));
                if (alien.Y + alien.Height >= PlayerY)
                {
                    EndGame();
                    return;
                }
            }
            enemyDelay -= dt;
            if (enemyDelay <= 0)
            {
                var candidates = Aliens.Where(alien => !Aliens.Any(other => other.Y > alien.Y && Math.Abs(other.X - alien.X) < 10)).ToList();
                var shooter = candidates[random.Next(candidates.Count)];
                Shots.Add(new SpaceInvadersGamePiece { X = shooter.X + 15, Y = shooter.Y + shooter.Height, Width = 5, Height = 16, Type = 1, Velocity = Math.Min(340, 185 + Wave * 15) });
                enemyDelay = Math.Max(0.22, 0.85 - Wave * 0.06) + random.NextDouble() * 0.35;
            }
        }

        private void UpdateBonus(double dt)
        {
            bonusDelay -= dt;
            if (BonusShip is null && bonusDelay <= 0)
            {
                BonusShip = new SpaceInvadersGamePiece { X = -55, Y = 25, Width = 48, Height = 22 };
                bonusDelay = 18 + random.NextDouble() * 10;
            }
            if (BonusShip is not null)
            {
                BonusShip.X += 115 * dt;
                if (BonusShip.X > Width)
                {
                    BonusShip = null;
                }
            }
        }

        private void UpdateShots(double dt)
        {
            foreach (var shot in Shots.ToArray())
            {
                double oldY = shot.Y;
                shot.Y += shot.Velocity * dt;
                double top = Math.Min(oldY, shot.Y);
                double height = Math.Abs(shot.Y - oldY) + shot.Height;
                var block = Shields.Where(candidate => candidate.Intersects(shot.X, top, shot.Width, height))
                    .OrderBy(candidate => shot.Type == 0 ? -candidate.Y : candidate.Y).FirstOrDefault();
                if (block is not null)
                {
                    block.Health--;
                    if (block.Health <= 0)
                    {
                        Shields.Remove(block);
                    }
                    Shots.Remove(shot);
                    continue;
                }
                if (shot.Type == 0)
                {
                    var target = Aliens.Where(alien => alien.Intersects(shot.X, top, shot.Width, height)).OrderByDescending(alien => alien.Y).FirstOrDefault();
                    if (target is not null)
                    {
                        Aliens.Remove(target);
                        AddScore(target.Type * 10);
                        Shots.Remove(shot);
                    }
                    else if (BonusShip?.Intersects(shot.X, top, shot.Width, height) == true)
                    {
                        AddScore((random.Next(3) + 1) * 100);
                        BonusShip = null;
                        Shots.Remove(shot);
                    }
                }
                else if (InvulnerableTime <= 0 && shot.X < PlayerX + PlayerWidth && shot.X + shot.Width > PlayerX
                    && top < PlayerY + PlayerHeight && top + height > PlayerY)
                {
                    Lives--;
                    Shots.Clear();
                    InvulnerableTime = 2.5;
                    PlayerX = (Width - PlayerWidth) / 2;
                    if (Lives <= 0)
                    {
                        EndGame();
                    }
                    break;
                }
                if (shot.Y < -30 || shot.Y > Height)
                {
                    Shots.Remove(shot);
                }
            }
        }

        private void AddScore(int points)
        {
            Score += points;
            BestScore = Math.Max(BestScore, Score);
            if (Score >= nextExtraLife)
            {
                Lives = Math.Min(5, Lives + 1);
                nextExtraLife += 1500;
            }
        }

        private void EndGame()
        {
            State = SpaceInvadersGameState.GameOver;
            ClearInput();
        }
    }
}
