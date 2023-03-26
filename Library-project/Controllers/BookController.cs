﻿using Library_project.Data.Enums;
using Library_project.Data.Objects;
using Library_project.Models;
using Library_project.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Npgsql;

namespace Library_project.Controllers
{
    public class BookController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _config;

        public BookController(ILogger<HomeController> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public async Task<IActionResult> Index()
        {

            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config["ConnectionString"]);
            dataSourceBuilder.MapEnum<genres>();
            dataSourceBuilder.MapComposite<Location>();
            await using var dataSource = dataSourceBuilder.Build();
            await using var command = dataSource.CreateCommand("SELECT * FROM book ,media WHERE bookId=media.mediaId");
            await using var reader = await command.ExecuteReaderAsync();

            var bookList = new ListBookViewModel();
            var LocalList = new List<book>();
            while (await reader.ReadAsync())
            {
                LocalList.Add(  new book()
                {
                    bookId = (int)reader["bookId"],
                    title = reader["title"] as string,
                    mediaId = (int)reader["bookId"],
                    isavailable = (bool)reader["isAvailable"],
                    isbn = (long)reader["isbn"],
                    pageCount = (int)reader["pageCount"],
                    publicDate = reader.GetFieldValue<DateOnly>(3),
                    author = reader.GetFieldValue <string[]>(2),
                    genres = reader.GetFieldValue<int>(8),
                    location = reader.GetFieldValue<Location>(10)
                });

                bookList.allBooks = LocalList;
            }


            return View(bookList);

        }
        public IActionResult BookForm()
        {
            return View();
        }
        [HttpGet]
        public IActionResult CreateBookView()
        {
            var newBook = new CreateBookViewModel();

            return View(newBook);
        }
        [HttpPost]
        public async Task<IActionResult> CreateBookLandingPage(CreateBookViewModel newBook)
        {
            CreateBookViewModel example = new CreateBookViewModel();
            example.title = newBook.title;
            example.author = newBook.author;
            example.pageCount = newBook.pageCount;
            example.isbn = newBook.isbn;
            example.isAvailable = newBook.isAvailable;
            example.publishDate = newBook.publishDate;
            example.genre = newBook.genre;

            if (ModelState.IsValid)
            {
                await using NpgsqlConnection conn = new NpgsqlConnection(_config["ConnectionString"]);

                // Connect to the database
                await conn.OpenAsync();

                await using var command = new NpgsqlCommand("WITH local_id AS (INSERT INTO media VALUES (DEFAULT,(1,1)) RETURNING mediaid)" +
                    "INSERT INTO book " +
                    "(bookid," +
                    "title,author,publicdate,pagecount,isbn,isavailable,genre,mediaid)" +
                    " VALUES((SELECT mediaid from local_id) , @title, @author, @publicDate, " +
                    "@pageCount,@isbn,@isAvailable,@genre,(SELECT mediaid from local_id))", conn)
                {
                    Parameters =
                        {
                            new("title", newBook.title),
                            new("author", newBook.author.Split(',')),
                            new("genre", newBook.genre),
                            new("publicDate", newBook.publishDate),
                            new("pageCount", newBook.pageCount),
                            new("isbn", newBook.isbn),
                            new("isAvailable", newBook.isAvailable),
                        }
                };
                await using var reader = await command.ExecuteReaderAsync();
            }
            else
            {

                example.title="fake";

            }
            return View(example);
        }  
            
        public async Task<IActionResult> Edit(int book_id)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config["ConnectionString"]);

            await using var dataSource = dataSourceBuilder.Build();
            await using var command = dataSource.CreateCommand("SELECT * FROM books");
            await using var reader = await command.ExecuteReaderAsync();




            var localBook = new EditBookViewModel();
            reader.Read();
            using var innerRead = reader.GetData(0);

            while(innerRead.Read())
            {
                localBook.title = innerRead.GetFieldValue<string>(1);
                localBook.author = innerRead.GetFieldValue<string[]>(2);
                localBook.genres = innerRead.GetFieldValue<genres[]>(3);
                localBook.publicDate = innerRead.GetFieldValue<DateOnly>(4);
                localBook.pageCount = innerRead.GetFieldValue<int>(4);
                localBook.isbn = innerRead.GetFieldValue<int>(6);
                localBook.isAvailable = innerRead.GetFieldValue<bool>(7);
            }

            return View(localBook);
            
            
        }
        public async Task<IActionResult> Edit(EditBookViewModel editBookvm)
        {

            return RedirectToAction("Index");
        }
    }
}
