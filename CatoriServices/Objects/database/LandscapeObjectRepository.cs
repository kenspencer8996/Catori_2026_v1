using CatoriServices.Objects.Entities;
using CityAppServices;
using CityAppServices.Objects.database;
using Microsoft.Data.Sqlite;
using System.Data;

namespace CatoriServices.Objects.database
{
    public class LandscapeObjectRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();
        SqlHelper sqlhelper = new SqlHelper();
        public async Task<List<LandscapeObjectEntity>> GetLandscapeObjectsAsync()
        {
            List<LandscapeObjectEntity> LandscapeObjects = new List<LandscapeObjectEntity>();
            try
            {
                string sql = "SELECT LandScapeObjectID,Name,Description,Height,Width,xActual,yActual,ImageName,GroupId,HomeObject,NextFromHomeObject,FeatureNote FROM LandScapeObject";
                sql += " where groupid = " + GlobalServices.LandscapeObjecGroupid;
                IDataReader reader = sqlhelper.GetReader(sql);
                Int32 LandScapeObjectIDIndex = 0;
                Int32 NameIndex = 1;
                Int32 DescriptionIndex = 2;
                Int32 HeightIndex = 3;
                Int32 WidthIndex = 4;
                Int32 xActualIndex = 5;
                Int32 yActualIndex = 6;
                Int32 ImageNameIndex = 7;
                Int32 GroupIdIndex = 8;
                Int32 HomeObjectIndex = 9;
                Int32 NextFromHomeObjectIndex = 10;
                Int32 FeatureNoteIndex = 11;
                while (reader.Read())
                {
                    LandscapeObjectEntity landscape = new LandscapeObjectEntity();
                    try
                    {
                        landscape.Name = reader.GetString(NameIndex);

                        if (!reader.IsDBNull(LandScapeObjectIDIndex))
                            landscape.LandScapeObjectID = reader.GetInt32(LandScapeObjectIDIndex);
                        string desc = reader.GetString(DescriptionIndex);
                        landscape.Description = desc;
                        landscape.Width = reader.GetDouble(WidthIndex);
                        landscape.Height = reader.GetDouble(HeightIndex);
                        landscape.xActual = reader.GetDouble(xActualIndex);
                        if (!reader.IsDBNull(yActualIndex))
                            landscape.yActual = reader.GetDouble(yActualIndex);
                        else
                            landscape.yActual = 0;
                        landscape.ImageName = reader.GetString(ImageNameIndex);
                        if (!reader.IsDBNull(GroupIdIndex))
                            landscape.GroupId = reader.GetInt32(GroupIdIndex);
                        else
                            landscape.GroupId = 0;
                        if (!reader.IsDBNull(HomeObjectIndex))
                            landscape.HomeObject = reader.GetBoolean(HomeObjectIndex);
                        if (!reader.IsDBNull(NextFromHomeObjectIndex))
                            landscape.NextFromHomeObject = reader.GetBoolean(NextFromHomeObjectIndex);
                        if (!reader.IsDBNull(FeatureNoteIndex))
                            landscape.FeatureNote = reader.GetString(FeatureNoteIndex);
                        
                        LandscapeObjects.Add(landscape);
                        cLogger.Log("GetLandscapeObjectsAsync  " + landscape.Name + " x " + landscape.xActual + " y " + landscape.yActual);

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
            return LandscapeObjects;
        }
        public List<Int32> GetLandscapeObjectsGroupIds()
        {
            List<Int32> LandscapeObjectGroupIds = new List<Int32>();
            try
            {
                string sql = "select distinct groupid from  LandScapeObject";
                IDataReader reader = sqlhelper.GetReader(sql);
                Int32 GroupIdIndex = 0;

                while (reader.Read())
                {
                    try
                    {
                        LandscapeObjectGroupIds.Add(reader.GetInt32(GroupIdIndex));
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
            return LandscapeObjectGroupIds;
        }
        public async Task<LandscapeObjectEntity> GetLandscapeObjectByIdAsync(Int32 LandScapeObjectID)
        {
            LandscapeObjectEntity landscape = new LandscapeObjectEntity();
            try
            {
                string sql = "SELECT LandScapeObjectID,Name,Description,Height,Width,xActual,yActual,ImageName,GroupId,HomeObject,NextFromHomeObject,FeatureNote FROM LandScapeObject";
                sql += " where groupid = " + GlobalServices.LandscapeObjecGroupid + " and LandScapeObjectID = " + LandScapeObjectID;
                IDataReader reader = sqlhelper.GetReader(sql);
                Int32 LandScapeObjectIDIndex = 0;
                Int32 NameIndex = 1;
                Int32 DescriptionIndex = 2;
                Int32 HeightIndex = 3;
                Int32 WidthIndex = 4;
                Int32 xActualIndex = 5;
                Int32 yActualIndex = 6;
                Int32 ImageNameIndex = 7;
                Int32 GroupIdIndex = 8;
                Int32 HomeObjectIndex = 9;
                Int32 NextFromHomeObjectIndex = 10;
                Int32 FeatureNoteIndex = 11;

                while (reader.Read())
                {
                    try
                    {
                        landscape.Name = reader.GetString(NameIndex);

                        if (!reader.IsDBNull(LandScapeObjectIDIndex))
                            landscape.LandScapeObjectID = reader.GetInt32(LandScapeObjectIDIndex);
                        string desc = reader.GetString(DescriptionIndex);
                        landscape.Description = desc;
                        landscape.Width = reader.GetDouble(WidthIndex);
                        landscape.Height = reader.GetDouble(HeightIndex);
                        if (!reader.IsDBNull(xActualIndex))
                            landscape.xActual = reader.GetDouble(xActualIndex);
                        else
                            landscape.xActual = 0;
                        if (!reader.IsDBNull(yActualIndex))
                            landscape.yActual = reader.GetDouble(yActualIndex);
                        else
                            landscape.yActual = 0;
                        landscape.ImageName = reader.GetString(ImageNameIndex);
                        if (!reader.IsDBNull(GroupIdIndex))
                            landscape.GroupId = reader.GetInt32(GroupIdIndex);
                        else
                            landscape.GroupId = 0;
                        if (!reader.IsDBNull(HomeObjectIndex))
                            landscape.HomeObject = reader.GetBoolean(HomeObjectIndex);
                        if (!reader.IsDBNull(NextFromHomeObjectIndex))
                            landscape.NextFromHomeObject = reader.GetBoolean(NextFromHomeObjectIndex);
                        if (!reader.IsDBNull(FeatureNoteIndex))
                            landscape.FeatureNote = reader.GetString(FeatureNoteIndex);
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
            return landscape;
        }
        public async void Upsert(LandscapeObjectEntity landscape)
        {
            cLogger.Log("LandscapeObjectEntity Upsert " + landscape.Name + " x " + landscape.xActual + " y " + landscape.yActual);

            bool result = false;
            LandscapeObjectEntity LandscapeFound = await GetLandscapeObjectByIdAsync(landscape.LandScapeObjectID);
            var connection = adoNetHelper.GetConnection();
            if (LandscapeFound != null && LandscapeFound.Name != null && LandscapeFound.Name != "")
            {
                try
                {
                    string sqlraw = "Update LandScapeObject ";
                    sqlraw += "Set Name = " + "'" + landscape.Name + "', ";
                    sqlraw += "Description = '" + landscape.Description + "', ";
                    sqlraw += "Height = " + landscape.Height + ", ";
                    sqlraw += "Width = "  + landscape.Width + ", ";
                    sqlraw += "xActual = " + "" + landscape.xActual + ", ";
                    sqlraw += "yActual = " + "" + landscape.yActual + ", ";
                     sqlraw += "ImageName = " + "'" + landscape.ImageName + "', ";
                    sqlraw += "GroupId = " + "" + landscape.GroupId + ", ";
                    sqlraw += "HomeObject = " + "" + landscape.HomeObject + ", ";
                    sqlraw += "FeatureNote = " + "'" + landscape.FeatureNote + "', ";
                    sqlraw += "NextFromHomeObject = " + "" + landscape.NextFromHomeObject + " ";
                    sqlraw += "where LandScapeObjectID = " + landscape.LandScapeObjectID;

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
                    string sqlraw = "INSERT INTO LandScapeObject ( Name,Description,ImageName,Height,Width,xActual,yActual,GroupId,HomeObject,NextFromHomeObject,FeatureNote) ";
                    sqlraw += "VALUES('" + landscape.Name + "',";
                    sqlraw += ",'" + landscape.Description + "','" + landscape.ImageName + "'," + landscape.Height + "," + landscape.Width + "," + landscape.xActual;
                    sqlraw += "," + landscape.xActual + "," + landscape.yActual + "," + landscape.GroupId + "," + landscape.HomeObject + "," + landscape.NextFromHomeObject + "'" + landscape.FeatureNote + "')";
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

        public void Delete(LandscapeObjectEntity entity)
        {
            string sql = "Delete from LandScapeObject ";
            sql += " where LandScapeObjectID = " + entity.LandScapeObjectID;
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
