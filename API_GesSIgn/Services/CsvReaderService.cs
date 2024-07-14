using CsvHelper;
using Microsoft.EntityFrameworkCore;
using API_GesSIgn.Models;
using System.Globalization;
using Microsoft.CodeAnalysis.Elfie.Serialization;

namespace API_GesSIgn.Services
{
    public class CsvReaderService
    {
        private readonly MonDbContext _context;

        public CsvReaderService(MonDbContext context)
        {
            _context = context;
        }

        public async Task<(bool isSuccess, string message)> ImportUsersFromCsvAsync(string filePath, int id_school)
        {
            if (!System.IO.File.Exists(filePath))
            {
                return (false, "Fichier introuvable.");
            }

            var usersToAdd = new List<Student>();

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvHelper.CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    var records = csv.GetRecords<UserCsvModel>().ToList();
                    var role = _context.Roles.FirstOrDefault(r => r.Role_Name == "Eleve");


                    foreach (var record in records)
                    {
                        var classe = _context.Sectors.FirstOrDefault(c => c.Sectors_Name == record.ClassName);

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
