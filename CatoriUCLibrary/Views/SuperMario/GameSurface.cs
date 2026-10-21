using System.Globalization;

using System.Windows;

using System.Windows.Media;

using CatoriUCLibrary.Views.SuperMario.GameObjects;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;

public sealed class GameSurface : FrameworkElement
{

    public Engine? Engine {
get;
set;
}

    private static readonly Dictionary<string,Brush> brushes=[];

    private static Brush B(string color){
if(!brushes.TryGetValue(color,out var b)){
b=new SolidColorBrush((Color)ColorConverter.ConvertFromString(color));
b.Freeze();
brushes[color]=b;
}
return b;
}

    private static void Box(DrawingContext d,string c,double x,double y,double w,double h)=>d.DrawRectangle(B(c),null,new Rect(x,y,w,h));

    private void Text(DrawingContext d,string text,double x,double y,double size,string color)=>d.DrawText(new FormattedText(text,CultureInfo.InvariantCulture,FlowDirection.LeftToRight,new Typeface("Consolas"),size,B(color),VisualTreeHelper.GetDpi(this).PixelsPerDip),new Point(x,y));

    protected override void OnRender(DrawingContext d)
    {

        if(Engine is not {
}
 g)return;

        Box(d,"#79c9f3",0,0,960,540);

        for(int i=0;
i<12;
i++)
        {

            double x=i*260-g.Camera*.25;

            d.DrawEllipse(B("#b5e6ac"),null,new Point(x,482),130,120);

            d.DrawEllipse(B("#78c985"),null,new Point(x+75,480),75,72);

            double cx=i*320-g.Camera*.15;

            d.DrawEllipse(B("#f5fbff"),null,new Point(cx+45,145),45,19);

            d.DrawEllipse(B("#f5fbff"),null,new Point(cx+22,150),30,14);

            d.DrawEllipse(B("#f5fbff"),null,new Point(cx+75,150),28,14);

        }

        d.PushClip(new RectangleGeometry(new Rect(0,0,960,540)));

        d.PushTransform(new TranslateTransform(-g.Camera,0));

        foreach(var t in g.Tiles.Where(t=>t.X>=g.Camera-40&&t.X<g.Camera+1000))
        {

            string c=t.Kind switch{
'g'=>"#b56d3c",'p'=>"#238c4c",'?'=>"#ffcc47",'u'=>"#987854",'s'=>"#ca9460",_=>"#d17b48"}
;

            Box(d,"#64422f",t.X,t.Y,40,40);
Box(d,c,t.X+2,t.Y+2,36,36);

            if(t.Kind=='g'){
Box(d,"#4fa54b",t.X,t.Y,40,7);
Box(d,"#d99b57",t.X+7,t.Y+17,7,6);
Box(d,"#8c502f",t.X+25,t.Y+29,9,5);
}

            if(t.Kind=='?')Text(d,"?",t.X+12,t.Y+4,27,"#8b4b1c");

            if(t.Kind=='b'){
Box(d,"#794e34",t.X,t.Y+19,40,2);
Box(d,"#794e34",t.X+19,t.Y,2,20);
Box(d,"#794e34",t.X+9,t.Y+20,2,20);
}

            if(t.Kind=='p'){
Box(d,"#5cda69",t.X+5,t.Y+3,6,34);
Box(d,"#146b39",t.X+30,t.Y+3,5,34);
}

        }

        foreach(var c in g.Coins){
d.DrawEllipse(B("#a56a17"),null,new Point(c.X,c.Y),10,14);
d.DrawEllipse(B("#ffe061"),null,new Point(c.X,c.Y),7,12);
Box(d,"#fff3b3",c.X-1,c.Y-7,2,14);
}

        foreach(var e in g.Enemies.Where(e=>e.Alive))
        {

            Box(d,"#623622",e.X+3,e.Y+5,24,19);
Box(d,"#b77545",e.X+7,e.Y,16,23);

            Box(d,"#fff2db",e.X+5,e.Y+8,7,7);
Box(d,"#fff2db",e.X+18,e.Y+8,7,7);
Box(d,"#342d2e",e.X+9,e.Y+9,3,6);
Box(d,"#342d2e",e.X+18,e.Y+9,3,6);

            Box(d,"#342d2e",e.X-1,e.Y+23,12,5);
Box(d,"#342d2e",e.X+19,e.Y+23,12,5);

        }

        Box(d,"#e8f2d8",6110,200,5,280);
d.DrawEllipse(B("#ffe073"),null,new Point(6112,197),7,7);

        var flag=new StreamGeometry();
using(var ctx=flag.Open()){
ctx.BeginFigure(new Point(6115,205),true,true);
ctx.LineTo(new Point(6160,220),true,false);
ctx.LineTo(new Point(6115,238),true,false);
}
d.DrawGeometry(B("#ee5353"),null,flag);

        Box(d,"#af6847",6220,360,130,120);
Box(d,"#503929",6265,423,40,57);
for(int i=0;
i<4;
i++)Box(d,"#af6847",6220+i*36,342,22,20);

        if(g.Invulnerable<=0||(int)(g.Time*12)%2==0)DrawPlayer(d,g);

        d.Pop();
d.Pop();

        Box(d,"#e6192c44",0,0,960,68);

        Text(d,"MARIO",25,10,14,"#9bb8ce");
Text(d,g.Score.ToString("D6"),25,28,24,"#fff3d0");

        Text(d,$"COINS  {g.Collected:D2}",220,24,21,"#ffe073");
Text(d,$"LIVES  {g.Lives}",435,24,21,"#fff3d0");
Text(d,"WORLD 1–1",615,24,21,"#fff3d0");
Text(d,$"TIME {(int)Math.Max(0,g.Remaining):D3}",800,24,21,"#fff3d0");

        if(g.Powered)Text(d,"POWER UP!",25,80,18,"#fff3d0");

        if(g.State!=PlayState.Playing)
        {

            Box(d,"#bd101b30",0,68,960,472);

            string title=g.State switch{
PlayState.Ready=>"LET'S-A GO!",PlayState.Paused=>"PAUSED",PlayState.Won=>"COURSE CLEAR!",_=>"GAME OVER"}
;

            Text(d,title,290,210,42,"#ffe073");

            Text(d,g.State==PlayState.Paused?"P / ESC / START TO RESUME":"ENTER / CONTROLLER A TO PLAY",255,284,24,"#ffffff");

            Text(d,"Collect coins. Stomp enemies. Reach the flag.",220,333,18,"#b7d2e6");

        }

    }

