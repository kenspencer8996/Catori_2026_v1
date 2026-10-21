namespace CatoriUCLibrary.Views.Pacman.GameObjects;

public sealed class GameState
{

    public static readonly string[] Map =
    [
        "#####################",
        "#o........#........o#",
        "#.###.###.#.###.###.#",
        "#...................#",
        "#.###.#.#####.#.###.#",
        "#.....#...#...#.....#",
        "#####.###.#.###.#####",
        "#.....#.......#.....#",
        "#.###.#.##.##.#.###.#",
        "#...#...#...#...#...#",
        "###.#.#.#...#.#.#.###",
        "#.....#.......#.....#",
        "#.###.####.####.###.#",
        "#...#...........#...#",
        "###.#.###.#.###.#.###",
        "#.....#...#...#.....#",
        "#.#####.#.#.#.#####.#",
        "#.......#...#.......#",
        "#.###.####.####.###.#",
        "#o..#...........#..o#",
        "#.#.#.###.#.###.#.#.#",
        "#...................#",
        "#####################"
    ];

    public const int Tile = 24;

    public readonly HashSet<(int X,int Y)> Pellets = new();

    public readonly HashSet<(int X,int Y)> Powers = new();

    public readonly List<(int X,int Y)> Ghosts = new();

    public int X, Y, Direction = 1, Requested = 1, Score, Lives, Level;

    public bool Paused, Ended;

    public DateTime PoweredUntil, SafeUntil;

    private int tick;

    private readonly Random random = new();

    private static readonly (int X,int Y)[] Steps = [(0,-1),(1,0),(0,1),(-1,0)];

    public bool Powered {
 get {
 return DateTime.UtcNow < PoweredUntil;
 }
 }

    public bool Walkable(int x,int y) {
 return y >= 0 && y < Map.Length && x >= 0 && x < Map[0].Length && Map[y][x] != '#';
 }

    public void NewGame() {
 Score = 0;
 Lives = 3;
 Level = 1;
 Paused = Ended = false;
 Fill();
 ResetActors();
 }

    private void Fill()
    {

        Pellets.Clear();
 Powers.Clear();

        for (int y = 0;
 y < Map.Length;
 y++) for (int x = 0;
 x < Map[y].Length;
 x++)
        {

            if (Map[y][x] == '.') Pellets.Add((x,y));

            if (Map[y][x] == 'o') Powers.Add((x,y));

        }

    }

    private void ResetActors()
    {

        X = 10;
 Y = 17;
 Direction = Requested = 1;
 tick = 0;

        Ghosts.Clear();
 Ghosts.Add((9,11));
 Ghosts.Add((10,11));
 Ghosts.Add((11,11));
 Ghosts.Add((10,9));

        PoweredUntil = DateTime.MinValue;
 SafeUntil = DateTime.UtcNow.AddSeconds(2);

    }

    public void Advance()
    {

        if (Paused || Ended) return;

        var desired = Steps[Requested];

        if (Walkable(X + desired.X,Y + desired.Y)) Direction = Requested;

        var step = Steps[Direction];

        if (Walkable(X + step.X,Y + step.Y)) {
 X += step.X;
 Y += step.Y;
 }

        if (Pellets.Remove((X,Y))) Score += 10;

        if (Powers.Remove((X,Y))) {
 Score += 50;
 PoweredUntil = DateTime.UtcNow.AddSeconds(7);
 }

        if (Collide()) return;

        tick++;

        if (tick % (Powered ? 3 : 2) == 0)
        {

            for (int i = 0;
 i < Ghosts.Count;
 i++)
            {

                var g = Ghosts[i];
 var choices = new List<(int X,int Y)>();

                foreach (var move in Steps) if (Walkable(g.X + move.X,g.Y + move.Y)) choices.Add((g.X + move.X,g.Y + move.Y));

                if (choices.Count == 0) continue;

                var best = choices[random.Next(choices.Count)];

                if (random.Next(4) != 0)
                {

                    int distance = Powered ? -1 : int.MaxValue;

                    foreach (var choice in choices)
                    {

                        int d = Distance(choice.X, choice.Y);

                        if (Powered ? d > distance : d < distance) {
 distance = d;
 best = choice;
 }

                    }

                }

                Ghosts[i] = best;

            }

        }

        if (Collide()) return;

        if (Pellets.Count + Powers.Count == 0) {
 Level++;
 Fill();
 ResetActors();
 }

    }

    private int Distance(int x,int y)
    {

        var queue = new Queue<(int X,int Y,int D)>();
 var seen = new HashSet<(int,int)>();

        queue.Enqueue((x,y,0));
 seen.Add((x,y));

        while (queue.Count > 0)
        {

            var current = queue.Dequeue();
 if (current.X == X && current.Y == Y) return current.D;

            foreach (var step in Steps)
            {

                int nx = current.X + step.X, ny = current.Y + step.Y;

                if (Walkable(nx,ny) && seen.Add((nx,ny))) queue.Enqueue((nx,ny,current.D + 1));

            }

        }

        return 1000;

    }

    private bool Collide()
    {

        for (int i = 0;
 i < Ghosts.Count;
 i++)
        {

            if (Ghosts[i] != (X,Y)) continue;

            if (Powered) {
 Score += 200;
 Ghosts[i] = (10,11);
 }

            else if (DateTime.UtcNow >= SafeUntil) {
 Lives--;
 if (Lives == 0) Ended = true;
 else ResetActors();
 return true;
 }

        }

        return false;

    }

}

