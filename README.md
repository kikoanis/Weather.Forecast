## Features
The project was built using ASP.NET Core and SQLite as database.

```
$ git clone https://github.com/kikoanis/Weather.Forecast
```
## To Build in CLI
navigate to project main folder and then run
```
$ cd Weather.Forecast
dotnet build
```

## To Run in CLI

```
dotnet run --project .\src\Weather.Forecast.Web\Weather.Forecast.Web.csproj
```

Then you can access the API/ swagger at http://localhost:5000

Edit launchSettings.json if you need to use another port than 5000

## Alternatively, the solution can be opened in Visual Studio and run within (debug or relase mode)

## Scheduler
The project contains a task scheduler that will run every 10 minutes to check on cities weather forecast that were fetch more than 4 hours ago.

### There are 4 endpoints:

![image](https://user-images.githubusercontent.com/615849/148949811-dcda89e1-0376-461d-b76c-87df79d4961b.png)
