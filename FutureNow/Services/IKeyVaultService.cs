using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public interface IKeyVaultService
    {
        Task<string> GetSecretAsync(string secretName);
    }
}
