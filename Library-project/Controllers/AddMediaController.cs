﻿using Library_project.ViewModels.Book;
using Library_project.ViewModels.Camera;
using Library_project.ViewModels.Computer;
using Library_project.ViewModels.Journal;
using Library_project.ViewModels.Movie;
using Library_project.ViewModels.Projector;
using Microsoft.AspNetCore.Mvc;
using Npgsql;

namespace Library_project.Controllers
{
    public class AddMediaController : Controller
    {
        private readonly IConfiguration _config;

        public AddMediaController(IConfiguration config)
        {
            _config = config;
        }

        //Cameras
        public IActionResult AddCamera()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddCamera(CameraViewModel model)
        {
            if (model.brand == null)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var insertCommand = "WITH newid AS (INSERT INTO medias (mediaid) VALUES (default) RETURNING mediaid)" +
                        "INSERT INTO cameras (cameraid, serialnumber, brand, description, megapixels, availability, mediaid)";
                    using (var command = new NpgsqlCommand(insertCommand + " VALUES ((SELECT mediaid from newid), @s1, @b1, @d1, @m1, @a1, (SELECT mediaid from newid))", conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        command.Parameters.AddWithValue("m1", model.megapixels);
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditCameraForm(int cameraId)
        {
            CameraViewModel cam = new CameraViewModel();
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "SELECT * FROM cameras WHERE cameraid='" + cameraId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        cam.cameraid = reader.GetInt32(0);
                        cam.serialnumber = reader.GetString(1);
                        cam.brand = reader.GetString(2);
                        cam.description = reader.GetString(3);
                        cam.megapixels = reader.GetDouble(4);
                        cam.availability = reader.GetBoolean(5);
                    }
                    reader.Close();
                }
            }
            return View("~/Views/AddMedia/AddCamera.cshtml", cam);
        }

