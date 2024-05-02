
namespace Elixir.Models.Enums
{
    // TODO: AY - take a closer look at this idea - store roles in Models. Probably better to move to BL.
    public enum UserRoleEnum
    {
        Administrator = 0,
        Reviewer,
        Author,
        Member
    }
}
