using System.Windows;

using System.Windows.Media;

namespace CatoriUCLibrary.Views.Pacman.GameObjects;

public sealed class MazeSurface : FrameworkElement
{

    public GameState? Game {
 get;
 set;
 }

    private static readonly Brush[] Colors = [Brushes.HotPink, Brushes.Cyan, Brushes.Orange, Brushes.Red];

    protected override void OnRender(DrawingContext dc)
    {

        base.OnRender(dc);

        if (Game is not GameState g) return;

        var wallPen = new Pen(new SolidColorBrush(Color.FromRgb(48,99,240)),2);

        for (int y = 0;
 y < GameState.Map.Length;
 y++) for (int x = 0;
 x < GameState.Map[y].Length;
 x++)
            if (GameState.Map[y][x] == '#') dc.DrawRoundedRectangle(new SolidColorBrush(Color.FromRgb(14,25,62)),wallPen,new Rect(x*24+2,y*24+2,20,20),5,5);

        foreach (var p in g.Pellets) dc.DrawEllipse(Brushes.PeachPuff,null,Center(p.X,p.Y),2.5,2.5);

        foreach (var p in g.Powers) dc.DrawEllipse(Brushes.White,null,Center(p.X,p.Y),6,6);

        var center = Center(g.X,g.Y);

        dc.DrawEllipse(Brushes.Gold,null,center,10,10);

        var mouth = new StreamGeometry();
 double angle = (g.Direction-1)*Math.PI/2;

        using (var context = mouth.Open())
        {

            context.BeginFigure(center,true,true);

            context.LineTo(new Point(center.X+12*Math.Cos(angle-.45),center.Y+12*Math.Sin(angle-.45)),true,false);

            context.LineTo(new Point(center.X+12*Math.Cos(angle+.45),center.Y+12*Math.Sin(angle+.45)),true,false);

        }

        dc.DrawGeometry(new SolidColorBrush(Color.FromRgb(8,11,23)),null,mouth);

        for (int i=0;
i<g.Ghosts.Count;
i++)
        {

            var p=Center(g.Ghosts[i].X,g.Ghosts[i].Y);

            dc.DrawRoundedRectangle(g.Powered?Brushes.RoyalBlue:Colors[i],null,new Rect(p.X-10,p.Y-10,20,21),8,8);

            dc.DrawEllipse(Brushes.White,null,new Point(p.X-4,p.Y-2),3,4);
 dc.DrawEllipse(Brushes.White,null,new Point(p.X+4,p.Y-2),3,4);

            dc.DrawEllipse(Brushes.Navy,null,new Point(p.X-4,p.Y-2),1.5,2);
 dc.DrawEllipse(Brushes.Navy,null,new Point(p.X+4,p.Y-2),1.5,2);

        }

    }

    private static Point Center(int x,int y) {
 return new Point(x*24+12,y*24+12);
 }

}

