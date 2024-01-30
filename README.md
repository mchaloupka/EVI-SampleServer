EVI: Sample server
===================

Sample SPARQL endpoint using RDB2RDF mechanism based on R2RML mapping

How to use the SPARQL endpoint using a docker image
--------------------------------------------

There is an autobuild docker image from this repository. It represents the easiest way to run the SPARQL endpoint or to build a custom image.

To execute the image, you can use the following command:

```shell
docker run --rm -it \
    -v {...}:/mapping \
    -e EVI_STORAGE__MAPPINGFILEPATH=/mapping/mapping.ttl \
    -e EVI_STORAGE__CONNECTIONSTRING="Server=host.docker.internal,1433;Database=benchmark=;User Id=sa;Password=p@ssw0rd" \
    -p 5000:8080 \
    mchaloupka/slp.evi:latest
```

Where you replace `{...}` by the path to the folder where you have the mapping file `mapping.ttl`. The environment variables then declares where to find the mapping and which connection string to use.

Or you can create your own image based on `mchaloupka/slp.evi:latest`:

* Application is located in `app`
* Update `appsettings.json` to suit your needs
* Use the following entrypoint `ENTRYPOINT [ "dotnet", "Slp.Evi.Endpoint.dll" ]`

How to build the server and use it with your own database
--------------------------------------------

.NET Core 8 is needed to build and run the endpoint.

In the repository you can find the `appsettings.json` file (in the Slp.Evi.Endpoint folder) where `ConnectionString` is configured. You can change it to anything you want. There is also `MappingFilePath` configuration where the path to the R2RML mapping file is configured.

To actually start the endpoint, you have to run `dotnet run` in the same folder. In the console you will see the address where the server is listening. It is a standard ASP.NET Core application, so you can use standard command line arguments to select the port etc.

How to use the server with a different database engine
---------------------------------------------

At this moment, the EVI solution supports both MS SQL and MySQL. In case you want to use the MySQL database, you have to change the `DatabaseType` in the `appsettings.json` file as follows:

```json
"Storage": {
  "DatabaseType": "MySql",
  "ConnectionString": "Server=localhost;Port=1454;User ID=root;Password=Password12!;Database=R2RMLTestStore",
  "MappingFilePath": "mapping.ttl",
  "QueryTimeout": 120
}
```

Or you can use environment variables to achieve the same:

```shell
docker run --rm -it \
    -v {...}:/mapping \
    -e EVI_STORAGE__DATABASETYPE=MySql
    -e EVI_STORAGE__MAPPINGFILEPATH=/mapping/mapping.ttl \
    -e EVI_STORAGE__CONNECTIONSTRING="Server=host.docker.internal;Port=3306;User ID=root;Password=psw;Database=benchmark" \
    -p 5000:8080 \
    mchaloupka/slp.evi:latest
```

The database is accessed using the [MySqlConnector](https://mysqlconnector.net/) package so the connection string needs to be formatted accordingly.

Local development
---------------------------------------------

To try the project out, you need to have database available. The simplest way to achieve that is to checkout also the [EVI](https://github.com/mchaloupka/EVI) package and run the following command:

```shell
dotnet fsi build.fsx -t RunTests
```

That starts the databases in docker and fills them with test data. It includes also BSBM data which are the default mapping file in the project.

See the EVI package documentation for more details, but to turn it off the following can be executed:

```shell
dotnet fsi build.fsx -t TearDownDatabases
```
