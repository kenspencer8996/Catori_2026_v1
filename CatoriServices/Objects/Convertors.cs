using CityAppServices.Objects.Entities;

namespace CatoriServices
{
    public class Convertors
    {
        public static ImageEnum GedImageEnumFromString(string iageenumstring)
        {
            ImageEnum result = ImageEnum.other;
            string imageenumlower = iageenumstring.ToLower();
            switch (imageenumlower)
            {
                case "imageenum.vehicle":
                    result = ImageEnum.vehicle;
                    break;
                case "imageenum.vehiclesales":
                    result = ImageEnum.vehiclesales;
                    break;
                case "imageenum.bank":
                    result = ImageEnum.bank;
                    break;
                case "imageenum.factory":
                    result = ImageEnum.factory;
                    break;
                case "imageenum.badguy":
                    result = ImageEnum.badguy;
                    break;
                case "imageenum.airport":
                    result = ImageEnum.airport;
                    break;
                case "imageenum.livingroom":
                    result = ImageEnum.livingroom;
                    break;
                case "imageenum.bedroom":
                    result = ImageEnum.bedroom;
                    break;
                case "imageenum.kitchen":
                    result = ImageEnum.kitchen;
                    break;
                case "imageenum.girl":
                    result = ImageEnum.girl;
                    break;
                case "imageenum.guy":
                    result = ImageEnum.guy;
                    break;
                case "imageenum.house":
                    result = ImageEnum.house;
                    break;
                case "imageenum.store":
                    result = ImageEnum.store;
                    break;
                case "imageenum.carlot":
                    result = ImageEnum.carlot;
                    break;
                case "imageenum.classroom":
                    result = ImageEnum.classroom;
                    break;
                case "imageenum.garage":
                    result = ImageEnum.garage;
                    break;
                case "imageenum.room":
                    result = ImageEnum.room;
                    break;
                case "imageenum.man":
                    result = ImageEnum.man;
                    break;
                case "imageenum.bush":
                    result = ImageEnum.bush;
                    break;
                case "imageenum.barn":
                    result = ImageEnum.barn;
                    break;
                case "imageenum.tree":
                    result = ImageEnum.tree;
                    break;
                case "imageenum.campfire":
                    result = ImageEnum.campfire;
                    break;
                case "imageenum.shed":
                    result = ImageEnum.shed;
                    break;
                case "imageenum.forest":
                    result = ImageEnum.forest;
                    break;
                case "imageenum.petshop":
                    result = ImageEnum.petshop;
                    break;
                case "imageenum.police":
                    result = ImageEnum.police;
                    break;
                case "imageenum.policestation":
                    result = ImageEnum.policestation;
                    break;
                case "imageenum.tent":
                    result = ImageEnum.tent;
                    break;
                default:
                    result = ImageEnum.other;
                    break;

            }
            return result;
        }
        public static BusinessTypeEnum GetBusinessTypeEnum(string businessTypeEnumstring)
        {
            BusinessTypeEnum result = BusinessTypeEnum.Factory;
            string businessTypeEnumlower = businessTypeEnumstring.ToLower();
            switch (businessTypeEnumlower)
            {
                case "businesstypeenum.airport":
                    result = BusinessTypeEnum.Airport;
                    break;
                case "businesstypeenum.financial":
                    result = BusinessTypeEnum.Financial;
                    break;
                case "businesstypeenum.result":
                    result = BusinessTypeEnum.Retail;
                    break;
                case "businesstypeenum.government":
                    result = BusinessTypeEnum.Government;
                    break;
                case "businesstypeenum.factory":
                    result = BusinessTypeEnum.Factory;
                    break;
                case "businesstypeenum.carlot":
                    result = BusinessTypeEnum.Carlot;
                    break;
                default:
                    break;

            }
            return result;
        }
        public static PersonEnum GetPersonEnum(string personenumstring)
        {
            PersonEnum result = PersonEnum.Individual;
            string personenumlower = personenumstring.ToLower();
            switch (personenumlower)
            {
                case "individual":
                    result = PersonEnum.Individual;
                    break;
                case ".police":
                    result = PersonEnum.Police;
                    break;
                case "badperson":
                    result = PersonEnum.BadPerson;
                    break;
                case "govworker":
                    result = PersonEnum.GovWorker;
                    break;
                case "medical":
                    result = PersonEnum.Medical;
                    break;
                default:
                    break;

            }
            return result;
        }
        public static PersonImageTypeEnum GetPersonImageTypeEnum(string personImageTypeEnum)
        {
            PersonImageTypeEnum result = PersonImageTypeEnum.Normal;
            string personImageTypeEnumlower = personImageTypeEnum.ToLower();
            switch (personImageTypeEnumlower)
            {
                case "personimagetypeenum.badguy":
                    result = PersonImageTypeEnum.BadGuy;
                    break;
                case "personimagetypeenum.working":
                    result = PersonImageTypeEnum.Working;
                    break;
                case "personimagetypeenum.normal":
                    result = PersonImageTypeEnum.Normal;
                    break;
                default:
                    break;

            }
            return result;
        }
        public static PersonImageStatusEnum GetPersonImageStatusEnumFromInt(int enumint)
        {
            return (PersonImageStatusEnum)Enum.ToObject(typeof(PersonImageStatusEnum), enumint);
        }
        public static PersonImageStatusEnum GetPersonImageStatus(string? personimagrenumstring)
        {
            PersonImageStatusEnum  result = PersonImageStatusEnum.Normal;
            string personimagrenumlower = personimagrenumstring.ToLower();
            switch (personimagrenumlower)
            {
                case "personimagestatusenum.money": 
                    result = PersonImageStatusEnum.Money;
                    break;
                case "personimagestatusenum.smirk": 
                    result = PersonImageStatusEnum.Smirk;
                    break;
                case "personimagestatusenum.normal": 
                    result = PersonImageStatusEnum.Normal;
                    break;
                case "personimagestatusenum.armsup": 
                    result = PersonImageStatusEnum.ArmsUp;
                    break;
                case "personimagestatusenum.armsout": 
                    result = PersonImageStatusEnum.ArmsOut;
                    break;
                case "personimagestatusenum.armsdown": 
                    result = PersonImageStatusEnum.ArmsDown;
                    break;
                case "personimagestatusenum.devil": 
                    result = PersonImageStatusEnum.Devil;
                    break;
                case "personimagestatusenum.mad": 
                    result = PersonImageStatusEnum.Mad;
                    break;
                case "personimagestatusenum.cash": 
                    result = PersonImageStatusEnum.Cash;
                    break;
                case "personimagestatusenum.cashgun": 
                    result = PersonImageStatusEnum.CashGun;
                    break;
                case "personimagestatusenum.cashPiles": 
                    result = PersonImageStatusEnum.CashPiles;
                    break;
                case "personimagestatusenum.computer": 
                    result = PersonImageStatusEnum.Computer;
                    break;
                case "personimagestatusenum.gun": 
                    result = PersonImageStatusEnum.Gun;
                    break;
                case "personimagestatusenum.guns": 
                    result = PersonImageStatusEnum.Guns;
                    break;
                case "personimagestatusenum.loot": 
                    result = PersonImageStatusEnum.Loot;
                    break;
                case "personimagestatusenum.lookingleft": 
                    result = PersonImageStatusEnum.LookingLeft;
                    break;
                case "personimagestatusenum.lookingright":
                    result = PersonImageStatusEnum.LookingRight ;
                    break;
                case "personimagestatusenum.lookingleft45":
                    result = PersonImageStatusEnum.LookingLeft;
                    break;
                case "personimagestatusenum.lookingright45": 
                    result = PersonImageStatusEnum.LookingLeft45;
                    break;
                case "personimagestatusenum.pacing": 
                    result = PersonImageStatusEnum.Facing;
                    break;
                case "personimagestatusenum.lookingaway": 
                    result = PersonImageStatusEnum.LookingAway;
                    break;
                case "personimagestatusenum.headkookingright45": 
                    result = PersonImageStatusEnum.HeadLookingRight45;
                    break;
                case "personimagestatusenum.headlookingleft45": 
                    result = PersonImageStatusEnum.HeadLookingLeft45;
                    break;
                case "personimagestatusenum.dancing":
                    result = PersonImageStatusEnum.Dancing;
                    break;
                case "personimagestatusenum.running": 
                    result = PersonImageStatusEnum.Running;
                    break;
                case "personimagestatusenum.walking": 
                    result = PersonImageStatusEnum.Walking;
                    break;
                case "personimagestatusenum.lookimgback": 
                    result = PersonImageStatusEnum.LookimgBack;break;
                case "personimagestatusenum.airguitar":
                    result = PersonImageStatusEnum.Airguitar; 
                    break;
                default:
                    break;
            }
            return result;
        }
    }
}
