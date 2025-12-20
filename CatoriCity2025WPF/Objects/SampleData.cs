using CatoriCity2025WPF.Objects;
using CityAppServices;
using CityAppServices.Objects.Entities;

namespace CatoriCity2025WPF.Objects
{
    public static class SampleData

    {
        static PersonEntity newPersonEntity = new PersonEntity();
        static PersonEntity PersonEntity = new PersonEntity();
        static PersonEntity catoriPersonEntity = new PersonEntity();
        static PersonEntity badperson1Entity = new PersonEntity();
        static PersonEntity badperson2Entity = new PersonEntity();
        static PersonEntity badperson3Entity = new PersonEntity();
        static PersonEntity badperson4Entity = new PersonEntity();


       
        public static async Task<bool> CreateSampledataAsync()
        {
            bool insertedSomething = false;

            try
            {
                ReturnForInsertMethods thisoneadded = new ReturnForInsertMethods();
                if (GlobalStuff.PersonImages.Count() == 0 && GlobalStuff.Persons.Count() == 0)
                {
                    thisoneadded = Insert_PersonsAndPersonImagesAsync();
                    CheckInsertStatus(thisoneadded,ref insertedSomething);
                }

                if (GlobalStuff.PersonImages.Count() == 0)
                {
                    thisoneadded = Insert_PersonImages();
                    CheckInsertStatus(thisoneadded, ref insertedSomething);
                }
     
                if (GlobalStuff.Houses.Count() == 0)
                {
                    thisoneadded = Insert_Houses();
                    CheckInsertStatus(thisoneadded, ref insertedSomething);
                }
                if (GlobalStuff.Images.Count() == 0)
                {
                    thisoneadded = Insert_Images();
                    CheckInsertStatus(thisoneadded, ref insertedSomething);
                }
                if (GlobalStuff.Businesses.Count() == 0)
                {
                    thisoneadded = Insert_Businesses();
                    CheckInsertStatus(thisoneadded, ref insertedSomething);
                }
                 //newPersonEntity = await GlobalServices.InsertPersonAsync("Joe Skinny", false, PersonEnum.BadGuy);

                //newPersonEntity = await GlobalServices.InsertPersonAsync("Mojo", false, PersonEnum.BadGuy);
                //newPersonEntity = await GlobalServices.InsertPersonAsync("sue Devil", false, PersonEnum.BadGuy);

            }
            catch (Exception ex4)
            {
                throw ex4;
            }
            return insertedSomething;
        }

        private static void CheckInsertStatus(ReturnForInsertMethods thisoneadded,ref bool inserted)
        {
            if (thisoneadded.Information == "insertedSomething")
                inserted = true;
        }

 
            private static ReturnForInsertMethods Insert_Businesses()
            {
                ReturnForInsertMethods insertedSomething = new ReturnForInsertMethods();
                if (GlobalStuff.Businesses.Count() == 0)
                {
                    try
                    {
                        insertedSomething.MethodName = "AddBusinesses";
                        insertedSomething.Information = "insertedSomething";
                        GlobalServices.InsertBusiness("Jones Fin.", 200, "bank_1.jpg", BusinessTypeEnum.Financial);
                        GlobalServices.InsertBusiness("123 Bank", 200, "bank_2.jpg", BusinessTypeEnum.Financial);
                        GlobalServices.InsertBusiness("Iron Factory", 130, "factory_1.jpg", BusinessTypeEnum.Factory);
                        GlobalServices.InsertBusiness("Auto Factory", 150, "factory_2.jpg", BusinessTypeEnum.Factory);
                        GlobalServices.InsertBusiness("Auto Factory2", 150, "factory_3.jpg", BusinessTypeEnum.Factory);
                        GlobalServices.InsertBusiness("Auto Factory3", 150, "factory_4.png", BusinessTypeEnum.Factory);
                    
                }
                catch (Exception ex2)
                    {
                        insertedSomething.Information += Environment.NewLine
                            + "exception:" + ex2.Message;
                    }
                }
            return insertedSomething;
        }
        private static ReturnForInsertMethods Insert_Houses()
        {
            ReturnForInsertMethods insertedSomething = new ReturnForInsertMethods();
            if (GlobalStuff.Houses.Count() == 0)
            {
                insertedSomething.Information = "insertedSomething";
                try
                {
                    GlobalServices.InsertHouse("Catories", "house_3d4.jpg", "living_10_roomarmchair.png", "kitchen_1_room.jpg", "garage_1.jpg", true, "Catories");

                    GlobalServices.InsertHouse("Jeffs", "houase_13_3dwyard.jpg", "living__7_room.jpg", "kitchen_2_room.jpg", "garage_2.jpg", false, "Jeffs");
                    GlobalServices.InsertHouse("Quyens", "house_11_3d1.jpg", "living_6_room.jpg", "kitchen_3_room.jpg", "garage_3.jpg", false, "Quyens");
                    GlobalServices.InsertHouse("Joe", "house_12_3ds2.jpg", "living__7_room.jpg", "kitchen_2_room.jpg", "garage_2.jpg", false, "Joe");
                    GlobalServices.InsertHouse("Papa", "house_3d4.jpg", "living_11_roombig.png", "kitchen_3_room.jpg", "garage_1.jpg", false, "Papa");
                    GlobalServices.InsertHouse("Gaga", "house_5_dkbrownroofgarage.jpg", "living__7_room.jpg", "kitchen_2_room.jpg", "garage_3.jpg", false, "Gaga");
                    GlobalServices.InsertHouse("Sara", "house_6_grayroofgarage.jpg", "living_11_roombig.png", "kitchen1.jpg", "garage_2.jpg", false, "Sara");
                    GlobalServices.InsertHouse("John", "house1.jpg", "living__7_room.jpg", "kitchen_3_room.jpg", "garage_1.jpg", false, "John");
                    GlobalServices.InsertHouse("Gary", "house2.jpg", "living_6_room.jpg", "kitchen_2_room.jpg", "garage_1.jpg", false, "Gary");
                    GlobalServices.InsertHouse("Sue", "house2.jpg", "living_11_roombig.png", "kitchen1.jpg", "garage_2.jpg", false, "Sue");
                    GlobalServices.InsertHouse("Bill", "house3.jpg", "living__7_room.jpg", "kitchen_3_room.jpg", "garage_3.jpg", false, "Bill");
                    GlobalServices.InsertHouse("Jack", "house_20.jpg", "living_6_room.jpg", "kitchen_2_room.jpg", "garage_2.jpg", false, "Jack");
                    GlobalServices.InsertHouse("June", "house_21.jpg", "living_6_room.jpg", "kitchen_2_room.jpg", "garage_2.jpg", false, "June");

                }
                catch (Exception ex3)
                {
                    insertedSomething.Information +=  Environment.NewLine 
                        + "exception:" + ex3.Message;
                }
            }
            return insertedSomething;
       }
    }
     
 }
