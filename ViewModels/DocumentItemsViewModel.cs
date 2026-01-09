using Microsoft.AspNetCore.Mvc.Rendering;
using WmsCore.Data;
using WmsCore.Models;

namespace WmsCore.ViewModels
{
    public class DocumentItemsViewModel
    {
        public PaginatedList<Article> Articles { get; set; }
        public List<int> SelectedArticles { get; set; } = new(); //wybrane pozycje

        //Dokument
        public int DocumentId { get; set; }
        public string? DocumentType { get; set; }

        //Sortowanie paginacja
        public string? SortOrder { get; set; }
        public int? PageNumber { get; set; }

        //Filtrowanie
        public string? SearchString { get; set; }
        public int? CategoryId { get; set; }

        //Dropdowny
        public SelectList CategoryList { get; set; }

    }
}
