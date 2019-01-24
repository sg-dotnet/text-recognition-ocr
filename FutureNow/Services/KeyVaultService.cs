using Microsoft.Azure.KeyVault;
using Microsoft.Azure.KeyVault.Models;
using Microsoft.Azure.Services.AppAuthentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FutureNow.Services
{
    public class KeyVaultService : IKeyVaultService
    {
        public async Task<string> GetSecretAsync(string secretName)
        {
            int retries = 0;
            bool retry = false;

            do
            {
                try
                {
                    var azureServiceTokenProvider = new AzureServiceTokenProvider();
                    var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
                    var secret = await keyVaultClient.GetSecretAsync($"https://futurenow.vault.azure.net/secrets/{ secretName }")
                            .ConfigureAwait(false);

                    return secret.Value;
                }
                catch (KeyVaultErrorException keyVaultException)
                {
                    if ((int)keyVaultException.Response.StatusCode == 429)
                    {
                        retry = true;

                        long waitTime = Math.Min(getWaitTime(retries), 2000000);
                        Thread.Sleep(new TimeSpan(waitTime));
                    }
                }
            } while (retry && (retries++ < 10));

            return null;
        }

        // This method implements exponential backoff incase of 429 errors from Azure Key Vault
        private static long getWaitTime(int retryCount)
        {
            return (long)Math.Pow(2, retryCount) * 100L;
        }
    }
}