    private static void DrawPlayer(DrawingContext d,Engine g)
    {

        double x=Math.Round(g.X),y=Math.Round(g.Y);
bool step=g.Grounded&&Math.Abs(g.VX)>10&&(int)(g.Time*12)%2==0;

        Box(d,g.Powered?"#fff4d9":"#e84637",x+3,y,22,5);
Box(d,g.Powered?"#fff4d9":"#e84637",x+1,y+5,27,4);

        Box(d,"#ffc992",x+5,y+9,20,10);
Box(d,"#75442e",x+3,y+9,7,10);
Box(d,"#292e40",x+20,y+9,3,5);
Box(d,"#75442e",x+15,y+16,12,3);

        Box(d,g.Powered?"#fff4d9":"#e84637",x+3,y+19,22,10);
Box(d,"#ffc992",x,y+22,5,7);
Box(d,"#ffc992",x+24,y+22,4,7);

        Box(d,"#386ec5",x+8,y+20,4,10);
Box(d,"#386ec5",x+19,y+20,4,10);
Box(d,"#386ec5",x+6,y+27,18,6);
Box(d,"#ffe073",x+9,y+26,3,3);

        Box(d,"#663c30",x+(step?0:3),y+33,11,3);
Box(d,"#663c30",x+(step?20:17),y+33,11,3);

    }

}

