using System.ComponentModel.DataAnnotations;
namespace QuHWBlazorWasmJwtUse.Server.Models
{
    public class User
    {
        [Key]
        public string Username{get;set;}=string.Empty;
#pragma warning disable CS8618
        public byte[] PasswordHash{get;set;}
        public byte[] PasswordSalt{get;set;}
#pragma warning restore CS8618
    }
}