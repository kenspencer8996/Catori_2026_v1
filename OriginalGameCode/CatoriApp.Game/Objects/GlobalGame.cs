using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriApp.Game.Objects
{
    public class GlobalGame
    {
        public static PersonViewModel CurrentPerson { get; set; }
        public static List<PersonViewModel> AllPersons { get; set; } = new List<PersonViewModel>();
        public static Window MainWindow { get; set; }
    }
}
