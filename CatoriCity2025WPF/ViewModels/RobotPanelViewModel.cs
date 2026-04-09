using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriCity2025WPF.ViewModels
{
    public class RobotPanelViewModel
    {
        public string RobotName { get; set; } = "";
        public List<string> InputAItems { get; set; } = new();
        public List<string> InputBItems { get; set; } = new();
        public List<string> OutputItems { get; set; } = new();
        public List<string> ModeItems { get; set; } = new();
    }
}
