using Microsoft.AspNetCore.Identity;

namespace Exam9.DAL.Models;

public class User : IdentityUser<int>
{
    public WalletAccount? WalletAccount { get; set; }
}