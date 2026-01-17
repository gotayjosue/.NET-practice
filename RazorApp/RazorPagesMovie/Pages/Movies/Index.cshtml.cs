using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RazorPagesMovie.Pages_Movies
{
    public class IndexModel : PageModel
    {
        private readonly RazorPagesMovie.Data.RazorPagesMovieContext _context;

        public IndexModel(RazorPagesMovie.Data.RazorPagesMovieContext context)
        {
            _context = context;
        }

        public IList<Movie> Movie { get;set; } = default!;

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Genres { get; set; }

        public SelectList? ReleaseDates { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MovieGenre { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? MovieReleaseDate { get; set; }

        public async Task OnGetAsync()
        {
            IQueryable<string> dateQuery = from m in _context.Movie
                                            orderby m.ReleaseDate.ToString()
                                            select m.ReleaseDate.ToString();
            // <snippet_search_linqQuery>
            IQueryable<string> genreQuery = from m in _context.Movie
                                            orderby m.Genre
                                            select m.Genre;
            // </snippet_search_linqQuery>

            var movies = from m in _context.Movie
                        select m;

            if (!string.IsNullOrEmpty(SearchString))
            {
                movies = movies.Where(s => s.Title.Contains(SearchString));
            }

            if (!string.IsNullOrEmpty(MovieGenre))
            {
                movies = movies.Where(x => x.Genre == MovieGenre);
            }

            if (!string.IsNullOrEmpty(MovieReleaseDate))
            {
                movies = movies.Where(x => x.ReleaseDate.ToString() == MovieReleaseDate);
            }

            // <snippet_search_selectList>
            Genres = new SelectList(await genreQuery.Distinct().ToListAsync());
            // </snippet_search_selectList>
            ReleaseDates = new SelectList(await dateQuery.Distinct().ToListAsync());
            // </snippet_search_selectList>
            Movie = await movies.ToListAsync();

            }
        }
}
