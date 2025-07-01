using System.ComponentModel.DataAnnotations;

namespace BLL.DTO.CommonDto
{
    public class SearchParametersDto
    {
        [Range(1, int.MaxValue, ErrorMessage = "Номер сторінки повинен бути більше 0")]
        public int PageNumber { get; set; } = 1;

        [Range(1, 100, ErrorMessage = "Розмір сторінки повинен бути від 1 до 100")]
        public int PageSize { get; set; } = 10;

        public string? SearchTerm { get; set; }

        public string? SortBy { get; set; }

        public string? SortDirection { get; set; } = "asc";
    }
} 