using LoginFinal.HelpingClasses;
using LoginFinal.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginFinal.DbSeed
{
    public class AppDbInitializer
    {

        public static async void DbSeed(IApplicationBuilder applicationBuilder)
        {


            using (var serviceScope = applicationBuilder.ApplicationServices.CreateScope())
            {
                var context = serviceScope.ServiceProvider.GetService<AppDbContext>();

                context.Database.EnsureCreated();

                if (!context.Users.Any())
                {
                    User obj = new User()
                    {
                        FirstName = "Usman",
                        LastName = "Ali",
                        Username = "usman",
                        Contact = "0000-0000000",
                        Email = "usman78056@gmail.com",
                        Password = StringCipher.Encrypt("123"),
                        Role = 1,
                        IsActive = 1,
                        CreatedAt = GeneralPurpose.DateTimeNow(),
                        Refferal_Code= "usman1",
                        Country= "Canada"
                    };

                    var usr = new IdentityUser()
                    {
                        UserName= obj.Username,
                        Email = obj.Email,

                    };

                    User obj2 = new User()
                    {
                        FirstName = "Micheal",
                        LastName = "Micheal",
                        Username = "micheal",
                        Contact = "0000-0000000",
                        Email = "micheal@gmail.com",
                        Country = "Canada",
                        Password = StringCipher.Encrypt("123"),
                        Role = 4,
                        IsActive = 1,
                        Refferal_Code = "micheal2",
                        CreatedAt = GeneralPurpose.DateTimeNow()
                    };

                    var usr2 = new IdentityUser()
                    {
                        UserName = obj2.Username,
                        Email = obj2.Email,

                    };

                    User obj3 = new User()
                    {
                        FirstName = "ian",
                        LastName = "ian",
                        Username = "ian",
                        Contact = "0000-0000000",
                        Email = "ian@gmail.com",
                        Password = StringCipher.Encrypt("123"),
                        Role = 3,
                        IsActive = 1,
                        Refferal_Code = "ian3",
                        CreatedAt = GeneralPurpose.DateTimeNow(),
                        Country = "Canada"
                    };

                    var usr3 = new IdentityUser()
                    {
                        UserName = obj3.Username,
                        Email = obj3.Email,

                    };

                    context.Users.Add(obj);

                 

                    context.Users.Add(obj2);

                 

                    context.Users.Add(obj3);


                    context.SaveChanges();
                }

            }

        }

    }
}
