using Elixir.Areas.Admin.Models;
using Elixir.Attributes;
using Elixir.Models;
using Elixir.Models.Enums;
using Elixir.Models.Validation;
using Elixir.Utils.View;
using Elixir.ViewModels.Base;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Elixir.Areas.Admin.ViewModels
{
    public class CreatePostViewModel : BaseCUWithMultipleSelectViewModel<SocialPostModel>
    {
    }
}