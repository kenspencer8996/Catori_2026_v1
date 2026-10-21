using System.Windows;

namespace CatoriUCLibrary.Views.SuperMario.GameObjects;




public sealed class Engine
{

    public const double WorldWidth=6400;

    public List<Tile> Tiles {
 get;
 }
=[];

    public List<Enemy> Enemies {
 get;
 }
=[];

    public List<Point> Coins {
 get;
 }
=[];

    public PlayState State {
 get;
 private set;
 }
=PlayState.Ready;

    public double X=100,Y=400,VX,VY,Camera,Time,Remaining=300,Invulnerable;

    public int Score,Collected,Lives=3;

    public bool Grounded,Powered;

    public Rect Player => new(X,Y,28,36);

    private bool jumpHeld;

    private double checkpoint=100;

    public Engine() {
 Build();
 }

    private void Build()
    {

        Tiles.Clear();
 Enemies.Clear();
 Coins.Clear();

        for(int x=0;
x<160;
x++) if (!(x>=28&&x<=30||x>=60&&x<=63||x>=101&&x<=104)) {
 Tiles.Add(new(x,12,'g'));
 Tiles.Add(new(x,13,'g'));
 }

        foreach(int x in new[]{
12,14,16,35,37,50,76,78,113,115}
) Tiles.Add(new(x,8,'?'));

        foreach(int x in new[]{
13,15,36,38,49,51,77,79,114}
) Tiles.Add(new(x,8,'b'));

        foreach(int start in new[]{
22,43,87,126}
) for(int dx=0;
dx<2;
dx++) for(int y=10;
y<12;
y++) Tiles.Add(new(start+dx,y,'p'));

        for(int i=0;
i<5;
i++) for(int y=11-i;
y<12;
y++) Tiles.Add(new(139+i,y,'s'));

        foreach(int x in new[]{
18,39,54,72,83,96,119,131}
) Enemies.Add(new(x*40,452));

        foreach(int start in new[]{
9,33,47,69,109,123}
) for(int i=0;
i<5;
i++) Coins.Add(new((start+i)*40+20,280));

    }

    public void Start() {
 Build();
Score=Collected=0;
Lives=3;
Remaining=300;
Time=0;
checkpoint=100;
Powered=false;
State=PlayState.Playing;
Respawn();
 }

    public void Pause() {
 if(State==PlayState.Playing) State=PlayState.Paused;
 else if(State==PlayState.Paused) State=PlayState.Playing;
 }

    private void Respawn() {
 X=checkpoint;
Y=380;
VX=VY=0;
Grounded=false;
Invulnerable=2;
jumpHeld=false;
 }

    private void Hurt() {
 if(Invulnerable>0)return;
 if(Powered){
Powered=false;
Invulnerable=2;
return;
}
 LoseLife();
 }

    private void LoseLife() {
 if(--Lives<=0) State=PlayState.GameOver;
 else {
 Remaining=300;
Respawn();
 }
 }

    public void Update(double dt,bool left=false,bool right=false,bool jump=false,bool run=false)
    {

        if(State!=PlayState.Playing)return;

        dt=Math.Clamp(dt,0,0.04);
Time+=dt;
Remaining-=dt;
Invulnerable=Math.Max(0,Invulnerable-dt);

        if(Remaining<=0){
LoseLife();
return;
}

        double target=((right?1:0)-(left?1:0))*(run?290:190);

        VX=Approach(VX,target,(Grounded?1500:850)*dt);

        if(jump&&!jumpHeld&&Grounded){
VY=-540;
Grounded=false;
}

        if(!jump&&VY < -200)VY=-200;

        jumpHeld=jump;
 VY=Math.Min(800,VY+1450*dt);

        X=Math.Clamp(X+VX*dt,0,WorldWidth-28);

        foreach(var t in Tiles) if(Overlaps(Player,t.Bounds)){
if(VX>0)X=t.X-28;
else if(VX<0)X=t.X+40;
VX=0;
}

        double oldBottom=Y+36;
Y+=VY*dt;
Grounded=false;

        foreach(var t in Tiles.ToArray()) if(Overlaps(Player,t.Bounds))
        {

            if(VY>=0){
Y=t.Y-36;
VY=0;
Grounded=true;
}

            else {
Y=t.Y+40;
VY=0;
if(t.Kind=='?'){
t.Kind='u';
Score+=200;
Collected++;
if(Collected%3==0)Powered=true;
}
else if(t.Kind=='b'&&Powered){
Tiles.Remove(t);
Score+=50;
}
}

        }

        foreach(var coin in Coins.ToArray()) if(Overlaps(Player,new Rect(coin.X-10,coin.Y-14,20,28))){
Coins.Remove(coin);
Collected++;
Score+=100;
}

        foreach(var e in Enemies.Where(e=>e.Alive))
        {

            e.X+=e.VX*dt;

            if(Tiles.Any(t=>Overlaps(e.Bounds,t.Bounds)) || !Tiles.Any(t=>t.Bounds.Contains(new Point(e.VX<0?e.X-2:e.X+32,481)))) {
e.X-=e.VX*dt;
e.VX=-e.VX;
}

            if(Overlaps(Player,e.Bounds)) {
if(VY>0&&oldBottom<=e.Y+12){
e.Alive=false;
VY=-340;
Score+=200;
}
else Hurt();
}

        }

        if(Y>580){
LoseLife();
return;
}

        if(X>3200)checkpoint=3240;

        Camera=Math.Clamp(X-320,0,WorldWidth-960);

        if(X>=6080){
Score+=(int)Remaining*10;
State=PlayState.Won;
}

    }

    private static bool Overlaps(Rect a,Rect b)=>a.Left<b.Right-0.001&&a.Right>b.Left+0.001&&a.Top<b.Bottom-0.001&&a.Bottom>b.Top+0.001;

    private static double Approach(double a,double b,double step)=>a<b?Math.Min(a+step,b):Math.Max(a-step,b);

}