        [HttpPost]
        public IActionResult UpdateCamera(CameraViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE cameras SET serialnumber=@s1, brand=@b1, description=@d1, megapixels=@l1, availability=@a1 " +
                        "WHERE cameraid='" + model.cameraid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        command.Parameters.AddWithValue("l1", model.megapixels);
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddCamera.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteCamera(int cameraId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM cameras WHERE cameraId='" + cameraId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + cameraId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddCamera", "AddMedia") });
        }

        //Computers
        public IActionResult AddComputer()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddComputer(ComputerViewModel model)
        {
            if (model.brand == null)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var insertCommand = "WITH newid AS (INSERT INTO medias (mediaid) VALUES (default) RETURNING mediaid)" +
                        "INSERT INTO computers (computerid, serialnumber, brand, description, availability, mediaid)";
                    using (var command = new NpgsqlCommand(insertCommand + " VALUES ((SELECT mediaid from newid), @s1, @b1, @d1, @a1, (SELECT mediaid from newid))", conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditComputerForm(int computerId)
        {
            ComputerViewModel comp = new ComputerViewModel();
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "SELECT * FROM computers WHERE computerid='" + computerId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        comp.computerid = reader.GetInt32(0);
                        comp.serialnumber = reader.GetString(1);
                        comp.brand = reader.GetString(2);
                        comp.description = reader.GetString(3);
                        comp.availability = reader.GetBoolean(4);
                    }
                    reader.Close();
                }
            }
            return View("~/Views/AddMedia/AddComputer.cshtml", comp);
        }

        [HttpPost]
        public IActionResult UpdateComputer(ComputerViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE computers SET serialnumber=@s1, brand=@b1, description=@d1, availability=@a1 " +
                        "WHERE computerid='" + model.computerid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddComputer.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteComputer(int computerId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM computers WHERE computerid='" + computerId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + computerId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddComputer", "AddMedia") });
        }

        //Projectors
        public IActionResult AddProjector()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddProjector(ProjectorViewModel model)
        {
            if (model.brand == null)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var insertCommand = "WITH newid AS (INSERT INTO medias (mediaid) VALUES (default) RETURNING mediaid)" +
                        "INSERT INTO projectors (projectorid, serialnumber, brand, description, lumens, availability, mediaid)";
                    using (var command = new NpgsqlCommand(insertCommand + " VALUES ((SELECT mediaid from newid), @s1, @b1, @d1, @l1, @a1, (SELECT mediaid from newid))", conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        command.Parameters.AddWithValue("l1", model.lumens);
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditProjectorForm(int projectorId)
        {
            ProjectorViewModel proj = new ProjectorViewModel();
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "SELECT * FROM projectors WHERE projectorid='" + projectorId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {

                    var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        proj.projectorid = reader.GetInt32(0);
                        proj.serialnumber = reader.GetString(1);
                        proj.brand = reader.GetString(2);
                        proj.description = reader.GetString(3);
                        proj.lumens = reader.GetInt32(4);
                        proj.availability = reader.GetBoolean(5);
                    }
                    reader.Close();
                }
            }
            return View("~/Views/AddMedia/AddProjector.cshtml", proj);
        }

        [HttpPost]
        public IActionResult UpdateProjector(ProjectorViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE projectors SET serialnumber=@s1, brand=@b1, description=@d1, lumens=@l1, availability=@a1 " +
                        "WHERE projectorid='" + model.projectorid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("s1", model.serialnumber);
                        command.Parameters.AddWithValue("b1", model.brand);
                        if (model.description != null)
                            command.Parameters.AddWithValue("d1", model.description);
                        else
                            command.Parameters.AddWithValue("d1", "No description provided");
                        command.Parameters.AddWithValue("l1", model.lumens);
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddProjector.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteProjector(int projectorId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM projectors WHERE projectorid='" + projectorId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + projectorId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddProjector", "AddMedia") });
        }

        //Books
        public IActionResult AddBook()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddBook(EditBookViewModel model)
        {
            if (model.title == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_config["ConnectionString"]);

                // Connect to the database
                conn.Open();

                using var command = new NpgsqlCommand("WITH local_id AS (INSERT INTO medias VALUES (DEFAULT) RETURNING mediaid)" +
                    "INSERT INTO books " +
                    "(bookid, title, author, genres, publicdate, pagecount, isbn, isavailable, mediaid)" +
                    " VALUES((SELECT mediaid from local_id) , @title, @author, @genres, @publicDate, " +
                    "@pagecount, @isbn, @isavailable,(SELECT mediaid from local_id))", conn)
                {
                    Parameters =
                        {
                            new("title", model.title),
                            new("author", model.author),
                            new("genres", model.genres),
                            new("publicdate", DateOnly.Parse(model.publicDate)),
                            new("pagecount", model.pageCount),
                            new("isbn", model.isbn),
                            new("isavailable", model.isAvailable),
                        }
                };
                using var reader = command.ExecuteReader();
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditBookForm(int bookId)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config["ConnectionString"]);

            using var dataSource = dataSourceBuilder.Build();
            using var command = dataSource.CreateCommand("SELECT * FROM books WHERE bookid='" + bookId.ToString() + "'");
            using var reader = command.ExecuteReader();

            var book = new EditBookViewModel();
            while (reader.Read())
            {
                book.bookid = reader.GetInt32(0);
                book.title = reader.GetFieldValue<string>(1);
                book.author = reader.GetFieldValue<string>(2);
                book.genres = reader.GetFieldValue<int>(3);
                book.publicDate = reader.GetFieldValue<DateOnly>(4).ToString("yyyy-MM-dd");
                book.pageCount = reader.GetFieldValue<int>(4);
                book.isbn = reader.GetFieldValue<long>(6);
                book.isAvailable = reader.GetFieldValue<bool>(7);
            }

            return View("~/Views/AddMedia/AddBook.cshtml", book);
        }

        [HttpPost]
        public IActionResult UpdateBook(EditBookViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE books SET title=@t1, author=@a1, genres=@g1, publicdate=@p1, pagecount=@p2, isbn=@i1, isavailable=@a2 " +
                        "WHERE bookid='" + model.bookid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("t1", model.title);
                        command.Parameters.AddWithValue("a1", model.author);
                        command.Parameters.AddWithValue("g1", model.genres);
                        command.Parameters.AddWithValue("p1", DateOnly.Parse(model.publicDate));
                        command.Parameters.AddWithValue("p2", model.pageCount);
                        command.Parameters.AddWithValue("i1", model.isbn);
                        if (model.isAvailable == null)
                            command.Parameters.AddWithValue("a2", false);
                        else
                            command.Parameters.AddWithValue("a2", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddBook.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteBook(int bookId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM books WHERE bookid='" + bookId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + bookId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddBook", "AddMedia") });
        }

        //Journals
        public IActionResult AddJournal()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddJournal(CreateJournalViewModel model)
        {
            if (model.title == null)
            {
                return View();
            }
            if (ModelState.IsValid)
            {
                using NpgsqlConnection conn = new NpgsqlConnection(_config["ConnectionString"]);
                conn.Open();

                using var command = new NpgsqlCommand("WITH local_id AS (INSERT INTO medias VALUES (DEFAULT) RETURNING mediaid) " +
                            "INSERT INTO journals (journalid, mediaid, title, researchers, subject, length, releasedate, isavailable) VALUES(" +
                            "(SELECT mediaid from local_id), " +
                            "(SELECT mediaid from local_id), " +
                            "@Title, " +
                            "@Researchers, " +
                            "@Subject, " +
                            "@Length, " +
                            "@DateReleased, " +
                            "@IsAvailable)", conn)
                {
                    Parameters =
                        {
                           new("Title", model.title),
                           new("Researchers", model.researchers),
                           new("Subject", model.subject),
                           new("IsAvailable", model.isavailable),
                           new("DateReleased", DateOnly.Parse(model.releasedate)),
                           new("Length", model.length)
                        }

                };
                using var reader = command.ExecuteReader();
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditJournalForm(int journalId)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config["ConnectionString"]);

            using var dataSource = dataSourceBuilder.Build();
            using var command = dataSource.CreateCommand("SELECT * FROM journals WHERE journalid='" + journalId.ToString() + "'");
            using var reader = command.ExecuteReader();

            var journal = new CreateJournalViewModel();
            while (reader.Read())
            {
                journal.journalid = reader.GetInt32(0);
                journal.title = reader.GetFieldValue<string>(2);
                journal.researchers = reader.GetFieldValue<string>(3);
                journal.subject = reader.GetFieldValue<string>(4);
                journal.length = reader.GetFieldValue<int>(5);
                journal.releasedate = reader.GetFieldValue<DateOnly>(6).ToString("yyyy-MM-dd");
                journal.isavailable = reader.GetFieldValue<bool>(7);
            }

            return View("~/Views/AddMedia/AddJournal.cshtml", journal);
        }

        [HttpPost]
        public IActionResult UpdateJournal(CreateJournalViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE journals SET title=@t1, researchers=@r1, subject=@s1, length=@l1, releasedate=@r2, isavailable=@a1 " +
                        "WHERE journalid='" + model.journalid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("t1", model.title);
                        command.Parameters.AddWithValue("r1", model.researchers);
                        command.Parameters.AddWithValue("s1", model.subject);
                        command.Parameters.AddWithValue("l1", model.length);
                        command.Parameters.AddWithValue("r2", DateOnly.Parse(model.releasedate));
                        if (model.isavailable == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddJournal.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteJournal(int journalId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM journals WHERE journalid='" + journalId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + journalId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddJournal", "AddMedia") });
        }

        //Movies
        public IActionResult AddMovie()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddMovie(MovieViewModel model)
        {
            if (model.title == null)
            {
                return View();
            }

            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var insertCommand = "WITH newid AS (INSERT INTO medias (mediaid) VALUES (default) RETURNING mediaid)" +
                        "INSERT INTO movies (movieid, mediaid, rating, title, director, genres, length, releasedate, availability)";
                    using (var command = new NpgsqlCommand(insertCommand + " VALUES ((SELECT mediaid from newid), (SELECT mediaid from newid), @r1, @t1, @d1, @g1, @l1, @r2, @a1)", conn))
                    {
                        command.Parameters.AddWithValue("r1", model.rating);
                        command.Parameters.AddWithValue("t1", model.title);
                        command.Parameters.AddWithValue("d1", model.director);
                        command.Parameters.AddWithValue("g1", model.genres);
                        command.Parameters.AddWithValue("l1", model.length);
                        command.Parameters.AddWithValue("r2", DateOnly.Parse(model.releasedate));
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View();
        }

        [HttpGet]
        public IActionResult EditMovieForm(int movieId)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(_config["ConnectionString"]);

            using var dataSource = dataSourceBuilder.Build();
            using var command = dataSource.CreateCommand("SELECT * FROM movies WHERE movieid='" + movieId.ToString() + "'");
            using var reader = command.ExecuteReader();

            var movie = new MovieViewModel();
            while (reader.Read())
            {
                movie.movieid = reader.GetInt32(0);
                movie.rating = reader.GetFieldValue<int>(2);
                movie.title = reader.GetFieldValue<string>(3);
                movie.director = reader.GetFieldValue<string>(4);
                movie.genres = reader.GetFieldValue<int>(5);
                movie.length = reader.GetFieldValue<int>(6);
                movie.releasedate = reader.GetFieldValue<DateOnly>(7).ToString("yyyy-MM-dd");
                movie.availability = reader.GetFieldValue<bool>(8);
            }

            return View("~/Views/AddMedia/AddMovie.cshtml", movie);
        }

        [HttpPost]
        public IActionResult UpdateMovie(MovieViewModel model)
        {
            if (ModelState.IsValid)
            {
                using (var conn = new NpgsqlConnection(_config["ConnectionString"]))

                {
                    conn.Open();
                    var updateCommand = "UPDATE movies SET title=@t1, rating=@r1, director=@d1, genres=@g1, length=@l1, releasedate=@r2, availability=@a1 " +
                        "WHERE movieid='" + model.movieid + "'";
                    using (var command = new NpgsqlCommand(updateCommand, conn))
                    {
                        command.Parameters.AddWithValue("t1", model.title);
                        command.Parameters.AddWithValue("r1", model.rating);
                        command.Parameters.AddWithValue("d1", model.director);
                        command.Parameters.AddWithValue("g1", model.genres);
                        command.Parameters.AddWithValue("l1", model.length);
                        command.Parameters.AddWithValue("r2", DateOnly.Parse(model.releasedate));
                        if (model.availability == null)
                            command.Parameters.AddWithValue("a1", false);
                        else
                            command.Parameters.AddWithValue("a1", true);

                        int nRows = command.ExecuteNonQuery();
                    }
                }
            }
            return View("~/Views/AddMedia/AddMovie.cshtml");
        }

        [HttpDelete]
        public IActionResult DeleteMovie(int movieId)
        {
            using (var conn = new NpgsqlConnection(_config["ConnectionString"]))
            {

                Console.Out.WriteLine("Opening connection");
                conn.Open();

                var sqlCommand = "DELETE FROM movies WHERE movieid='" + movieId.ToString() + "';";
                sqlCommand += "DELETE FROM medias WHERE mediaid='" + movieId.ToString() + "'";
                using (var command = new NpgsqlCommand(sqlCommand, conn))
                {
                    int nRows = command.ExecuteNonQuery();
                }
            }
            return new JsonResult(new { redirectToUrl = Url.Action("AddMovie", "AddMedia") });
        }
    }
}
