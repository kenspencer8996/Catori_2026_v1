using CatoriApp.Core.Objects.DragDrop;
using System.Runtime.CompilerServices;
namespace CatoriApp.Core.Objects.Shared
{
    public class GlobalCode
    {
        private static readonly ConditionalWeakTable<Canvas, DragManager> DragManagers = new();

        public static DragManager GetDragmanager(Canvas canvas)
        {
            return DragManagers.GetValue(canvas, static targetCanvas => new DragManager(targetCanvas));
        }
    }
}


