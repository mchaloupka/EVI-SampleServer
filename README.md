EVI: Sample server
===================

Sample SPARQL endpoint using RDB2RDF mechanism based on R2RML mapping

How to use the server with your own database
--------------------------------------------
.NET Core SDK 2.1 is needed to build and run the endpoint.

In the repository you can find the `appsettings.json` file (in the Slp.Evi.Endpoint folder) where `ConnectionString` is configured. You can change it to anything you want. There is also `MappingFilePath` configuration where the path to the R2RML mapping file is configured.

To actually start the endpoint, you have to run `dotnet run` in the same folder. In the console you will see the address where the server is listening. It is a standard ASP.NET Core application, so you can use standard command line arguments to select the port etc.
