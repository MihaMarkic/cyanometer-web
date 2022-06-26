# Development secrets

Secrets for production are stored in environmental variables while for development the secret storage is used.
https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows

Currently secrets are 
* SABRA_USERNAME (Sabra:Username for dev) and SABRA_PASSWORD (Sabra:Password for dev) for accessing FTP and upload tokens.
* AQICN_TOKEN (Aqicn:Token) for accessing World's Air Pollution data at https://waqi.info/