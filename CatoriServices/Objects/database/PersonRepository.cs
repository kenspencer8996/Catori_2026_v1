using CatoriServices;
using CityAppServices.Objects.Entities;
using Dapper;
using Microsoft.Data.Sqlite;
using static System.Net.Mime.MediaTypeNames;

namespace CityAppServices.Objects.database
{
    public class PersonRepository
    {
        AdoNetHelper adoNetHelper = new AdoNetHelper();

        public async Task<List<PersonEntity>> GetPersonsAsync()
        {
            List<PersonEntity> Persons = new List<PersonEntity>();
            string connectionString = "Data Source=" + GlobalServices.Database + " ;";

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();
                string selectQuery = "SELECT PersonID,Name,ImagesFolder,PersonRole,IsUser,Funds,Active,FileNameOptional FROM Person;";
                using (SqliteCommand command = new SqliteCommand(selectQuery, connection))
                {
                    using (SqliteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PersonEntity person = new PersonEntity();
                            try
                            {
                                if (reader["PersonID"] != DBNull.Value)
                                    person.PersonId = Convert.ToInt32(reader["PersonID"]);
                                else
                                    person.PersonId = 0;
                                if (reader["active"] != DBNull.Value)
                                    person.Active = Convert.ToBoolean(reader["active"]);
                                else
                                    person.Active = true;
                                person.Name = reader["Name"].ToString();
                                person.ImagesFolder = reader["ImagesFolder"].ToString();
                                string isuserstring = reader["IsUser"].ToString();
                                if (isuserstring == "1")
                                    person.IsUser = true;
                                else
                                    person.IsUser = false;
                                string fundsstring = reader["Funds"].ToString();
                                if (string.IsNullOrEmpty(fundsstring))
                                    fundsstring = "0";  
                                person.Funds = Convert.ToDecimal(fundsstring);
                                person.FileNameOptional = reader["FileNameOptional"].ToString();
                                person.PersonRole = Convertors.GetPersonEnum(reader["PersonRole"].ToString());
                                Persons.Add(person);

                            }
                            catch (Exception ex)
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            //    try
            //{
            //    var connection = adoNetHelper.GetConnection();
            //    var sql = "SELECT * FROM Person";
            //    var results = await connection.QueryAsync<PersonEntity>(sql);
            //    if (results != null && results.Any())
            //    {
            //        Persons = results.ToList();
            //    }
            //}
            //catch (Exception ex)
            //{

            //    throw;
            //}
            return Persons;
        }
            
        public async Task<PersonEntity> GetPersonbyNameAsync(string name)
        {
            List<PersonEntity> Persons = new List<PersonEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT * FROM Person where name  = " + "'" + name + "' ";
                var results = await connection.QueryAsync<PersonEntity>(sql);
                if (results != null && results.Any())
                {
                    Persons = results.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            PersonEntity personout = new PersonEntity();
            if (Persons.Any())
                personout = Persons.FirstOrDefault();
            return personout;
        }
        public async Task<PersonEntity> GetPersonbyIdAsync(int Id)
        {
            List<PersonEntity> Persons = new List<PersonEntity>();
            try
            {
                var connection = adoNetHelper.GetConnection();
                var sql = "SELECT * FROM Person where personid  = " + "" + Id + " ";
                var results = await connection.QueryAsync<PersonEntity>(sql);
                if (results != null && results.Any())
                {
                    Persons = results.ToList();
                }
            }
            catch (Exception ex)
            {

                throw;
            }
            PersonEntity personout = new PersonEntity();
            if (Persons.Any())
                personout = Persons.FirstOrDefault();
            return personout;
        }

        public async Task<PersonEntity> UpsertPerson(PersonEntity person)
        {
            bool result = false;
            PersonEntity personfound = await GetPersonbyNameAsync(person.Name);
            var connection = adoNetHelper.GetConnection();
            int isuser = 0;
            if (person.IsUser)
            {
                isuser = 1;
            }
            if (personfound != null && personfound.Name != "")
            {
                string sqlraw = "";
                try
                {
                    string role = person.PersonRole.ToString();
                    sqlraw = "Update Person ";
                    sqlraw += "Set Name = " + "'" + person.Name + "', ";
                    sqlraw += "IsUser = " + isuser + ", ";
                    sqlraw += "Funds = " + person.Funds + ", ";
                    sqlraw += "PersonRole = " + "'" + role + "', ";
                    sqlraw += "ImagesFolder = " + "'" + person.ImagesFolder + "', ";
                    sqlraw += "FileNameOptional =  " + "'" + person.FileNameOptional + "',";
                    sqlraw += "Active = " + person.Active + "  ";
                    sqlraw += "where PersonId = " + person.PersonId;
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
                    string role = person.PersonRole.ToString();
                    string sqlraw = "INSERT INTO person ([Name],[IsUser] ,[Funds],[PersonRole],[ImagesFolder],[Active],[FileNameOptional],Funds) ";
                    sqlraw+= "VALUES('" + person.Name + "'," + isuser + ",0,'" + role + "'," + person.ImagesFolder + "," + person.Active + "'" + person.FileNameOptional + "'," + person.Funds  + " )"; 
                    var command = new SqliteCommand();
                    command.Connection = connection;
                    command.CommandText = sqlraw;
                    command.ExecuteNonQuery();
                    //get person and return
                    person = await GetPersonbyNameAsync(person.Name);

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            return person;
        }
    }
}
