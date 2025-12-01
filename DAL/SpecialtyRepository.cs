using Models;

using MySql.Data.MySqlClient;

using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace DAL
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        public async Task AssignToPersonAsync(int personId, int specialtyId)
        {
            const string sql = "INSERT IGNORE INTO person_specialties (PersonId, SpecialtyId) VALUES (@personId}, @specialtyId);";

            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@personId", personId);
            cmd.Parameters.AddWithValue("@specialtyId", specialtyId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteAsync(int id)
        {
            const string sql = "DELETE FROM specialties WHERE Id = @specialtyId;";
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@specialtyId", id);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task<Specialty?> GetByIdAsync(int id)
        {
            Specialty? specialty = null;
            const string sql = "SELECT Id, Name FROM specialties WHERE Id = @specialtyId;";
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@specialtyId", id);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                specialty = new Specialty
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                };
            }
            return specialty;
        }

        public async Task<int> InsertAsync(Specialty specialty)
        {
            var sql = new StringBuilder();
            sql.Append("INSERT INTO specialties (Name) VALUES (@name);"); 
            sql.Append("SELECT LAST_INSERT_ID();");
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql.ToString(), conn);
            cmd.Parameters.AddWithValue("@name", specialty.Name);
            var result = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(result);
        }

        public async Task<List<Specialty>> ListAllAsync()
        {
            var list = new List<Specialty>();
            const string sql = "SELECT Id, Name FROM specialties ORDER BY Name;";
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                var specialty = new Specialty
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                };
                list.Add(specialty);
            }
            return list;
        }

        public async Task<List<Specialty>> ListForPersonAsync(int personId)
        {
            var list = new List<Specialty>();
            const string sql = @"
                SELECT s.Id, s.Name
                FROM specialties s
                INNER JOIN person_specialties ps ON s.Id = ps.SpecialtyId
                WHERE ps.PersonId = @personId
                ORDER BY s.Name;";

            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@personId", personId);
            await using var reader = await cmd.ExecuteReaderAsync();
            while(await reader.ReadAsync())
            {
                var specialty = new Specialty
                {
                    Id = reader.GetInt32("Id"),
                    Name = reader.GetString("Name")
                };
                list.Add(specialty);
            }
            return list;
        }

        public async Task RemoveFromPersonAsync(int personId, int specialtyId)
        {
            const string sql = "DELETE FROM person_specialties WHERE PersonId = @personId AND SpecialtyId = @specialtyId;";
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@personId", personId);
            cmd.Parameters.AddWithValue("@specialtyId", specialtyId);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task UpdateAsync(Specialty specialty)
        {
            const string sql = "UPDATE specialties SET Name = @name WHERE Id = @specialtyId;";
            await using var conn = new MySqlConnection(Config.DbConnectionString);
            await conn.OpenAsync();
            var cmd = new MySqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@name", specialty.Name);
            cmd.Parameters.AddWithValue("@specialtyId", specialty.Id);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
