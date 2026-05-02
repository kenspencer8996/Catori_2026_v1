using System;
using System.Collections.Generic;
using System.Text;

namespace CatoriUCLibrary.Views.RobotArmUC
{
    public class RobotPose
    {
        public double Joint1 { get; set; }
        public double Joint2 { get; set; }
        public double Joint3 { get; set; }
        public double JointHand { get; set; }
        public RobotPose(double joint1, double joint2, double joint3, double hand)
        {
            Joint1 = joint1;
            Joint2 = joint2;
            Joint3 = joint3;
            JointHand = hand;
        }

        public RobotPose Clone()
        {
            return new RobotPose(Joint1, Joint2, Joint3, JointHand);
        }
    }
}
