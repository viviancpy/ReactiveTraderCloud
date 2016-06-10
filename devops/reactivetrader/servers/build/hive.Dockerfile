FROM        <% base.dotnet.image %>:<% base.dotnet.major %>.<% base.dotnet.minor %>
MAINTAINER  <% maintainer %>

COPY        server    /server

WORKDIR     /server/
CMD         dotnet restore
