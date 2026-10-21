using CatoriUCLibrary.Views.SpaceInvaders.GameObjects;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CatoriUCLibrary.Views
{
    public sealed class SpaceInvadersGameSurface : FrameworkElement
    {
        private static readonly Brush Green = MakeBrush("#80F9B5");
        private static readonly Brush Cyan = MakeBrush("#77DBF7");
        private static readonly Brush Purple = MakeBrush("#C5A1FF");
        private static readonly Brush Red = MakeBrush("#FF7691");
        private static readonly Brush Muted = MakeBrush("#7F91AC");
        private static readonly Brush Background = MakeBrush("#040811");
        private static readonly Brush Damaged = MakeBrush("#306B53");
        private static readonly Brush Overlay = MakeBrush("#EB040811");
        private static readonly Brush Star = MakeBrush("#273D56");
        private static readonly Typeface Font = new("Consolas");
        private static readonly string[] Squid = ["00011000", "00111100", "01111110", "11011011", "11111111", "00100100", "01011010", "10100101"];
        private static readonly string[] Crab = ["00100000100", "00010001000", "00111111100", "01101110110", "11111111111", "10111111101", "10100000101", "00011011000"];
        private static readonly string[] Octopus = ["00011111000", "01111111110", "11111111111", "11100100111", "11111111111", "00011011000", "00110101100", "11000000011"];
        private static readonly string[] Cannon = ["0000001000000", "0000011100000", "0000011100000", "0111111111110", "1111111111111", "1111111111111", "1111111111111", "1111111111111"];
        private static readonly string[] Saucer = ["00001111110000", "00111111111100", "01111111111110", "11011011011011", "11111111111111", "00110000001100"];

        public SpaceInvadersGameEngine? Engine { get; set; }

        protected override void OnRender(DrawingContext dc)
        {
            dc.DrawRectangle(Background, null, new Rect(0, 0, 900, 690));
            var game = Engine;
            if (game is null)
            {
                return;
            }
            Text(dc, "SCORE", 20, 8, 13, Muted);
            Text(dc, game.Score.ToString("D6"), 20, 27, 25, Green);
            Text(dc, "SESSION BEST", 365, 8, 13, Muted);
            Text(dc, game.BestScore.ToString("D6"), 393, 27, 25, Cyan);
            Text(dc, $"WAVE {game.Wave:D2}    LIVES {game.Lives}", 653, 25, 19, Green);
            dc.DrawLine(new Pen(Star, 1), new Point(20, 62), new Point(880, 62));
            dc.PushTransform(new TranslateTransform(0, 70));
            for (int i = 0; i < 65; i++)
            {
                dc.DrawRectangle(Star, null, new Rect((i * 137 + 31) % 900, (i * 97 + 23) % 555, 1.5, 1.5));
            }
            bool alternate = (int)(game.Time * 3) % 2 == 1;
            foreach (var alien in game.Aliens)
            {
                Sprite(dc, alien.Type == 3 ? Squid : alien.Type == 2 ? Crab : Octopus,
                    alien.X, alien.Y, alien.Width, alien.Height, alien.Type == 3 ? Purple : alien.Type == 2 ? Cyan : Green, alternate);
            }
            foreach (var block in game.Shields)
            {
                dc.DrawRectangle(block.Health == 2 ? Green : Damaged, null, new Rect(block.X, block.Y, block.Width - 0.5, block.Height - 0.5));
            }
            foreach (var shot in game.Shots)
            {
                dc.DrawRectangle(shot.Type == 0 ? Cyan : Red, null, new Rect(shot.X, shot.Y, shot.Width, shot.Height));
            }
            if (game.BonusShip is { } bonus)
            {
                Sprite(dc, Saucer, bonus.X, bonus.Y, bonus.Width, bonus.Height, Red);
            }
            if (game.State != SpaceInvadersGameState.Ready && (game.InvulnerableTime <= 0 || (int)(game.Time * 10) % 2 == 0))
            {
                Sprite(dc, Cannon, game.PlayerX, SpaceInvadersGameEngine.PlayerY, SpaceInvadersGameEngine.PlayerWidth, SpaceInvadersGameEngine.PlayerHeight, Green);
            }
            dc.DrawLine(new Pen(Green, 2), new Point(18, 602), new Point(882, 602));
            if (game.State != SpaceInvadersGameState.Playing)
            {
                dc.DrawRoundedRectangle(Overlay, new Pen(Star, 1), new Rect(155, 137, 590, 264), 12, 12);
                string title = game.State switch
                {
                    SpaceInvadersGameState.Ready => "DEFEND THE EARTH",
                    SpaceInvadersGameState.Paused => "PAUSED",
                    SpaceInvadersGameState.WaveCleared => "WAVE CLEARED",
                    _ => "GAME OVER"
                };
                CenterText(dc, title, 173, 36, Green);
                string subtitle = game.State switch
                {
                    SpaceInvadersGameState.Ready => "55 INVADERS. ONE CANNON. HOLD THE LINE.",
                    SpaceInvadersGameState.Paused => "Press P or Escape to resume",
                    SpaceInvadersGameState.WaveCleared => $"Wave {game.Wave + 1} approaching…",
                    _ => $"FINAL SCORE  {game.Score:D6}"
                };
                CenterText(dc, subtitle, 233, 17, Cyan);
                if (game.State == SpaceInvadersGameState.Ready)
                {
                    CenterText(dc, "Move: ← → / A D     Fire: hold SPACE", 275, 16, Muted);
                    CenterText(dc, "ENTER OR NEW GAME TO START", 327, 20, Green);
                }
                else if (game.State == SpaceInvadersGameState.GameOver)
                {
                    CenterText(dc, "PRESS ENTER TO TRY AGAIN", 320, 20, Green);
                }
                else
                {
                    CenterText(dc, "BONUS LIFE EVERY 1,500 POINTS", 320, 15, Muted);
                }
            }
            dc.Pop();
        }

        private static void Sprite(DrawingContext dc, string[] pixels, double x, double y, double width, double height, Brush brush, bool alternate = false)
        {
            double cellWidth = width / pixels[0].Length;
            double cellHeight = height / pixels.Length;
            for (int row = 0; row < pixels.Length; row++)
            {
                for (int column = 0; column < pixels[row].Length; column++)
                {
                    int source = alternate && row >= pixels.Length - 2 ? pixels[row].Length - 1 - column : column;
                    if (pixels[row][source] == '1')
                    {
                        dc.DrawRectangle(brush, null, new Rect(Math.Round(x + column * cellWidth), Math.Round(y + row * cellHeight), Math.Ceiling(cellWidth), Math.Ceiling(cellHeight)));
                    }
                }
            }
        }

        private void Text(DrawingContext dc, string value, double x, double y, double size, Brush brush)
        {
            dc.DrawText(new FormattedText(value, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Font, size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip), new Point(x, y));
        }

        private void CenterText(DrawingContext dc, string value, double y, double size, Brush brush)
        {
            var text = new FormattedText(value, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, Font, size, brush, VisualTreeHelper.GetDpi(this).PixelsPerDip);
            dc.DrawText(text, new Point((900 - text.Width) / 2, y));
        }

        private static Brush MakeBrush(string color)
        {
            var brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
            brush.Freeze();
            return brush;
        }
    }
}
