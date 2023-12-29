﻿using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services
{
    public class SeedService(
        UserService userService,
        CatService catService,
        CatShowService catShowService
        )
    {
        public async Task<List<User>> SeedUsers()
        {
            var userData = new List<UserCreatePayloadDTO>
            {
                new()
                {
                    MailNickname = "johndoe",
                    GivenName = "John",
                    Surname = "Doe",
                    DisplayName = "John Doe",
                    Password = "John1234!",
                    Email = "john.doe@example.com"
                },
                new()
                {
                    MailNickname = "janedoe",
                    GivenName = "Jane",
                    Surname = "Doe",
                    DisplayName = "Jane Doe",
                    Password = "Jane1234!",
                    Email = "jane.doe@example.com"
                },
                new()
                {
                    MailNickname = "alexsmith",
                    GivenName = "Alex",
                    Surname = "Smith",
                    DisplayName = "Alex Smith",
                    Password = "Alex1234!",
                    Email = "alex.smith@example.com"
                }
            };

            var existingUsers = await userService.GetUsers();

            var usersToCreate = userData
                .Where(
                    newUser =>
                        !existingUsers.Any(existingUser => existingUser.Email == newUser.Email)
                )
                .ToList();

            var createdUsers = new List<User>();

            foreach (var userPayload in usersToCreate)
            {
                var createdUser = await userService.CreateUser(userPayload);
                createdUsers.Add(createdUser);
            }

            return createdUsers;
        }



        public async Task SeedCatShows()
        {
            var catShowData = new List<CatShow>
            {
                new()
                {
                    Name = "Kissakilpailu 2021",
                    Location = "Helsinki",
                   StartDate = new DateTime(2021, 1, 1),
                    EndDate = new DateTime(2021, 1, 2),
                    Description = "Kissakilpailu 2021",

                },
                new()
                {
                    Name = "Kissakilpailu 2022",
                    Location = "Helsinki",
                    StartDate = new DateTime(2022, 1, 1),
                    EndDate = new DateTime(2022, 1, 2),
                    Description = "Kissakilpailu 2022",

                },
                new()
                {
                    Name = "Kissakilpailu 2023",
                    Location = "Helsinki",
                    StartDate = new DateTime(2023, 1, 1),
                    EndDate = new DateTime(2023, 1, 2),
                    Description = "Kissakilpailu 2023",

                }
            };

            foreach (CatShow catShow in catShowData)
            {
                await catShowService.CreateCatShow(catShow);
            }
        }

        public async Task<List<Cat>> SeedCats()
        {
            var users = await userService.GetUsers();
            var random = new Random();
            var catData = new List<CatRequest>
            {
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Misse",
                    Breed = "Persian",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "male",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                },
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Mouru",
                    Breed = "Maine Coon",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "female",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                },
                new()
                {
                    OwnerId = users[random.Next(users.Count)].Id,
                    BreederId = users[random.Next(users.Count)].Id,
                    Name = "Mauku",
                    Breed = "Maine Coon",
                    BirthDate = new DateTime(2019, 1, 1),
                    Sex = "female",
                    ImageUrl =
                        "https://upload.wikimedia.org/wikipedia/commons/thumb/3/3a/Cat03.jpg/1200px-Cat03.jpg"
                }
            };

            var existingCats = await catService.GetCatsAsync();

            var catsToCreate = catData
                .Where(
                    newCat =>
                        !existingCats.Any(
                            existingCat =>
                                existingCat.Name.Equals(
                                    newCat.Name,
                                    StringComparison.OrdinalIgnoreCase
                                )
                                && existingCat.Breed.Equals(
                                    newCat.Breed,
                                    StringComparison.OrdinalIgnoreCase
                                )
                        )
                )
                .ToList();

            var createdCats = new List<Cat>();
            foreach (var catRequest in catsToCreate)
            {
                var createdCat = await catService.CreateCat(catRequest);
                createdCats.Add(createdCat);
            }

            return createdCats;
        }
    }
}