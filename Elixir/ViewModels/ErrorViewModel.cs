using System;

namespace Elixir.ViewModels
{
    public class ErrorViewModel
    {
        private const String DefaultTitle = "Error occurred during processing of your request";
        private const String DefaultDescription = "Details";

        public ErrorViewModel() : this(DefaultTitle, DefaultDescription)
        {
            
        }

        public ErrorViewModel(String description) : this(DefaultTitle, description)
        {
            Description = description;
        }

        public ErrorViewModel(String title, String description)
        {
            Title = title;
            Description = description;
        }

        public String Title { get; set; }
        public String Description { get; set; }
    }
}