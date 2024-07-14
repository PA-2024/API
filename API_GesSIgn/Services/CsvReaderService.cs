using CsvHelper;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models;
using System.Globalization;
using System.Net.Http;

namespace API_GesSIgn.Services
{
    public class CsvReaderService
    {
        private readonly MonDbContext _context;
        private readonly HttpClient _httpClient;

        public CsvReaderService(MonDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        public async Task<(bool isSuccess, string message)> ImportUsersFromCsvAsync(string url, int id_school)
        {
            var csvContent = await DownloadCsvAsync(url);
            if (string.IsNullOrEmpty(csvContent))
            {
                return (false, "Impossible de télécharger le fichier CSV.");
            }

            var usersToAdd = new List<Student>();

            try
            {
                using (var reader = new StringReader(csvContent))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<UserCsvModel>().ToList();
                    var role = await _context.Roles.FirstOrDefaultAsync(r => r.Role_Name == "Eleve");

                    foreach (var record in records)
                    {
                        var classe = await _context.Sectors.FirstOrDefaultAsync(c => c.Sectors_Name == record.ClassName);

                        if (classe == null)
                        {
                            return (false, $"Classe {record.ClassName} non trouvée.");
                        }

                        var user = new User
                        {
                            User_email = record.User_email,
                            User_password = GeneratePassword(),
                            User_lastname = record.User_lastname,
                            User_firstname = record.User_firstname,
                            User_num = record.User_num,
                            User_Role = role,
                            User_School_Id = id_school
                        };

                        var newStudent = new Student
                        {
                            Student_User = user,
                            Student_Sectors = classe
                        };
                        usersToAdd.Add(newStudent);
                    }
                }

                _context.Students.AddRange(usersToAdd);
                await _context.SaveChangesAsync();

                return (true, $"Fichier importé avec succès. Utilisateurs ajoutés : {usersToAdd.Count}");
            }
            catch (Exception ex)
            {
                return (false, $"Une erreur est survenue lors de l'importation : {ex.Message}");
            }
        }

        private async Task<string> DownloadCsvAsync(string url)
        {
            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch
            {
                return null;
            }
        }

        private string GeneratePassword()
        {
            return Guid.NewGuid().ToString();
        }
    }

    public class UserCsvModel
    {
        public string User_email { get; set; }
        public string User_lastname { get; set; }
        public string User_firstname { get; set; }
        public string User_num { get; set; }
        public string ClassName { get; set; }
    }
}
