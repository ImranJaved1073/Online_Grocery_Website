﻿using Microsoft.Data.SqlClient;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Ecommerce.Models
{
    public class GenericRepository<TEntity> : IRepository<TEntity>
    {
        private readonly string connectionString;

        public GenericRepository(string c)
        {
            connectionString = c;
        }

        public void Add(TEntity entity)
        {
            var tablename = typeof(TEntity).Name;

            var properties =
                typeof(TEntity).GetProperties().Where(p => p.Name != "Id" && p.GetCustomAttribute<NotMappedAttribute>() == null);
            var columnNames = string.Join(",", properties.Select(x => x.Name));
            var parameterName =
                string.Join(",", properties.Select(y => "@" + y.Name));

            var query = $"insert into {tablename} ({columnNames}) values({parameterName}) ";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var comm = new SqlCommand(query, connection);
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity) ?? DBNull.Value;
                    comm.Parameters.AddWithValue("@" + prop.Name, value);
                }
                comm.ExecuteNonQuery();
            }
        }

        public void Update(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var properties = typeof(TEntity).GetProperties().Where(x => x.GetCustomAttribute<NotMappedAttribute>() == null);
            var propertiesExclude = typeof(TEntity).GetProperties().Where(x => x.Name != primaryKey && x.GetCustomAttribute<NotMappedAttribute>() == null);

            var setClause = string.Join(",", propertiesExclude.Select(a => $"{a.Name}=@{a.Name}"));
            //var primaryKeyProp = properties.FirstOrDefault(x => x.Name == primaryKey);

            var query = $"update {tableName} set {setClause} where {primaryKey}=@{primaryKey} ";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var comm = new SqlCommand(query, connection);
                foreach (var prop in properties)
                {
                    var value = prop.GetValue(entity) ?? DBNull.Value;
                    comm.Parameters.AddWithValue("@" + prop.Name, value);
                }
                comm.ExecuteNonQuery();
            }
        }

        public void Delete(TEntity entity)
        {
            var tableName = typeof(TEntity).Name;
            var primaryKey = "Id";
            var query = $"delete from {tableName} where {primaryKey}=@{primaryKey}";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var comm = new SqlCommand(query, connection);
                comm.Parameters.AddWithValue("@" + primaryKey, entity.GetType().GetProperty(primaryKey).GetValue(entity));
                comm.ExecuteNonQuery();
            }
        }
        

        public TEntity? Get(int id)
        {
            var tablename = typeof(TEntity).Name;

            var query = $"select * from {tablename} where id = @id";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                cmd.Parameters.AddWithValue("@id", id);
                var reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    var properties = typeof(TEntity).GetProperties()
                                                     .Where(p => p.GetCustomAttribute<NotMappedAttribute>() == null);

                    foreach (var prop in properties)
                    {
                        var value = reader[prop.Name];
                        if (value == DBNull.Value)
                        {
                            // Handle null values based on the property type
                            if (prop.PropertyType == typeof(string))
                            {
                                prop.SetValue(entity, string.Empty);
                            }
                            else
                            {
                                prop.SetValue(entity, Activator.CreateInstance(prop.PropertyType));
                            }
                        }
                        else
                        {
                            prop.SetValue(entity, value);
                        }
                    }
                    return entity;
                }
            }
            return default;
        }

        public List<TEntity> Get()
        {
            var tablename = typeof(TEntity).Name;

            var query = $"select * from {tablename}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(query, connection);
                var reader = cmd.ExecuteReader();
                var entities = new List<TEntity>();
                while (reader.Read())
                {
                    var entity = Activator.CreateInstance<TEntity>();
                    foreach (var prop in typeof(TEntity).GetProperties())
                    {
                        prop.SetValue(entity, reader[prop.Name]);
                    }
                    entities.Add(entity);
                }
                return entities;
            }
        }

        virtual public List<TEntity> Search(string search)
        {
            List<TEntity> entities = new List<TEntity>();
            var entityType = typeof(TEntity);
            var properties = entityType.GetProperties();

            var tableName = entityType.Name;

            //name LIKE @search OR category LIKE @search"; make a string cloumn generi like this
            var whereClause = string.Join(" OR ", properties.Select(x => $"{x.Name} LIKE @search"));
            var query = $"select * from {tableName} where {whereClause}";

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.Parameters.AddWithValue("@search", $"%{search}%");
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var entity = Activator.CreateInstance<TEntity>();
                        foreach (var prop in properties)
                        {
                            prop.SetValue(entity, reader[prop.Name]);
                        }
                        entities.Add(entity);
                    }
                }
            }
            return entities;
        }
    }
}
