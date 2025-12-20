using System.Xml.Linq;

namespace CityAppServices.Objects.Entities
{
    public class SettingEntity
    {
        public SettingEntity() { }
        private int _settingID;
        private string _name = string.Empty;
        private string _stringSetting = string.Empty;
        private int _intSetting = 0;

        public SettingEntity(string name,string stringSetting, int intSetting) 
        {
            Name = name;
            StringSetting = stringSetting;
            IntSetting = intSetting;
        }
        public int SettingID
        {
            get => _settingID;
            set => _settingID = value;
        }

        public string Name
        {
            get => _name;
            set => _name = value ?? string.Empty;
        }

        public string StringSetting
        {
            get => _stringSetting;
            set => _stringSetting = value ?? string.Empty;
        }

        public int IntSetting
        {
            get 
            { 
                return _intSetting;
            }

            set => _intSetting = value;
        }
    }
}
