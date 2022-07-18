in config (for arch /etc/nginx/nginx.conf):
    ...
    proxy_pass         http://127.0.0.1:7248;
    ...

in Properties for profiles:
    "AppName": {
        ...
        "applicationUrl": "http://127.0.0.1:7248;",
        "environmentVariables": {
            "ASPNETCORE_ENVIRONMENT": "Production"
        }
        ...
    }

run DB PosgreSQL

> sudo -iu yournickname
> createdb [dropdb] gchat

[ > psql gchat ]
> sudo systemctl start postgresql.service

> dotnet run

> sudo systemctl start nginx
> dotnet run [dotnet run --environment Production]

go to http://your_ip/Account/Login