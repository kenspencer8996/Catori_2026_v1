namespace CatoriCity2025WPF.Objects
{
    public class StreetsModel
    {
        public string GetStreet(StreetsEnum street)
        {
            switch (street)
            {
                case StreetsEnum.Teastreet:
                    return TeastreetName;
                case StreetsEnum.YouStreet:
                    return YouStreetName;
                case StreetsEnum.MikAve:
                    return MikAveName;
                case StreetsEnum.YodelLane:
                    return YodelLaneName;
                case StreetsEnum.MooDr:
                    return MooDrName;
                default:
                    return "Unknown Street";
            }
        }
        private string TeastreetName
        {
            get
            {
                return "Tea Street";
            }
        }
        private string YouStreetName
        {
            get
            {
                return "You Street";
            }
        }
        private string MikAveName
        {
            get
            {
                return "Mik Avenue";
            }
}
        private string YodelLaneName
        {
            get
            {
                return "Yodel Lane";
            }
        }
        private string MooDrName
        {
            get
            {
                return "Moo Drive";
            }
        }

    }
    public enum StreetsEnum
    {
        Teastreet,
        YouStreet,
        MikAve,
        YodelLane,
        MooDr
    }
}
