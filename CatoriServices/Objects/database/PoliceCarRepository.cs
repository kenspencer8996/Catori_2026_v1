using CatoriServices.Objects.Entities;
using CityAppServices;
using CityAppServices.Objects.database;
using Microsoft.Data.Sqlite;
using System.Data;
 
namespace CatoriServices.Objects.database
{
    public class PoliceCarRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();
      public List<PoliceCarEntity> GetPoliceCars()
        {
        		Int32 PoliceCarIdIndex = 0;
		Int32 CarNameIndex = 1;
		Int32 ImageNameIndex = 2;
		Int32 CarTypeIndex = 3;
                List<PoliceCarEntity> PoliceCars = new List<PoliceCarEntity>();
            try
            {
                string sql = "SELECT PoliceCarId, CarName, ImageName, CarType FROM PoliceCar" ;
                IDataReader reader = sqlhelper.GetReader(sql);
                while (reader.Read())
                {
                   PoliceCarEntity entity = new PoliceCarEntity();
                    try
                   {
   
          		if (!reader.IsDBNull(PoliceCarIdIndex)) 
			entity.PoliceCarId = reader.GetInt32(PoliceCarIdIndex); ;
		if (!reader.IsDBNull(CarNameIndex)) 
			entity.CarName = reader.GetString(CarNameIndex); ;
		if (!reader.IsDBNull(ImageNameIndex)) 
			entity.ImageName = reader.GetString(ImageNameIndex); ;
		if (!reader.IsDBNull(CarTypeIndex)) 
			entity.CarType = reader.GetString(CarTypeIndex); ;
  
                       
                       
                       PoliceCars.Add(entity);
 
                   }
                   catch (Exception ex)
                   {
                       throw;
                   }
               }
           }
           catch (Exception ex)
           {

               throw;
           }
           return PoliceCars;
       }
   
       public async Task<PoliceCarEntity> GetPoliceCarByIdAsync(Int32 PoliceCarID)
        {
            PoliceCarEntity entity = new PoliceCarEntity();
            try
           {
               string sql = "SELECT PoliceCarId, CarName, ImageName, CarType FROM PoliceCar" ;
               sql += " where PoliceCarID = " + PoliceCarID;
               IDataReader reader = sqlhelper.GetReader(sql);
               		Int32 PoliceCarIdIndex = 0;
		Int32 CarNameIndex = 1;
		Int32 ImageNameIndex = 2;
		Int32 CarTypeIndex = 3;

 
               while (reader.Read())
               {
                   try
                   {

          		if (!reader.IsDBNull(PoliceCarIdIndex)) 
			entity.PoliceCarId = reader.GetInt32(PoliceCarIdIndex); ;
		if (!reader.IsDBNull(CarNameIndex)) 
			entity.CarName = reader.GetString(CarNameIndex); ;
		if (!reader.IsDBNull(ImageNameIndex)) 
			entity.ImageName = reader.GetString(ImageNameIndex); ;
		if (!reader.IsDBNull(CarTypeIndex)) 
			entity.CarType = reader.GetString(CarTypeIndex); ;
  

                   }
                  catch (Exception ex)
                   {
                       throw;
                   }
               }
           }
          catch (Exception ex)
          {

              throw;
           }
           return entity;
       }
      public async void Upsert(PoliceCarEntity entity)
       {

           bool result = false;
           PoliceCarEntity found = await GetPoliceCarByIdAsync(entity.PoliceCarId);
          var connection = adoNetHelper.GetConnection();
          if (found != null )
          {
               try
              {
                   string sqlraw = "Update PoliceCar Set " + Environment.NewLine;
                  					sqlraw +="PoliceCarId = " + entity.PoliceCarId + ", ";
					sqlraw +="CarName = " + "'" + entity.CarName + "'" + ", ";
					sqlraw +="ImageName = " + "'" + entity.ImageName + "'" + ", ";
					sqlraw +="CarType = " + "'" + entity.CarType + "'";

                    sqlraw += "where PoliceCarID =  entity.PoliceCarID";
                   var command = new SqliteCommand();
                   command.Connection = connection;
                  command.CommandText = sqlraw;
                  command.ExecuteNonQuery();
              }
              catch (Exception ex)
              {

                   throw;
                }
           }
           else
           {
               try
               {
                   string sqlraw = "";
                   sqlraw += "INSERT INTO PoliceCar (PoliceCarId, CarName, ImageName, CarType)";
                   sqlraw += " Values(" + entity.PoliceCarId + "," +   "'" + entity.CarName + "'"   + "," +   "'" + entity.ImageName + "'"   + "," +   "'" + entity.CarType + "'"  + ")";
                   var command = new SqliteCommand();
                  command.Connection = connection;
                  command.CommandText = sqlraw;
                  command.ExecuteNonQuery();

              }
               catch (Exception ex)
               {

                   throw;
               }
          }
       }

       public void Delete(PoliceCarEntity entity)
       {
           string sql = "Delete from PoliceCar " + Environment.NewLine;
           sql += " where PoliceCarID = " + entity.PoliceCarId;
           var connection = adoNetHelper.GetConnection();
           try
           {
              var command = new SqliteCommand();
              command.Connection = connection;
              command.CommandText = sql;
               command.ExecuteNonQuery();

           }
           catch (Exception ex)
           {
               throw;
           }
       }
   }
}
