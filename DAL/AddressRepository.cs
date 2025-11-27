using System.Data;
using System.Text;
using Models;
using MySql.Data.MySqlClient;

namespace DAL;

public class AddressRepository : IAddressRepository
{
    public async Task<Address> GetForPersonIdAsync(int personId)
    {
        var address = new Address();
        
        var sql = new StringBuilder();
        sql.AppendLine("SELECT * FROM addresses");
        sql.AppendLine("WHERE PersonId = @personId");

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();
            
            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("personId", personId);
            
            var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                address = PopulateAddress(reader);
            }
        }

        return address;
    }

    public async Task<int> InsertAsync(Address address)
    {
        var sql = new StringBuilder();
        sql.AppendLine("INSERT INTO addresses (PersonId, Line1, City, Postcode)");
        sql.AppendLine("VALUES (@personId, @line1, @city, @postcode);");
        sql.AppendLine("SELECT LAST_INSERT_ID();");

        int newId;

        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("personId", address.PersonId);
            command.Parameters.AddWithValue("line1", address.Line1);
            command.Parameters.AddWithValue("city", address.City);
            command.Parameters.AddWithValue("postcode", address.Postcode);

            var result = await command.ExecuteScalarAsync();
            newId = Convert.ToInt32(result);
        }

        return newId;
    }

    public async Task SaveAsync(Address address)
    {
        var sql = new StringBuilder();
        sql.AppendLine("UPDATE addresses SET");
        sql.AppendLine("Line1 = @line1,");
        sql.AppendLine("City = @city,");
        sql.AppendLine("Postcode = @postcode");
        sql.AppendLine("WHERE Id = @addressId");
        
        await using (var connection = new MySqlConnection(Config.DbConnectionString))
        {
            await connection.OpenAsync();

            var command = new MySqlCommand(sql.ToString(), connection);
            command.Parameters.AddWithValue("line1", address.Line1);
            command.Parameters.AddWithValue("city", address.City);
            command.Parameters.AddWithValue("postcode", address.Postcode);
            command.Parameters.AddWithValue("addressId", address.Id);

            await command.ExecuteNonQueryAsync();
        }
    }

    private Address PopulateAddress(IDataRecord data)
    {
        var idValue = data["Id"]?.ToString();
        var personIdValue = data["PersonId"]?.ToString();

        var address = new Address
        {
            Id = !string.IsNullOrEmpty(idValue) ? int.Parse(idValue) : 0,
            PersonId = !string.IsNullOrEmpty(personIdValue) ? int.Parse(personIdValue) : 0,
            Line1 = data["Line1"]?.ToString() ?? string.Empty,
            City = data["City"]?.ToString() ?? string.Empty,
            Postcode = data["Postcode"]?.ToString() ?? string.Empty
        };
        return address;
    }
}