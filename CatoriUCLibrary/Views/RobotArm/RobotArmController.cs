using CatoriUCLibrary;
using System.Diagnostics;
using System.Windows.Media.Imaging;

namespace CatoriUCLibrary.Views.RobotArm
{
    public class RobotArmController
    {
        private RoboticArmUC _view;
        private List<RobotPartEntity> ArmParts;
        private string imageBasePath = "C:\\Development\\Gaming\\Catori2026\\Catori_2026_v1\\Images\\Factories\\RobotArms";
        public RobotArmController(RoboticArmUC view)
        {
            _view = view;
            LoadParts();
        }
        public void SetupRobot(RobotColorEnum color)
        {

            var basePart = ArmParts.FirstOrDefault(p => p.Type == RobotPartType.Base && p.Color == color);  
            var armShort = ArmParts.FirstOrDefault(p => p.Type == RobotPartType.ArmShort && p.Color == color);
            var armMedium = ArmParts.FirstOrDefault(p => p.Type == RobotPartType.ArmMedium && p.Color == color);
            var armLong = ArmParts.FirstOrDefault(p => p.Type == RobotPartType.ArmLong && p.Color == color);
            var hand = ArmParts.FirstOrDefault(p => p.Type == RobotPartType.Hand && p.Color == color);
            basePart.ImagePath = GetImagePath(basePart);
            armShort.ImagePath = GetImagePath(armShort);
            armMedium.ImagePath = GetImagePath(armMedium);
            armLong.ImagePath = GetImagePath(armLong);
            hand.ImagePath = GetImagePath(hand);
            //_view.Segment1Image.Source = UIUtility.GetImageControl(armMedium.ImagePath, 30, 140, 1000).Source;
            //_view.Segment2Image.Source = UIUtility.GetImageControl(armLong.ImagePath, 30, 140, 1000).Source;
            //_view.Segment3Image.Source = UIUtility.GetImageControl(armLong.ImagePath, 30, 140, 1000).Source;
            //_view.SegmentHandImage.Source = UIUtility.GetImageControl(hand.ImagePath, 30, 140, 1000).Source;
            //_view.RobotBaseImage.Source = UIUtility.GetImageControl(basePart.ImagePath, 30, 140, 1000).Source;
            //Debug.WriteLine($"Seg1: {_view.Segment1Image.Source}"); 
            //Debug.WriteLine($"Seg2: {_view.Segment2Image.Source}");
            //Debug.WriteLine($"Seg3: {_view.Segment3Image.Source}");
            //Debug.WriteLine($"Seg4: {_view.SegmentHandImage.Source}");
        }
        private string GetImagePath(RobotPartEntity part)
        {
            return System.IO.Path.Combine(imageBasePath, part.ImagePath);
        }   
        public double Joint1Angle { get; set; }
        public double Joint2Angle { get; set; }
        public double Joint3Angle { get; set; }
        public double Joint4Angle { get; set; }

        private void LoadParts()
        {
            ArmParts = new List<RobotPartEntity>();
            RobotPartEntity part = GetPart("RobotArmHandBlue.png", RobotColorEnum.Blue, RobotPartType.Hand);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmHandGray.png",RobotColorEnum.Gray, RobotPartType.Hand);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmHandRed.png", RobotColorEnum.Red, RobotPartType.Hand);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmHandYellow.png", RobotColorEnum.Yellow, RobotPartType.Hand);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("robotArmLongBlue.png", RobotColorEnum.Blue, RobotPartType.ArmLong);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmMediumGray.png", RobotColorEnum.Gray, RobotPartType.ArmMedium);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmMediumRed.png", RobotColorEnum.Red, RobotPartType.ArmMedium);
            part = new RobotPartEntity();
            part = GetPart("RobotArmMediumBlue.png", RobotColorEnum.Blue, RobotPartType.ArmMedium);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmMediumYellow.png", RobotColorEnum.Yellow, RobotPartType.ArmMedium);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmShortBlue.png", RobotColorEnum.Blue, RobotPartType.ArmShort)                    ;
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmShortGray.png", RobotColorEnum.Gray, RobotPartType.ArmShort);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmShortRed.png", RobotColorEnum.Red, RobotPartType.ArmShort);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotArmShortYellow.png", RobotColorEnum.Yellow, RobotPartType.ArmShort);
            ArmParts.Add(part);
              part = new RobotPartEntity();
            part = GetPart("RobotBaseBlue.png", RobotColorEnum.Blue, RobotPartType.Base)        ;
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotBaseGray.png", RobotColorEnum.Gray, RobotPartType.Base);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotBaseRed.png", RobotColorEnum.Red, RobotPartType.Base);
            ArmParts.Add(part);
            part = new RobotPartEntity();
            part = GetPart("RobotBaseYellow.png", RobotColorEnum.Yellow, RobotPartType.Base);
            ArmParts.Add(part);
        }
        private RobotPartEntity GetPart(string imagename,RobotColorEnum color,RobotPartType type)
        {
            RobotPartEntity part = new RobotPartEntity();
            part.ImagePath = imagename;
            part.Color = color;
            part.Type = type;
            part.Name = System.IO.Path.GetFileNameWithoutExtension( imagename);
            return part;
        }
    }
}
